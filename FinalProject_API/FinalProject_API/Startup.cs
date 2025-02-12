//using FinalProject_API.AuthFinalProjectApp;
//using FinalProject_API.ContextFolder;
//using FinalProject_API.Data;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using System.Text;

//namespace FinalProject_API
//{
//    public class Startup
//    {
//        public IConfiguration Configuration { get; }
//        public Startup(IConfiguration configuration)
//        {
//            Configuration = configuration;
//        }

//        public void ConfigureServices(IServiceCollection services)
//        {
//            services.AddDbContext<DataContext>(options => options.UseSqlServer(
//                Configuration.GetConnectionString("DefaultConnection")));

//            services.AddAuthentication(opt => {
//                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//                opt.DefaultSignInScheme = IdentityConstants.ExternalScheme;
//            })
//                .AddJwtBearer(options =>
//                {
//                    options.TokenValidationParameters = new TokenValidationParameters
//                    {
//                        ValidateIssuer = false,
//                        ValidateAudience = false,
//                        ValidateLifetime = true,
//                        ValidateIssuerSigningKey = true,
//                        ValidIssuer = Configuration["Jwt:Issuer"]!,
//                        ValidAudience = Configuration["Jwt:Audience"]!,
//                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Secret"]!))
//                    };
//                    options.Events = new JwtBearerEvents
//                    {
//                        OnChallenge = context =>
//                        { 
//                            context.HandleResponse();
//                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
//                            context.Response.ContentType = "application/json";
//                            return context.Response.WriteAsync("{\"error\": \"Unauthorized\"}");
//                        }
//                    };
//                });

//            services.AddAuthorization(options =>
//            {
//                options.DefaultPolicy =
//            new AuthorizationPolicyBuilder
//                    (JwtBearerDefaults.AuthenticationScheme)
//                .RequireAuthenticatedUser()
//                .Build();

//                options.AddPolicy("AdminOnly", policy =>
//                {
//                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
//                    policy.RequireAuthenticatedUser();
//                    policy.RequireRole("Admin");
//                });
//            });

//            services.AddHttpContextAccessor();
//            services.AddScoped<ApplicationData>();
//            services.AddScoped<AccountData>();
//            services.AddScoped<ServiceData>();
//            services.AddScoped<ProjectData>();
//            services.AddScoped<BlogData>();
//            services.AddScoped<ContactData>();
//            services.AddScoped<LinkData>();
//            services.AddScoped<ImageData>();
//            services.AddScoped<MenuNavigationAndTagData>();
//            services.AddDbContext<DataContext>();

//            services.AddControllers();

//            services.AddIdentity<User, IdentityRole>()
//                    .AddEntityFrameworkStores<DataContext>()
//                    .AddDefaultTokenProviders()
//                    .AddRoles<IdentityRole>();
//            services.AddCors(options =>
//            {
//                options.AddPolicy("FinalProject_API", builder =>
//                {
//                    builder.WithOrigins("https://localhost:7010")
//                           .AllowAnyHeader()
//                           .AllowAnyMethod();
//                });
//            });
//        }

//        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
//        {
//            if (env.IsDevelopment())
//            {
//                app.UseDeveloperExceptionPage();
//            }
//            else
//            {
//                app.UseHsts();
//            }

//            app.UseRouting();

//            app.UseAuthentication();

//            app.UseAuthorization();

//            app.UseCors("FinalProject_API");

//            app.UseEndpoints(endpoints =>
//            {
//                endpoints.MapControllers();
//            });
//        }
//    }
//}
