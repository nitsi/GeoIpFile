using System;
using System.Linq;
using System.Reflection;
using Kubernetes1.Configuration;
using Kubernetes1.GeoProviders;
using Kubernetes1.GeoProviders.JsonGeoProvider;
using Kubernetes1.Throttling;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Kubernetes1
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
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Geo location service", Version = "v1" });
            });

            BindConfiguration(services);
            ConfigureDependencyInjection(services);
        }

        private void BindConfiguration(IServiceCollection services)
        {
            foreach (Type type in Assembly
                .GetAssembly(typeof(ConfigurationBase))
                .GetTypes()
                .Where(configType =>
                        configType.IsClass && 
                        !configType.IsAbstract && 
                        configType.IsSubclassOf(typeof(ConfigurationBase))))
            {
                var configurationSection = Configuration.GetSection(type.Name);
                var configuration = configurationSection.Get(type);
                services.AddSingleton(type, provider => configuration);
            }
        }

        private void ConfigureDependencyInjection(IServiceCollection services)
        {
            services.AddSingleton<GeoProvidersManagement>();
            services.AddSingleton<JsonGeoProvider>();
            services.AddSingleton(provider => new IGeoProvider[]
            {
                provider.GetService<JsonGeoProvider>()
            });

            services.AddSingleton<JsonGeoDataProvider>();
            services.AddScoped<GeoHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Kubernetes1 v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseMiddleware<ThrottlingMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
