using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace EntityFrameworkCore.Jet.Data;

internal static class Interop
{
    private static IEnumerable<string> EnumerateNullTerminatedStrings(this byte[] buffer, int size, Encoding encoding)
    {
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
                yield return text;
            }
            else
            {
                break;
            }
        }
    }

    internal class Odbc
    {
        public static IEnumerable<OdbcDriver> GetInstalledDrivers(Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;

            var pool = ArrayPool<byte>.Shared;
            var driversBuffer = pool.Rent(4096);

            if (SQLGetInstalledDrivers(driversBuffer, out var size))
            {
                foreach (var driverName in driversBuffer.EnumerateNullTerminatedStrings(size, encoding))
                {
                    var properties = new Dictionary<string, string>();
                    var keysBuffer = pool.Rent(4096);
                    var keysSize = SQLGetPrivateProfileString(driverName, null, "", keysBuffer, keysBuffer.Length, "ODBCINST.INI");
                    foreach (var key in keysBuffer.EnumerateNullTerminatedStrings(keysSize, encoding))
                    {
                        var valueBuffer = pool.Rent(4096);
                        var valueSize = SQLGetPrivateProfileString(driverName, key, "", valueBuffer, valueBuffer.Length, "ODBCINST.INI");
                        properties[key] = encoding.GetString(valueBuffer, 0, valueSize);
                        pool.Return(valueBuffer);
                    }
                    pool.Return(keysBuffer);
                    yield return new OdbcDriver(driverName, properties);
                }
            }

            pool.Return(driversBuffer);
        }

        internal static int SQLGetPrivateProfileString(string? section, string? entry, string @default, byte[] buffer, int bufferSize, string? fileName)
        {
            if (OperatingSystem.IsWindows())
            {
                return Windows.SQLGetPrivateProfileString(section, entry, @default, buffer, bufferSize, fileName);
            }

            if (OperatingSystem.IsMacOS())
            {
                return MacOS.SQLGetPrivateProfileString(section, entry, @default, buffer, bufferSize, fileName);
            }

            return Linux.SQLGetPrivateProfileString(section, entry, @default, buffer, bufferSize, fileName);
        }

        /// <summary>
        /// SQLGetInstalledDrivers reads the <c>[ODBC Drivers]</c> section of the system information and returns a list of descriptions of the installed drivers.
        /// </summary>
        /// <param name="buffer">List of descriptions of the installed drivers. For information about the list structure, see remarks.</param>
        /// <param name="size">
        /// Total number of bytes (excluding the null-termination byte) returned in <paramref name="buffer"/>.
        /// If the number of bytes available to return is greater than or equal to the <paramref name="buffer"/> size,
        /// the list of driver descriptions in lpszBuf is truncated to cbBufMax minus the null-termination character.
        /// The <paramref name="size"/> argument can be a null pointer.</param>
        /// <remarks>
        /// Each driver description is terminated with a null byte, and the entire list is terminated with a null byte.
        /// (That is, two null bytes mark the end of the list.) If the allocated buffer is not large enough to hold the entire list,
        /// the list is truncated without error. An error is returned if a null pointer is passed in as <paramref name="buffer"/>.
        /// </remarks>
        /// <returns>The function returns <see langword="true"/> if it is successful, <see langword="false"/> if it fails.</returns>
        internal static bool SQLGetInstalledDrivers(byte[] buffer, out ushort size)
        {
            if (OperatingSystem.IsWindows())
            {
                return Windows.SQLGetInstalledDrivers(buffer, buffer.Length, out size) == 1;
            }

            if (OperatingSystem.IsMacOS())
            {
                return MacOS.SQLGetInstalledDrivers(buffer, buffer.Length, out size) == 1;
            }

            return Linux.SQLGetInstalledDrivers(buffer, buffer.Length, out size) == 1;
        }
    }

    private static class Windows
    {
        [DllImport("Odbccp32.dll")]
        internal static extern int SQLGetPrivateProfileString(string? section, string? entry, string? @default, byte[] buffer, int bufferSize, string? fileName);

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