using FinalProject_API.AuthFinalProjectApp;
using FinalProject_API.ContextFolder;
using FinalProject_API.Data;
using FinalProject_API.Kafka;
using FinalProject_API.Temporary;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text;

namespace FinalProject_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddAuthentication(opt => {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"]!,
                        ValidAudience = builder.Configuration["Jwt:Audience"]!,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";
                            return context.Response.WriteAsync("{\"error\": \"Unauthorized\"}");
                        }
                    };
                });
            builder.Services.AddAuthorization(options =>
            {
                options.DefaultPolicy =
            new AuthorizationPolicyBuilder
                    (JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();

                options.AddPolicy("AdminOnly", policy =>
                {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Admin");
                });
            });

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSingleton(new ConcurrentDictionary<string, TaskCompletionSource<TokenResponseModel>>());
            builder.Services.AddSingleton<TokenModelHandler>();
            builder.Services.AddScoped<ApplicationData>();
            builder.Services.AddScoped<AccountData>();
            builder.Services.AddScoped<ServiceData>();
            builder.Services.AddScoped<ProjectData>();
            builder.Services.AddScoped<BlogData>();
            builder.Services.AddScoped<ContactData>();
            builder.Services.AddScoped<LinkData>();
            builder.Services.AddScoped<ImageData>();
            builder.Services.AddScoped<MenuNavigationAndTagData>();

            builder.Services.AddControllers();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API обработки заявок пользователей", 
                    Version = "v1", Description= "CRM приложение обработки заявок" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            builder.Services.AddIdentity<User, IdentityRole>()
                    .AddEntityFrameworkStores<DataContext>()
                    .AddDefaultTokenProviders()
                    .AddRoles<IdentityRole>();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("FinalProject_API", builder =>
                {
                    builder.WithOrigins("https://localhost:7010")
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });
            builder.Services.AddSingleton<KafkaProducerService>(sp =>
            {
                return new KafkaProducerService("localhost:9092");
            });
            builder.Services.AddSingleton<KafkaConsumerService>(sp =>
            {
                var tokenHandler = sp.GetRequiredService<TokenModelHandler>();
                return new KafkaConsumerService(
                    "localhost:9092", 
                    "my-group",
                    tokenHandler
                );
            });
            builder.Services.AddTransient<TemporaryDatabase>();

            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CRM API V1");
                c.RoutePrefix = string.Empty;
            });
            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseCors("FinalProject_API");

            app.MapControllers();
            var kafkaConsumerService = app.Services.GetRequiredService<KafkaConsumerService>();
            var cancellationTokenSource = new CancellationTokenSource();

            kafkaConsumerService.StartConsuming("response-topic", cancellationTokenSource.Token);

            app.Run();

            AppDomain.CurrentDomain.ProcessExit += (s, e) => cancellationTokenSource.Cancel();
        }
    }
}