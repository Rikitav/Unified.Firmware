// The MIT License (MIT)
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
using System.IO;
using System.Runtime.InteropServices;

namespace Unified.Firmware.BootService.Marshalling;

internal class MemoryPointerStream : Stream, IDisposable
{
    private readonly IntPtr _BufferLength;
    private readonly IntPtr _Buffer;
    private readonly bool _LeaveOpen;
    private long _CurPos;

    public override bool CanRead => true;
    public override bool CanSeek => true;
    public override bool CanWrite => true;
    public override long Length => _BufferLength.ToInt64();
    
    public override long Position
    {
        get => _CurPos;
        set => Seek(value, SeekOrigin.Begin);
    }

    public IntPtr NativeLength
    {
        get => _BufferLength;
    }

    public IntPtr Buffer
    {
        get => _Buffer;
    }

    public MemoryPointerStream(IntPtr buffer, int bufferLength, bool leaveOpen)
    {
        _Buffer = buffer;
        _BufferLength = new IntPtr(bufferLength);
        _LeaveOpen = leaveOpen;
    }

    public MemoryPointerStream(IntPtr buffer, IntPtr bufferLength, bool leaveOpen)
    {
        _Buffer = buffer;
        _BufferLength = bufferLength;
        _LeaveOpen = leaveOpen;
    }

    public MemoryPointerStream(int bufferLength)
    {
        _Buffer = Marshal.AllocHGlobal(bufferLength);
        _BufferLength = new IntPtr(bufferLength);
        _LeaveOpen = false;
    }

    public MemoryPointerStream(IntPtr bufferLength)
    {
        _Buffer = Marshal.AllocHGlobal(bufferLength);
        _BufferLength = bufferLength;
        _LeaveOpen = false;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (_CurPos == Length)
            throw new EndOfStreamException();

        if (_CurPos + count > Length)
            throw new ArgumentOutOfRangeException();

        for (int i = offset; i < count; i++)
            buffer[i] = Marshal.ReadByte(_Buffer, (int)_CurPos++);

        return count;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        if (_CurPos + count > Length)
            throw new ArgumentOutOfRangeException();

        for (int i = offset; i < count; i++)
            Marshal.WriteByte(_Buffer, (int)_CurPos++, buffer[i]);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        switch (origin)
        {
            case SeekOrigin.Begin:
                {
                    if (offset > Length)
                        throw new ArgumentOutOfRangeException();

                    _CurPos = offset;
                    break;
                }

            case SeekOrigin.Current:
                {
                    if (_CurPos + offset > Length)
                        throw new ArgumentOutOfRangeException();

                    _CurPos += offset;
                    break;
                }

            case SeekOrigin.End:
                {
                    if (offset > Length)
                        throw new ArgumentOutOfRangeException();

                    _CurPos = Length - offset;
                    break;
                }
        }

        return _CurPos;
    }

    public override void SetLength(long value)
    {
        throw new InvalidOperationException();
    }

    public override void Flush()
    {
        _ = 0xBAD + 0xC0DE;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && !_LeaveOpen)
            Marshal.FreeHGlobal(_Buffer);

        base.Dispose(disposing);
    }
}
