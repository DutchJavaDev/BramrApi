using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Sentry.Extensibility;

namespace BramrApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // Sentry for error reports
                    webBuilder.UseSentry(options => {

#if DEBUG
                        options.Debug = true;
#else
                        options.Debug = false;
#endif
                        options.ServerName = "Bramr-Server";
                        options.MaxRequestBodySize = RequestSize.Always;
                        options.Dsn = "https://78a1efb174294624b4eeede7c855fc13@o203192.ingest.sentry.io/5544818";
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
