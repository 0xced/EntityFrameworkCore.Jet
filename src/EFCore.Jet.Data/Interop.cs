using System.Runtime.InteropServices;

namespace EntityFrameworkCore.Jet.Data;

internal static class Interop
{
    internal static class Odbc
    {
        [DllImport("ODBCCP32.DLL")]
        internal static extern int SQLGetPrivateProfileString(string? section, string? entry, string? @default, byte[] buffer, int bufferSize, string? fileName);

        [DllImport("ODBCCP32.DLL")]
        internal static extern int SQLGetInstalledDrivers(byte[] buffer, int bufferSize, out ushort size);
    }
}