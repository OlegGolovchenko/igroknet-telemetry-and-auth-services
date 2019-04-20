using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using org.igrok_net.infrastructure.data;
using org.igrok_net.infrastructure.domain.Interfaces;

namespace org.igrok_net.telemetry
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING");
            IDataAccess dataConnection;
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                dataConnection = new DataConnection();
            }
            else
            {
                dataConnection = new DataConnection(connectionString);
            }
            var repo = new org.igrok_net.infrastructure.domain.Services.ServiceProvider(dataConnection);
            services.AddSingleton(repo);
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseMvc();
        }
    }
}
