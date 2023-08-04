using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace EntityFrameworkCore.Jet.Data;

internal static class Interop
{
    private static IReadOnlyCollection<string> GetNullTerminatedStrings(this byte[] buffer, int size, Encoding encoding)
    {
        var result = new List<string>();
        var length = size;
        var start = 0;
        while (length > 0)
        {
            var nullPosition = buffer.AsSpan(start, length).IndexOf((byte)0);
            if (nullPosition > 0)
            {
                var text = encoding.GetString(buffer, start, nullPosition);
                var offset = nullPosition + 1;
                start += offset;
                length -= offset;
                result.Add(text);
            }
            else
            {
                break;
            }
        }
        return result;
    }

    internal class Odbc
    {
        public static IEnumerable<OdbcDriver> GetInstalledDrivers(Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;

            foreach (var driverName in SqlGetInstalledDrivers(encoding))
            {
                var properties = new Dictionary<string, string>();
                foreach (var key in SqlGetPrivateProfileString(driverName, null, "", "ODBCINST.INI", encoding))
                {
                    var values = SqlGetPrivateProfileString(driverName, key, "", "ODBCINST.INI", encoding);
                    properties[key] = values.Single();
                }
                yield return new OdbcDriver(driverName, properties);
            }
        }

        private static IEnumerable<string> SqlGetPrivateProfileString(string? section, string? entry, string @default, string? fileName, Encoding encoding)
        {
            var pool = ArrayPool<byte>.Shared;
            var buffer = pool.Rent(4096);
            try
            {
                int size;
                if (OperatingSystem.IsWindows())
                    size = Windows.SQLGetPrivateProfileString(section, entry, @default, buffer, buffer.Length, fileName);
                else if (OperatingSystem.IsMacOS())
                    size = MacOS.SQLGetPrivateProfileString(section, entry, @default, buffer, buffer.Length, fileName);
                else
                    size = Linux.SQLGetPrivateProfileString(section, entry, @default, buffer, buffer.Length, fileName);

                return buffer.GetNullTerminatedStrings(size + 1, encoding);
            }
            finally
            {
                pool.Return(buffer);
            }
        }

        private static IEnumerable<string> SqlGetInstalledDrivers(Encoding encoding)
        {
            var pool = ArrayPool<byte>.Shared;
            var buffer = pool.Rent(4096);
            try
            {
                bool success;
                ushort size;
                if (OperatingSystem.IsWindows())
                    success = Windows.SQLGetInstalledDrivers(buffer, buffer.Length, out size) == 1;
                else if (OperatingSystem.IsMacOS())
                    success = MacOS.SQLGetInstalledDrivers(buffer, buffer.Length, out size) == 1;
                else
                    success = Linux.SQLGetInstalledDrivers(buffer, buffer.Length, out size) == 1;

                return success ? buffer.GetNullTerminatedStrings(size + 1, encoding) : Array.Empty<string>();
            }
            finally
            {
                pool.Return(buffer);
            }
        }
    }

    private static class Windows
    {
        /// <summary>
        /// Gets a list of names of values or data corresponding to a value of the system information.
        /// </summary>
        /// <param name="section">
        /// Points to a null-terminated string that specifies the section containing the key name.
        /// If this argument is <see langword="null" />, the function copies all section names in the file to the supplied buffer.
        /// </param>
        /// <param name="entry">
        /// Points to the null-terminated string containing the key name whose associated string is to be retrieved.
        /// If this argument is <see langword="null" />, all key names in the section specified by the <paramref name="section"/> argument are copied to the buffer specified by the <paramref name="buffer"/> argument.
        /// </param>
        /// <param name="default">
        /// Points to a null-terminated string that specifies the default value for the given key if the key cannot be found in the initialization file.
        /// This argument cannot be <see langword="null" />.
        /// </param>
        /// <param name="buffer">Points to the buffer that receives the retrieved string.</param>
        /// <param name="bufferSize">Specifies the size, in characters, of the buffer pointed to by the <paramref name="buffer"/> argument.</param>
        /// <param name="fileName">
        /// Points to a null-terminated string that names the initialization file.
        /// If this argument does not contain a full path to the file, the default directory is searched.
        /// </param>
        /// <remarks>
        /// The configuration mode (as set by SQLSetConfigMode) indicates where the Odbc.ini entry listing DSN values is in the system information.
        /// If the DSN is a User DSN (the configuration mode is USERDSN_ONLY), the function reads from the Odbc.ini entry in HKEY_CURRENT_USER.
        /// If the DSN is a System DSN (SYSTEMDSN_ONLY), the function reads from the Odbc.ini entry in HKEY_LOCAL_MACHINE.
        /// If the configuration mode is BOTHDSN, HKEY_CURRENT_USER is tried, and if it fails, HKEY_LOCAL_MACHINE is used.
        /// </remarks>
        /// <returns>An integer value that indicates the number of characters read.</returns>
        [DllImport("Odbccp32.dll")]
        internal static extern int SQLGetPrivateProfileString(string? section, string? entry, string? @default, byte[] buffer, int bufferSize, string? fileName);

        /// <summary>
        /// Reads the <c>[ODBC Drivers]</c> section of the system information and returns a list of descriptions of the installed drivers.
        /// </summary>
        /// <param name="buffer">List of descriptions of the installed drivers. For information about the list structure, see remarks.</param>
        /// <param name="bufferSize">Length of <paramref name="buffer"/>.</param>
        /// <param name="size">
        /// Total number of bytes (excluding the null-termination byte) returned in <paramref name="buffer"/>.
        /// If the number of bytes available to return is greater than or equal to the <paramref name="buffer"/> size,
        /// the list of driver descriptions in <paramref name="buffer"/> is truncated to <paramref name="bufferSize"/> minus the null-termination character.
        /// The <paramref name="size"/> argument can be a null pointer.</param>
        /// <remarks>
        /// Each driver description is terminated with a null byte, and the entire list is terminated with a null byte.
        /// (That is, two null bytes mark the end of the list.) If the allocated buffer is not large enough to hold the entire list,
        /// the list is truncated without error. An error is returned if a null pointer is passed in as <paramref name="buffer"/>.
        /// </remarks>
        /// <returns>The function returns <c>1</c> if it is successful, <c>0</c> if it fails.</returns>
        [DllImport("Odbccp32.dll")]
        internal static extern int SQLGetInstalledDrivers(byte[] buffer, int bufferSize, out ushort size);
    }

    private static class Linux
    {
        [DllImport("libodbcinst.so")]
        internal static extern int SQLGetPrivateProfileString(string? section, string? entry, string? @default, byte[] buffer, int bufferSize, string? fileName);

        [DllImport("libodbcinst.so")]
        internal static extern int SQLGetInstalledDrivers(byte[] buffer, int bufferSize, out ushort size);
    }

    private static class MacOS
    {
        [DllImport("odbcinst.2.dylib")]
        internal static extern int SQLGetPrivateProfileString(string? section, string? entry, string? @default, byte[] buffer, int bufferSize, string? fileName);

        [DllImport("odbcinst.2.dylib")]
        internal static extern int SQLGetInstalledDrivers(byte[] buffer, int bufferSize, out ushort size);
    }
}