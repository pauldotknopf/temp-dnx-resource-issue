using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.SqlServer.Dac;

namespace TestDacResourceResolver
{
    public class Program
    {
        const string ConnectionString = @"Data Source=(localdb)\DOESNTMATTER;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False";

        public static void Main(string[] args)
        {
            try
            {
                var appEnv =
                    CallContextServiceLocator.Locator.ServiceProvider.GetService(typeof (IApplicationEnvironment)) as
                        IApplicationEnvironment;
                if (appEnv == null) throw new Exception("Couldn't get application environment to get base path.");

                var dacPackageLocation = Path.Combine(appEnv.ApplicationBasePath, "database.dacpac");


                var dacPackage = DacPackage.Load(dacPackageLocation);
                var dacServices = new DacServices(ConnectionString);

                dacServices.Deploy(dacPackage, "DOESNTMATTER", true, new DacDeployOptions
                {
                    GenerateSmartDefaults = true,
                    CreateNewDatabase = true
                });
            }
            catch (Exception ex)
            {
                // Normally, you should get a "connection can't be made exception", becase the connection string is garbage.
                // This is expected and normal.
                // However, we get a "resource" not found exception. This exception goes away when we move
                // "Microsoft.Data.Tools.Schema.Sql.dll" and "Microsoft.SqlServer.Dac.dll" into the directory
                // where "dnx.exe" is located.

                Console.WriteLine(ex.Message);
            }
        }
    }
}
