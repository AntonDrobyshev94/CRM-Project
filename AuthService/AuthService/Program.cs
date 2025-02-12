
using AuthService.AuthModels;
using AuthService.ContextFolder;
using AuthService.Data;
using AuthService.Kafka;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AuthService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<DataContext>(options =>options.UseSqlServer
            (builder.Configuration.GetConnectionString("DefaultConnection")));
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
            builder.Services.AddScoped<AccountData>();
            builder.Services.AddTransient<AuthHandler>();
            builder.Services.AddTransient<RegistrationHandler>();
            builder.Services.AddTransient<IMessageHandlerFactory, MessageHandlerFactory>();
            builder.Services.AddControllers();
            builder.Services.AddIdentity<User, IdentityRole>()
                    .AddEntityFrameworkStores<DataContext>()
                    .AddDefaultTokenProviders()
                    .AddRoles<IdentityRole>();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<KafkaProducerService>(sp =>
            {
                return new KafkaProducerService("localhost:9092");
            });
        
            builder.Services.AddSingleton<KafkaConsumerService>(sp =>
            {
                var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();

                return new KafkaConsumerService("localhost:9092", "auth-group", scopeFactory);
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseRouting();


            app.MapControllers();
            var kafkaConsumerService = app.Services.GetRequiredService<KafkaConsumerService>();
            var cancellationTokenSource = new CancellationTokenSource();

            kafkaConsumerService.StartConsumingAsync("auth-top", cancellationTokenSource.Token);

            app.Run();

            AppDomain.CurrentDomain.ProcessExit += (s, e) => cancellationTokenSource.Cancel();
        }
    }
}
