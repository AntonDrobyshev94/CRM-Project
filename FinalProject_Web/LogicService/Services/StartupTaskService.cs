using FinalProject_Web.Interfaces;
using FinalProject_Web.Vars;

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

                Variables.TitleModelVars = await navigationData.GetTitle();
                Variables.Contacts = await contactData.GetContacts();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
