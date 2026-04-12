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

using Unified.Firmware.Win32Native;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Unified.Firmware;

/// <summary>
/// Features for interacting with UEFI
/// </summary>
public static class FirmwareUtilities
{
    /// <summary>
    /// Determines whether firmware environment variable functionality is available on the current system.
    /// </summary>
    /// <remarks>
    /// Firmware environment variables are typically used for advanced system configuration and are
    /// only supported on certain platforms, such as Windows running on UEFI firmware. This method can be used to check
    /// for support before attempting operations that require firmware environment variables.
    /// </remarks>
    /// <returns>true if firmware environment variable support is available; otherwise, false.</returns>
    public static bool CheckFirmwareAvailablity()
    {
        _ = NativeMethods.GetFirmwareEnvironmentVariableExW("", "{00000000-0000-0000-0000-000000000000}", IntPtr.Zero, 0, out _);
        return Marshal.GetLastWin32Error() != NativeMethods.ERROR_INVALID_FUNCTION;
    }

    /// <summary>
    /// Retrieves the value of a global environment variable as a string.
    /// </summary>
    /// <remarks>
    /// The returned value is read from the global environment. If the specified variable is not found, the method returns null.
    /// This method allocates and frees unmanaged memory internally, callers do not need to manage memory for the returned string.
    /// </remarks>
    /// <param name="VarName">The name of the environment variable to retrieve. Cannot be null or empty.</param>
    /// <returns>A string containing the value of the specified environment variable, or null if the variable does not exist.</returns>
    public static string ReadStringVariable(string VarName)
    {
        IntPtr pointer = GetGlobalEnvironmentVariable(VarName, out _, 1024);
        string varVal = Marshal.PtrToStringUni(pointer);
        Marshal.FreeHGlobal(pointer);
        return varVal;
    }

    /// <summary>
    /// Sets a global environment variable with the specified name to the provided string value.
    /// </summary>
    /// <remarks>
    /// The variable is set globally and may affect other processes or components that read environment variables.
    /// The value is stored as a Unicode string. Ensure that the variable name and value conform to any platform-specific restrictions.
    /// </remarks>
    /// <param name="VarName">The name of the environment variable to set. Cannot be null or empty.</param>
    /// <param name="Value">The string value to assign to the environment variable. Cannot be null.</param>
    public static void WriteStringVariable(string VarName, string Value)
    {
        IntPtr pointer = Marshal.StringToHGlobalUni(Value);
        SetGlobalEnvironmentVariable(VarName, pointer, Value.Length * 2);
        Marshal.FreeHGlobal(pointer);
    }

    /// <summary>
    /// Retrieves the value of a global environment variable and returns it as a value type.
    /// </summary>
    /// <remarks>
    /// This method reads the variable from the global environment and marshals it to the specified value type.
    /// The caller must ensure that the variable exists and is compatible with the requested type.
    /// If the variable does not exist or cannot be marshaled to type T, the result may be undefined.
    /// </remarks>
    /// <typeparam name="T">The value type to which the environment variable will be marshaled. Must be a struct.</typeparam>
    /// <param name="VarName">The name of the global environment variable to read. Cannot be null or empty.</param>
    /// <returns>The value of the specified environment variable, marshaled to type T.</returns>
    public static T ReadVariable<T>(string VarName) where T : struct
    {
        // Getting variable data
        int ptrSize = Marshal.SizeOf<T>();
        IntPtr pointer = GetGlobalEnvironmentVariable(VarName, out _, ptrSize);

        T varVal = Marshal.PtrToStructure<T>(pointer);
        Marshal.FreeHGlobal(pointer);
        return varVal;
    }

