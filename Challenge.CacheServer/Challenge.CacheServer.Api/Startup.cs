using Challenge.CacheServer.Api.Middlewares;
using Challenge.CacheServer.Api.Models;
using Challenge.CacheServer.Core.Interfaces;
using Challenge.CacheServer.Infrastructure.Database;
using Challenge.CacheServer.Infrastructure.Repositories;
using Challenge.CacheServer.Infrastructure.Services;
using Challenge.CacheServer.Infrastructure.Services.Implementations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Challenge.CacheServer.Api
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

            services.AddDbContext<CacheServerDbContext>( options => 
            options.UseSqlServer( Configuration.GetConnectionString("DefaultDatabase")));

            services.AddResponseCaching();

            services.AddScoped<ICacheRecordRepository, CacheRecordRepository>();
            services.AddScoped<IExchangeRateRepository, ExchangeRateRepository>();
            services.AddScoped<IExchangeRateService, ExchangeRateCacheService>();
            services.AddScoped<IEcbClient, EcbApiClientService>();

            services.AddSingleton<ISdmxXmlReader,SdmxXmlReader>();

            services.AddSingleton<ApiKeyStore>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // turned off for performance
            //app.UseMiddleware<LoggingMiddleware>();

            app.UseResponseCaching();

            app.UseMiddleware<CacheControlMiddleware>();

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
