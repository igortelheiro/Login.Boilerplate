using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Login.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                //.UseSerilog((host, log) =>
                //{
                //    if (host.HostingEnvironment.IsProduction())
                //        log.MinimumLevel.Information();
                //    else
                //        log.MinimumLevel.Debug();

                //    log.MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
                //    log.MinimumLevel.Override("Quartz", LogEventLevel.Information);
                //    log.WriteTo.Console();
                //})
                .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>());
    }
}
