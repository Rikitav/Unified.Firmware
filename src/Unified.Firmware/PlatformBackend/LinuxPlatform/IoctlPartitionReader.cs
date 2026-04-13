// The MIT License(MIT)
// 
// Unified.Firmware
// Copyright 2026 © Rikitav Tim4ik
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Unified.Firmware.SystemPartition;

namespace Unified.Firmware.PlatformBackend.LinuxPlatform;

internal static class IoctlPartitionReader
{
    private const ulong EfiPartSignature = 0x5452415020494645UL;
    private const string SysBlockPath = "/sys/block/";

    public static IEnumerable<GptPartitionModel> FindAllEfiPartitionsInSystem()
    {
        if (!Directory.Exists(SysBlockPath))
            yield break;

        foreach (string diskDir in Directory.EnumerateDirectories(SysBlockPath))
        {
            string diskName = Path.GetFileName(diskDir);
            if (diskName.StartsWith("loop") || diskName.StartsWith("ram") || diskName.StartsWith("dm-"))
                continue;

            string devicePath = $"/dev/{diskName}";
            foreach (GptPartitionModel espEntry in FindEspPartitions(devicePath))
                yield return espEntry;
        }
    }

    public static IEnumerable<GptPartitionModel> FindEspPartitions(string blockDevicePath, int logicalSectorSize = 512)
    {
        try
        {
            List<GptPartitionModel> espPartitions = [];
            using FileStream fs = new FileStream(
                blockDevicePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite,
                1, // Disabling internal bufferization
                FileOptions.None);

            fs.Position = logicalSectorSize;

            Span<byte> headerBuffer = stackalloc byte[logicalSectorSize];
            int bytesRead = fs.Read(headerBuffer);

            if (bytesRead < logicalSectorSize)
                return espPartitions;

            GPT_PARTITION_HEADER header = MemoryMarshal.Read<GPT_PARTITION_HEADER>(headerBuffer);
            if (header.Signature != EfiPartSignature)
                return espPartitions;

            long entriesOffset = (long)header.PartitionEntryLba * logicalSectorSize;
            int entryArrayTotalSize = (int)(header.NumberOfPartitionEntries * header.SizeOfPartitionEntry);

            byte[] entriesBuffer = ArrayPool<byte>.Shared.Rent(entryArrayTotalSize);
            try
            {
                fs.Position = entriesOffset;

                Span<byte> activeEntriesSpan = entriesBuffer.AsSpan(0, entryArrayTotalSize);
                bytesRead = fs.Read(activeEntriesSpan);

                if (bytesRead != entryArrayTotalSize)
                    return espPartitions;

                for (int i = 0; i < header.NumberOfPartitionEntries; i++)
                {
                    int entryStartOffset = i * (int)header.SizeOfPartitionEntry;
                    Span<byte> singleEntrySpan = activeEntriesSpan.Slice(entryStartOffset, (int)header.SizeOfPartitionEntry);

                    GPT_PARTITION_ENTRY entry = MemoryMarshal.Read<GPT_PARTITION_ENTRY>(singleEntrySpan);
                    if (entry.PartitionTypeGuid == EfiPartition.TypeID)
                        espPartitions.Add(new GptPartitionModel(entry, blockDevicePath, GetPartitionNodeName(blockDevicePath, i + 1)));
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(entriesBuffer);
            }

            return espPartitions;
        }
        catch (UnauthorizedAccessException)
        {
            // Игнорируем, если нет прав root на чтение сырого блочного девайса
            return [];
        }
        catch (IOException)
        {
            // Игнорируем недоступные устройства
            return [];
        }
    }

    private static string GetPartitionNodeName(string devicePath, int partitionIndex)
    {
        char lastChar = devicePath[devicePath.Length - 1];
        if (char.IsDigit(lastChar))
            return $"{devicePath}p{partitionIndex}";

        return $"{devicePath}{partitionIndex}";
    }
}
