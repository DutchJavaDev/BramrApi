using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BramrApi.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

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

            //services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(identityConnectionString));

            /// Default identity
            services.AddIdentityCore<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
        }
    }
}
