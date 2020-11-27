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

namespace BramrApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
#if DEBUG
            var identityConnectionString = Configuration.GetConnectionString("LocalConnection");
#else
            var identityConnectionString = Configuration.GetConnectionString("LiveConnection");
#endif
            /// Database connection for identity
            services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(identityConnectionString,
                mysqlOptions => {
                    mysqlOptions.ServerVersion(new Version(10, 4, 8), ServerType.MariaDb);
                    mysqlOptions.DisableBackslashEscaping();
                }));

            /// Default identity user
            services.AddIdentityCore<IdentityUser>(options => {

                /// Paswword must be atleast 8 characters long
                /// must contain a digit, uppercase and 1 special character
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireLowercase = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddControllers(); 

            /// Services
            services.AddSingleton<IServerBlockWriterService, ServerBlockWriterService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

#if DEBUG
            CreateLocalAccounts(serviceProvider).Wait();
#endif
        }

        private async Task CreateLocalAccounts(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            if (await userManager.FindByEmailAsync("admin@bramr.tech") == null)
            {
                await userManager.CreateAsync(new IdentityUser {
                    Email = "admin@bramr.tech",
                    UserName = "Admin"
                }, "XtS8tT~w");
            }
        }
    }
}
