using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Couchbase.Lite;

namespace Even.Persistence.CouchbaseLite.Testing
{
    public static class TestingUtility
    {
        public const string TestDbNamePrefix = "even-cbl-store-tests";
        public const string DefaultTestDbName = TestDbNamePrefix + "-default";



        public static Database GetDatabaseForCaller([CallerMemberName]string callerName = null)
        {
            var dbName = $"{TestDbNamePrefix}-{callerName?.Replace(".", "") ?? "(no-caller)"}".ToLowerInvariant();            
            return Manager.SharedInstance.GetDatabase(dbName);
        }

        public static Database GetIsolatedDatabase()
        {
            string dbName;
            do
            {
                dbName = $"{TestDbNamePrefix}-{Guid.NewGuid()}".ToLowerInvariant();
            } while (Manager.SharedInstance.AllDatabaseNames.Contains(dbName));

            return Manager.SharedInstance.GetDatabase(dbName);
        }

        public static Database GetIsolatedDatabaseForCaller([CallerMemberName]string callerName=null)
        {
            string dbName;
            var suffix = callerName?.Replace(".","") ?? (Guid.NewGuid().ToString());
            int idx = 0;
            do
            {
                dbName = $"{TestDbNamePrefix}-{suffix}-{idx}".ToLowerInvariant();
                idx++;
            } while (Manager.SharedInstance.AllDatabaseNames.Contains(dbName));

            return Manager.SharedInstance.GetDatabase(dbName);
        }        

        public static void WithIsolatedDatabase(Action<Database> action)
        {
            using (var db = GetIsolatedDatabase())
            {
                action(db);
                db.Delete();
            }                
        }
    }
}