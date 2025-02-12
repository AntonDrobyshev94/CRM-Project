using Models.AuthModels;
using Models.DataServiceModels;
using FinalProject_Web.Filters;
using Microsoft.AspNetCore.Identity;
using FinalProject_Web.Services;
using FinalProject_Web.Services.Interfaces;
using FinalProject_Web.SignalRChat;
using BusinessLogicService.Interfaces;
using BusinessLogicService.Data;
using FinalProject_Web.Interfaces;
using FinalProject_Web.Data;
using ServicesLibrary.Services.Interfaces;
using ServicesLibrary.Services;
using Microsoft.OpenApi.Models;

namespace FinalProject_Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Метод, который служит для добавления необходимых сервисов 
        /// в контейнер.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSignalR();
            services.AddHttpClient();
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            //});

            services.AddTransient<IApplicationData, ApplicationDataService>();
            services.AddTransient<IAccountData, AccountDataService>();
            services.AddTransient<INavigationData, NavigationDataService>();
            services.AddTransient<IContactData, ContactDataService>();
            services.AddTransient<IImageData, ImageData>();
            services.AddTransient<IProjectData, ProjectDataService>();
            services.AddTransient<IServiceData, ServiceDataService>();
            services.AddTransient<IBlogData, BlogDataService>();
            services.AddTransient<ILinkData, LinkDataService>();
            services.AddTransient<IImageDataService, ImageDataService>();
            services.AddTransient<ICookiesService, CookiesService>();
            services.AddHostedService<StartupTaskService>();
            services.AddScoped<ILayoutViewModelServiceCreate<LayoutViewModel>, LayoutViewModelServiceCreate>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<CheckTokenFilter>();
            services.AddControllersWithViews(options =>
            {
                options.Filters.Add<UnauthorizedExceptionFilter>();
            });
            services.AddIdentity<User, IdentityRole>()
                .AddDefaultTokenProviders().AddRoles<IdentityRole>();
        }

        /// <summary>
        /// Метод настройки конфигурации
        /// </summary>
        /// <param name="app"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseHsts();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            //app.UseSwagger();
            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            //    c.RoutePrefix = string.Empty; 
            //});

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseRouting();
            app.UseAuthentication();
            app.UseResponseCaching();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Web}/{action=Index}/{id?}");
                endpoints.MapHub<ChatHub>("/chatHub");
            });
        }
    }
}
