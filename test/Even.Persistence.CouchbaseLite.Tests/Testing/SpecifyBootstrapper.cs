using Serilog;
using Serilog.Core;
using Specify.Configuration;

namespace Even.Persistence.CouchbaseLite.Testing
{
    public class SpecifyBootstrapper : DefaultBootstrapper
    {
        public SpecifyBootstrapper()
        {
            LoggingEnabled = true;
            HtmlReport.ReportType = HtmlReportConfiguration.HtmlReportType.Metro;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole()
                .WriteTo.RollingFile($".log\\{GetType().Assembly.GetName().Name}-{{Date}}.log")
                .CreateLogger();
        }        
    }
}