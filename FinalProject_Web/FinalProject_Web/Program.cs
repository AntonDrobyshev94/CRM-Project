using Microsoft.AspNetCore;

namespace FinalProject_Web
{
    public class Program
    { 
        public static void Main(string[] args)
        {
            var init = BuildWebHost(args);
            using (var scope = init.Services.CreateScope())
            {
                var s = scope.ServiceProvider;
            }
            init.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureLogging(log => log.AddConsole())
                .Build();
    }
}