    /// <summary>
    /// Writes a value of a specified struct type to a global environment variable identified by name.
    /// </summary>
    /// <remarks>
    /// This method stores the value as a binary representation using unmanaged memory.
    /// The caller should ensure that the variable name is unique and valid within the global environment.
    /// Only value types (structs) are supported.</remarks>
    /// <typeparam name="T">The struct type of the value to write to the environment variable.</typeparam>
    /// <param name="VarName">The name of the global environment variable to set. Cannot be null or empty.</param>
    /// <param name="Value">The value to assign to the environment variable.</param>
    public static void WriteVariable<T>(string VarName, T Value) where T : struct
    {
        // Getting variable data
        int ptrSize = Marshal.SizeOf<T>();
        IntPtr pointer = Marshal.AllocHGlobal(ptrSize);

        // Writing data
        Marshal.StructureToPtr(Value, pointer, false);
        SetGlobalEnvironmentVariable(VarName, pointer, ptrSize);

        // Freeing
        Marshal.FreeHGlobal(pointer);
    }

    /// <summary>
    /// Retrieves the value of a global environment variable as an array of value type elements.
    /// </summary>
    /// <remarks>
    /// The method reads the raw memory of the environment variable and interprets it as an array of the specified value type.
    /// Use this method only when the variable is known to contain binary data compatible with the type T.
    /// The caller is responsible for ensuring the variable's format matches the expected type.
    /// </remarks>
    /// <typeparam name="T">The value type of the elements to read from the environment variable.</typeparam>
    /// <param name="VarName">The name of the global environment variable to retrieve. Cannot be null or empty.</param>
    /// <returns>An array of elements of type T containing the values stored in the specified environment variable. The array
    /// will be empty if the variable contains no data.</returns>
    public static T[] ReadArrayVariable<T>(string VarName) where T : struct
    {
        // Getting variable data
        int ptrSize = Marshal.SizeOf<T>();
        IntPtr Data = GetGlobalEnvironmentVariable(VarName, out int DataSize, 256);

        // Parsing values
        int length = DataSize / ptrSize;
        T[] varVal = Data.PinnedMemoryToArray<T>(length);

        // Freeing
        Marshal.FreeHGlobal(Data);
        return varVal;
    }

    /// <summary>
    /// Writes a global environment variable as an array of value type elements.
    /// </summary>
    /// <remarks>
    /// This method pins the array in memory and writes its contents to the specified global environment variable.
    /// The variable is set as a contiguous block of memory representing the array.
    /// Use caution when passing large arrays, as this operation involves unmanaged memory handling.
    /// </remarks>
    /// <typeparam name="T">The value type of the elements in the array to be written.</typeparam>
    /// <param name="VarName">The name of the global environment variable to set. Cannot be null or empty.</param>
    /// <param name="Value">The array of value type elements to write. Cannot be null and must contain at least one element.</param>
    public static void WriteArrayVariable<T>(string VarName, T[] Value) where T : struct
    {
        // Formating new value
        int ptrSize = Marshal.SizeOf<T>();
        IntPtr handle = Marshal.UnsafeAddrOfPinnedArrayElement(Value, 0);

        // Setting variable
        SetGlobalEnvironmentVariable(VarName, handle, Value.Length * ptrSize);
    }

