using System;
using System.Threading.Tasks;
using BramrApi.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using BramrApi.Service.Interfaces;
using BramrApi.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BramrApi.Utils;

namespace BramrApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public string IdentityConnectionString { get; set; }
        public string BramrConnectionString { get; set; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
#if DEBUG
            IdentityConnectionString = Configuration.GetConnectionString("LocalIdentityConnection");
            BramrConnectionString = Configuration.GetConnectionString("LocalDatabaseConnection");
#else
            IdentityConnectionString = Configuration.GetConnectionString("LiveIdentityConnection");
            BramrConnectionString = Configuration.GetConnectionString("LiveDatabaseConnection");
#endif
            /// Allow cors
            services.AddCors(options => {
                options.AddPolicy("CorsPolicy",
                builder => builder.AllowAnyOrigin()
                                  .AllowAnyMethod()
                                  .AllowAnyHeader());
            });

            /// Database connection for identity
            services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(IdentityConnectionString,
                mysqlOptions => {
                    mysqlOptions.ServerVersion(new Version(10, 4, 8), ServerType.MariaDb);
                    mysqlOptions.DisableBackslashEscaping();
                }));

            /// Default identity user
            services.AddIdentity<IdentityUser, IdentityRole>(options => {

                /// Paswword must be atleast 8 characters long
                /// must contain a digit, uppercase and 1 special character
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireLowercase = true;

            }).AddEntityFrameworkStores<ApplicationDbContext>()
               .AddDefaultTokenProviders();

            /// JWT setup
            services.AddAuthentication(x => {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x => {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["JwtKey"])),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            /// JWT / identity config
            IdentityConfig.Init(Configuration);

            services.AddControllers(); 

            /// Services
            services.AddScoped<IServerBlockWriterService, ServerBlockWriterService>();
            services.AddScoped<ICommandService, CommandService>();
            services.AddSingleton<IDatabase, DatabaseService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            app.UseCors("CorsPolicy");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            Init(serviceProvider).Wait();
        }

        private async Task Init(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Create roles that are missing 
            foreach (var role in IdentityConfig.ApiRoles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
#if DEBUG
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            if (await userManager.FindByEmailAsync("admin@bramr.tech") == null)
            {
                var result = await userManager.CreateAsync(new IdentityUser
                {
                    Email = "admin@bramr.tech",
                    UserName = "Admin"
                }, "XtS8tT~w");
            }
#endif
        }
    }
}
