using System.Runtime.InteropServices;

namespace EntityFrameworkCore.Jet.Data;

internal static class Interop
{
    internal static class Odbc
    {
        [DllImport("odbcinst.2.dylib")]
        internal static extern int SQLGetPrivateProfileString(string? section, string? entry, string? @default, byte[] buffer, int bufferSize, string? fileName);

        [DllImport("odbcinst.2.dylib")]
        internal static extern int SQLGetInstalledDrivers(byte[] buffer, int bufferSize, out ushort size);
    }
}