    /// <summary>
    /// Sets a global environment variable with the specified name and value pointer.
    /// </summary>
    /// <remarks>
    /// This method sets a global environment variable using a native identifier. Use this method
    /// when you need to update environment variables that are shared across the firmware or system context. The caller
    /// is responsible for ensuring that the pointer and size are valid and that the memory referenced remains
    /// accessible for the duration required by the environment.
    /// </remarks>
    /// <param name="VarName">The name of the environment variable to set. Cannot be null or empty.</param>
    /// <param name="Value">A pointer to the value to assign to the environment variable.</param>
    /// <param name="PtrSize">The size, in bytes, of the value pointed. Must be greater than zero.</param>
    public static void SetGlobalEnvironmentVariable(string VarName, IntPtr Value, int PtrSize)
        => SetEnvironmentVariable(VarName, NativeMethods.FirmwareGlobalEnvironmentIdentifier, Value, PtrSize);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="VarName"></param>
    /// <param name="DataLength"></param>
    /// <param name="VarSize"></param>
    /// <returns></returns>
    public static IntPtr GetGlobalEnvironmentVariable(string VarName, out int DataLength, int VarSize = 4)
        => GetEnvironmentVariable(VarName, NativeMethods.FirmwareGlobalEnvironmentIdentifier, out DataLength, VarSize);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="VarName"></param>
    /// <param name="attributes"></param>
    /// <param name="Value"></param>
    /// <param name="PtrSize"></param>
    public static void SetGlobalEnvironmentVariableEx(string VarName, VariableAttributes attributes, IntPtr Value, int PtrSize)
        => SetEnvironmentVariableEx(VarName, NativeMethods.FirmwareGlobalEnvironmentIdentifier, attributes, Value, PtrSize);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="VarName"></param>
    /// <param name="attributes"></param>
    /// <param name="DataLength"></param>
    /// <param name="VarSize"></param>
    /// <returns></returns>
    public static IntPtr GetGlobalEnvironmentVariableEx(string VarName, out VariableAttributes attributes, out int DataLength, int VarSize = 4)
        => GetEnvironmentVariableEx(VarName, NativeMethods.FirmwareGlobalEnvironmentIdentifier, out attributes, out DataLength, VarSize);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="VarName"></param>
    /// <param name="EnvironmentIdentificator"></param>
    /// <param name="Value"></param>
    /// <param name="PtrSize"></param>
    /// <exception cref="PlatformNotSupportedException"></exception>
    /// <exception cref="FirmwareEnvironmentException"></exception>
    public static void SetEnvironmentVariable(string VarName, Guid EnvironmentIdentificator, IntPtr Value, int PtrSize)
    {
        if (!FirmwareInterface.Available)
            throw new PlatformNotSupportedException("This system does not support UEFI, or is loaded in LEGACY mode");

        try
        {
            // Execution and error check
            using (new SystemEnvironmentPrivilege())
            {
                if (!NativeMethods.SetFirmwareEnvironmentVariableW(VarName, "{" + EnvironmentIdentificator.ToString() + "}", Value, PtrSize))
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    throw new Win32Exception(errorCode);
                }
            }
        }
        catch (Exception ex)
        {
            // Something wrong happened
            throw new FirmwareEnvironmentException("Failed to write environment variable", ex);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="VarName"></param>
    /// <param name="EnvironmentIdentificator"></param>
    /// <param name="DataLength"></param>
    /// <param name="VarSize"></param>
    /// <returns></returns>
    /// <exception cref="PlatformNotSupportedException"></exception>
    /// <exception cref="FirmwareEnvironmentException"></exception>
    public static IntPtr GetEnvironmentVariable(string VarName, Guid EnvironmentIdentificator, out int DataLength, int VarSize = 4) // 4 - int size
    {
        if (!FirmwareInterface.Available)
            throw new PlatformNotSupportedException("This system does not support UEFI, or is loaded in LEGACY mode");

        // Data
        IntPtr pointer = IntPtr.Zero;
        DataLength = 0;

        try
        {
            // Allocate variable pointer
            pointer = Marshal.AllocHGlobal(VarSize);

            // Reading variable
            using (new SystemEnvironmentPrivilege())
            {
                DataLength = (int)NativeMethods.GetFirmwareEnvironmentVariableW(VarName, "{" + EnvironmentIdentificator.ToString() + "}", pointer, VarSize);
            }

            // Error check
            if (DataLength == 0)
            {
                Marshal.FreeHGlobal(pointer);
                int errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode);
            }

            return pointer;
        }
        catch (Exception ex)
        {
            // Something wrong happened
            Marshal.FreeHGlobal(pointer);
            throw new FirmwareEnvironmentException("Failed to read environment variable", ex);
        }
    }

