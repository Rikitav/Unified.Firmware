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

using System.Runtime.InteropServices;

namespace Unified.Firmware.PlatformBackend.LinuxPlatform;

internal static class VariableAttributesHelper
{
    public static void EnsureMutable(string filePath)
    {
        int fd = NativeMethods.open(filePath, NativeMethods.O_RDONLY);
        if (fd < 0)
            return;

        try
        {
            int flags = 0;
            if (NativeMethods.ioctl(fd, NativeMethods.FS_IOC_GETFLAGS, ref flags) == 0)
            {
                if ((flags & NativeMethods.FS_IMMUTABLE_FL) != 0)
                {
                    flags &= ~NativeMethods.FS_IMMUTABLE_FL;
                    int result = NativeMethods.ioctl(fd, NativeMethods.FS_IOC_SETFLAGS, ref flags);

                    /*
                    if (result < 0)
                        int error = Marshal.GetLastWin32Error();
                    */
                }
            }
        }
        finally
        {
            NativeMethods.close(fd);
        }
    }

    private static class NativeMethods
    {
        public const int O_RDONLY = 0x00;
        public const int FS_IMMUTABLE_FL = 0x00000010;

        public const int FS_IOC_GETFLAGS = unchecked((int)0x80086601);
        public const int FS_IOC_SETFLAGS = unchecked((int)0x40086602);

        [DllImport("libc", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern int open(string pathname, int flags);

        [DllImport("libc", SetLastError = true)]
        public static extern int ioctl(int fd, int request, ref int data);

        [DllImport("libc", SetLastError = true)]
        public static extern int close(int fd);
    }
}