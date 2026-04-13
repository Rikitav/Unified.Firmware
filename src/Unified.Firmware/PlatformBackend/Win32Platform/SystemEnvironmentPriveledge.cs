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
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;

namespace Unified.Firmware.PlatformBackend.Win32Platform;

/// <summary>
/// Manages the temporary elevation of the current thread to enable the system environment privilege.
/// </summary>
/// <remarks>
/// This class acquires the system environment privilege for the lifetime of the instance on the CURRENT THREAD ONLY 
/// and reverts the privilege when disposed. It supports nested calls on the same thread via a reference counter.
/// WARNING: Because this relies on OS thread tokens, DO NOT use this across `await` points in async methods.
/// </remarks>
internal sealed class SystemEnvironmentPrivilege : IDisposable
{
    [ThreadStatic]
    private static int _referenceCount;

    [ThreadStatic]
    private static bool _impersonatedByUs;

    [ThreadStatic]
    private static NativeMethods.TokenPrivilege _previousState;

    private bool _disposed;
    private readonly int _owningThreadId;
    private readonly AsyncLocal<object> _asyncTrap;

    public SystemEnvironmentPrivilege()
    {
        _owningThreadId = Environment.CurrentManagedThreadId;
        _asyncTrap = new AsyncLocal<object>(OnAsyncFlowDetected);
        _asyncTrap.Value = new object();

        if (_referenceCount == 0)
            EnablePrivilege();

        _referenceCount++;
    }

    private void OnAsyncFlowDetected(AsyncLocalValueChangedArgs<object> args)
    {
        if (!args.ThreadContextChanged)
            return;

        if (args.CurrentValue == null)
            return;

        if (Environment.CurrentManagedThreadId != _owningThreadId)
        {
            Environment.FailFast(
                $"CRITICAL SECURITY VIOLATION: Execution flow escaped the authorized OS thread ({_owningThreadId}) " +
                $"and attempted to land on ThreadPool thread ({Environment.CurrentManagedThreadId}) while holding System Environment Privileges. " +
                "The process has been terminated to prevent a privilege escalation leak.");
        }
    }

    private void EnablePrivilege()
    {
        _impersonatedByUs = false;
        if (!NativeMethods.OpenThreadToken(NativeMethods.GetCurrentThread(), NativeMethods.TOKEN_ADJUST_PRIVILEGES | NativeMethods.TOKEN_QUERY, true, out IntPtr hToken))
        {
            int err = Marshal.GetLastWin32Error();
            if (err == NativeMethods.ERROR_NO_TOKEN)
            {
                if (!NativeMethods.ImpersonateSelf(NativeMethods.SecurityImpersonationLevel.SecurityImpersonation))
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to impersonate self");

                _impersonatedByUs = true;
                if (!NativeMethods.OpenThreadToken(NativeMethods.GetCurrentThread(), NativeMethods.TOKEN_ADJUST_PRIVILEGES | NativeMethods.TOKEN_QUERY, true, out hToken))
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to open thread token after impersonation");
            }
            else
            {
                throw new Win32Exception(err, "Failed to open thread token");
            }
        }

        try
        {
            NativeMethods.TokenPrivilege tp = new NativeMethods.TokenPrivilege
            {
                Count = 1,
                Attr = NativeMethods.SE_PRIVILEGE_ENABLED
            };

            if (!NativeMethods.LookupPrivilegeValue(null!, NativeMethods.SE_SYSTEM_ENVIRONMENT_NAME, ref tp.Luid))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to lookup privilege value");

            _previousState = new NativeMethods.TokenPrivilege();
            if (!NativeMethods.AdjustTokenPrivileges(hToken, false, ref tp, Marshal.SizeOf<NativeMethods.TokenPrivilege>(), ref _previousState, out _))
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to adjust thread token privileges");

            if (Marshal.GetLastWin32Error() == NativeMethods.ERROR_NOT_ALL_ASSIGNED)
                throw new Win32Exception(NativeMethods.ERROR_NOT_ALL_ASSIGNED, "The caller does not hold the SeSystemEnvironmentPrivilege");
        }
        finally
        {
            NativeMethods.CloseHandle(hToken);
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        if (_owningThreadId != Environment.CurrentManagedThreadId)
        {
            Environment.FailFast(
                "CRITICAL SECURITY FAILURE: SystemEnvironmentPrivilege was disposed on a different thread. " +
                "This is usually caused by using 'await' inside the scope. The OS thread pool is corrupted.");
        }

        if (_referenceCount > 0)
        {
            _referenceCount--;
            if (_referenceCount == 0)
                RevertPrivilege();
        }

        _disposed = true;
    }

    private void RevertPrivilege()
    {
        if (_impersonatedByUs)
        {
            NativeMethods.RevertToSelf();
        }
        else
        {
            if (NativeMethods.OpenThreadToken(NativeMethods.GetCurrentThread(), NativeMethods.TOKEN_ADJUST_PRIVILEGES, true, out IntPtr hToken))
            {
                try
                {
                    NativeMethods.AdjustTokenPrivileges(hToken, false, ref _previousState, 0, IntPtr.Zero, out _);
                }
                finally
                {
                    NativeMethods.CloseHandle(hToken);
                }
            }
        }
    }

    private static class NativeMethods
    {
        public const string SE_SYSTEM_ENVIRONMENT_NAME = "SeSystemEnvironmentPrivilege";
        public const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
        public const int TOKEN_QUERY = 0x00000008;
        public const int SE_PRIVILEGE_ENABLED = 0x00000002;
        public const int ERROR_NO_TOKEN = 1008;
        public const int ERROR_NOT_ALL_ASSIGNED = 1300;

        public enum SecurityImpersonationLevel
        {
            SecurityAnonymous,
            SecurityIdentification,
            SecurityImpersonation,
            SecurityDelegation
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TokenPrivilege
        {
            public int Count;
            public long Luid;
            public int Attr;
        }

        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern IntPtr GetCurrentThread();

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool OpenThreadToken(IntPtr ThreadHandle, int DesiredAccess, bool OpenAsSelf, out IntPtr TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool ImpersonateSelf(SecurityImpersonationLevel ImpersonationLevel);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool RevertToSelf();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool LookupPrivilegeValue(string host, string name, ref long pluid);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool AdjustTokenPrivileges(IntPtr TokenHandle, bool DisableAllPrivileges, ref TokenPrivilege NewState, int BufferLength, ref TokenPrivilege PreviousState, out int ReturnLength);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool AdjustTokenPrivileges(IntPtr TokenHandle, bool DisableAllPrivileges, ref TokenPrivilege NewState, int BufferLength, IntPtr PreviousState, out int ReturnLength);
    }
}