    /// <summary>
    /// Sets the value of an environment variable with the specified name and attributes in the specified environment.
    /// The variable is stored in non-volatile memory and will persist across system reboots.
    /// The variable is associated with the specified environment, which is identified by a GUID.
    /// The attributes parameter specifies the attributes of the variable, such as whether it is read-only or can be accessed from user mode.
    /// The value parameter is a pointer to a buffer that contains the data to be stored in the variable, and ptrSize specifies the size of the buffer in bytes.
    /// </summary>
    /// <param name="VarName"></param>
    /// <param name="EnvironmentIdentificator"></param>
    /// <param name="attributes"></param>
    /// <param name="Value"></param>
    /// <param name="PtrSize"></param>
    /// <exception cref="PlatformNotSupportedException"></exception>
    /// <exception cref="FirmwareEnvironmentException"></exception>
    public static void SetEnvironmentVariableEx(string VarName, Guid EnvironmentIdentificator, VariableAttributes attributes, IntPtr Value, int PtrSize)
    {
        if (!FirmwareInterface.Available)
            throw new PlatformNotSupportedException("This system does not support UEFI, or is loaded in LEGACY mode");

        try
        {
            // Execution and error check
            using (new SystemEnvironmentPrivilege())
            {
                if (!NativeMethods.SetFirmwareEnvironmentVariableExW(VarName, "{" + EnvironmentIdentificator.ToString() + "}", Value, PtrSize, attributes))
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    throw new Win32Exception(errorCode);
                }
            }
        }
        catch (Exception ex)
        {
            // Something wrong happened
            throw new FirmwareEnvironmentException("Failed to write environment variable", ex);
        }
    }

    /// <summary>
    /// Gets the value of an environment variable with the specified name and attributes from the specified environment.
    /// </summary>
    /// <param name="VarName"></param>
    /// <param name="EnvironmentIdentificator"></param>
    /// <param name="attributes"></param>
    /// <param name="DataLength"></param>
    /// <param name="VarSize"></param>
    /// <returns></returns>
    /// <exception cref="PlatformNotSupportedException"></exception>
    /// <exception cref="FirmwareEnvironmentException"></exception>
    public static IntPtr GetEnvironmentVariableEx(string VarName, Guid EnvironmentIdentificator, out VariableAttributes attributes, out int DataLength, int VarSize = 4) // 4 - int size
    {
        // Data
        IntPtr pointer = IntPtr.Zero;
        DataLength = 0;

        if (!FirmwareInterface.Available)
            throw new PlatformNotSupportedException("This system does not support UEFI, or is loaded in LEGACY mode");

        try
        {
            // Allocate variable pointer
            pointer = Marshal.AllocHGlobal(VarSize);

            // Reading variable
            using (new SystemEnvironmentPrivilege())
                DataLength = (int)NativeMethods.GetFirmwareEnvironmentVariableExW(VarName, "{" + EnvironmentIdentificator.ToString() + "}", pointer, VarSize, out attributes);

            // Error check
            if (DataLength == 0)
            {
                Marshal.FreeHGlobal(pointer);
                int errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode);
            }

            return pointer;
        }
        catch (Exception ex)
        {
            // Something wrong happened
            Marshal.FreeHGlobal(pointer);
            throw new FirmwareEnvironmentException("Failed to read environment variable", ex);
        }
    }

    /// <summary>
    /// Manages the temporary elevation of the current thread to enable the system environment privilege.
    /// </summary>
    /// <remarks>
    /// This class acquires the system environment privilege for the lifetime of the instance on the CURRENT THREAD ONLY 
    /// and reverts the privilege when disposed. It supports nested calls on the same thread via a reference counter.
    /// WARNING: Because this relies on OS thread tokens, DO NOT use this across `await` points in async methods.
    /// </remarks>
    private sealed class SystemEnvironmentPrivilege : IDisposable
    {
        // ����������������� (������������� ��� ������� ������) ���������� ���������
        [ThreadStatic]
        private static int _referenceCount;

        [ThreadStatic]
        private static bool _impersonatedByUs;

        [ThreadStatic]
        private static NativeMethods.TokenPrivilege _previousState;

        private bool _disposed;

        public SystemEnvironmentPrivilege()
        {
            if (_referenceCount == 0)
            {
                EnablePrivilege();
            }
            _referenceCount++;
        }

        private void EnablePrivilege()
        {
            IntPtr hToken = IntPtr.Zero;
            _impersonatedByUs = false;

            // �������� �������� ����� �������� ������
            if (!NativeMethods.OpenThreadToken(NativeMethods.GetCurrentThread(), NativeMethods.TOKEN_ADJUST_PRIVILEGES | NativeMethods.TOKEN_QUERY, true, out hToken))
            {
                int err = Marshal.GetLastWin32Error();
                if (err == NativeMethods.ERROR_NO_TOKEN)
                {
                    // � ������ ��� ������ (�� ���������� ����� ��������). 
                    // ������� ����� ������������� (impersonation token) ��� �������� ������.
                    if (!NativeMethods.ImpersonateSelf(NativeMethods.SecurityImpersonationLevel.SecurityImpersonation))
                        throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to impersonate self");

                    _impersonatedByUs = true;

                    // ����� �������� �������� ����� ������
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

                // �������� LUID ��� ����������
                if (!NativeMethods.LookupPrivilegeValue(null, NativeMethods.SE_SYSTEM_ENVIRONMENT_NAME, ref tp.Luid))
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to lookup privilege value");

                _previousState = new NativeMethods.TokenPrivilege();

                // �������� ���������� ��� ������ ������, �������� ���������� ���������
                if (!NativeMethods.AdjustTokenPrivileges(hToken, false, ref tp, Marshal.SizeOf<NativeMethods.TokenPrivilege>(), ref _previousState, out _))
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to adjust thread token privileges");

                // AdjustTokenPrivileges ����� ������� true, �� �� ��������� ����������, ���� � ����� � � �������� ���
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

            if (_referenceCount > 0)
            {
                _referenceCount--;

                if (_referenceCount == 0)
                {
                    RevertPrivilege();
                }
            }

            _disposed = true;
        }

        private void RevertPrivilege()
        {
            if (_impersonatedByUs)
            {
                // ���� �� ���� ��������� ����� ��� ������, ������ ���������� ��� � ������������ � ������ ��������
                NativeMethods.RevertToSelf();
            }
            else
            {
                // ���� � ������ ��� ��� ����� �� ���, ���������� ��� ����������� ��������� �������
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
    }

    private static class NativeMethods
    {
        public static readonly Guid FirmwareGlobalEnvironmentIdentifier = new Guid("8BE4DF61-93CA-11D2-AA0D-00E098032B8C");
        public const string SE_SYSTEM_ENVIRONMENT_NAME = "SeSystemEnvironmentPrivilege";
        public const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
        public const int TOKEN_QUERY = 0x00000008;
        public const int SE_PRIVILEGE_ENABLED = 0x00000002;
        public const int ERROR_INVALID_FUNCTION = 1;
        public const int ERROR_NO_TOKEN = 1008;
        public const int ERROR_NOT_ALL_ASSIGNED = 1300;

        public enum SecurityImpersonationLevel
        {
            SecurityAnonymous,
            SecurityIdentification,
            SecurityImpersonation,
            SecurityDelegation
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

        // ������ CallingConvention.Cdecl - WinAPI �� ��������� ���������� StdCall
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern uint GetFirmwareEnvironmentVariableExW(
            string lpName,
            string lpGuid,
            IntPtr pBuffer,
            int nSize,
            out VariableAttributes Attributes);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool SetFirmwareEnvironmentVariableExW(
            string lpName,
            string lpGuid,
            IntPtr pValue,
            int nSize,
            VariableAttributes Attributes);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern uint GetFirmwareEnvironmentVariableW(
            string lpName,
            string lpGuid,
            IntPtr pBuffer,
            int nSize);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool SetFirmwareEnvironmentVariableW(
            string lpName,
            string lpGuid,
            IntPtr pValue,
            int nSize);

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TokenPrivilege
        {
            public int Count;
            public long Luid;
            public int Attr;
        }
    }
}

/// <summary>
/// Represents an error that occurred while working with UEFI
/// </summary>
public class FirmwareEnvironmentException(string message, Exception? innerException = null) : Exception(message, innerException)
{
    /// <inheritdoc/>
    public FirmwareEnvironmentException()
        : this(string.Empty, null) { }

    /// <inheritdoc/>
    public FirmwareEnvironmentException(string message)
        : this(message, null) { }
}
