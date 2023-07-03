using System.Runtime.Versioning;

namespace EntityFrameworkCore.Jet.Data
{
    public class PreciseDatabaseCreator
        : JetDatabaseCreator
    {
        [SupportedOSPlatform("windows")]
        public override void CreateDatabase(
            string fileNameOrConnectionString,
            DatabaseVersion version = DatabaseVersion.NewestSupported,
            CollatingOrder collatingOrder = CollatingOrder.General,
            string? databasePassword = null)
            => new DaoDatabaseCreator().CreateDatabase(fileNameOrConnectionString, version, collatingOrder, databasePassword);
    }
}