using Common.Infrastructure.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Ms1.Api.Services;
using Ms1.Api.Store;
using Ms1.Api.Store.Services;
using System;
using System.Reflection;

namespace Ms1.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDbContext<InsideTestDbContext>(o =>
            {
                var connectionString = Configuration.GetConnectionString(typeof(InsideTestDbContext).Name);
                o.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), (options) => { });

            });

            services.AddScoped(typeof(IDbService), typeof(DbService));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ms1.Api", Version = "v1" });
            });

            services.AddHttpClient<IWsService, WsService>("WsApi",
                client =>
                {
                    client.BaseAddress = new Uri(Configuration.GetValue<string>("WsServiceUrl"));
                });

            JaegerUtils.ConfigureService(services, Assembly.GetEntryAssembly().GetName().Name);

        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ms1.Api v1"));
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
