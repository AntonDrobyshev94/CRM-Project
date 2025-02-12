using ServicesLibrary.Vars;
using BusinessLogicService.Interfaces;

namespace FinalProject_Web.Services
{
    public class StartupTaskService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        public StartupTaskService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var navigationData = scope.ServiceProvider.GetRequiredService<INavigationData>();
                var contactData = scope.ServiceProvider.GetRequiredService<IContactData>();

                Variables.TitleModelVars = await navigationData.GetTitle("https://localhost:7037/api/navigation");
                Variables.Contacts = await contactData.GetContacts("https://localhost:7037/api/contact");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
