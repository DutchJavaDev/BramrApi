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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            Init(serviceProvider).Wait();
        }

        private async Task Init(IServiceProvider serviceProvider)
        {
            //serviceProvider.GetRequiredService<IDatabase>().SetConnectionString(BramrConnectionString);

#if DEBUG
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            if (await userManager.FindByEmailAsync("admin@bramr.tech") == null)
            {
                await userManager.CreateAsync(new IdentityUser
                {
                    Email = "admin@bramr.tech",
                    UserName = "Admin"
                }, "XtS8tT~w");
            }
#endif
        }
    }
}
