using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using MyCodeCamp.Data;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace PSCodeCamp
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            _env = env;
            _config = builder.Build();
        }

        IConfigurationRoot _config;
        private IHostingEnvironment _env;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_config);
            services.AddDbContext<CampContext>(ServiceLifetime.Scoped);
            services.AddScoped<ICampRepository, CampRepository>();
            services.AddTransient<CampDbInitializer>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddAutoMapper();

            services.AddCors(cfg => 
            {
                cfg.AddPolicy("PyPolicy", bldr => 
                {
                    bldr.AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithOrigins("http://jedox.com");
                });

                cfg.AddPolicy("AnyGET", bldr =>
                {
                    bldr.AllowAnyHeader()
                    .WithMethods("GET")
                    .AllowAnyOrigin();
                });
            });

            // Add framework services.
            services.AddMvc(opt => 
            {
                if (!_env.IsProduction())
                {
                    opt.SslPort = 44352;
                }
                opt.Filters.Add(new RequireHttpsAttribute());
            })
                .AddJsonOptions(opt =>
                { opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore; });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            CampDbInitializer seeder)
        {
            loggerFactory.AddConsole(_config.GetSection("Logging"));
            loggerFactory.AddDebug();

            //app.UseCors(cfg =>
            //{
            //    cfg.AllowAnyHeader()
            //    .AllowAnyMethod()
            //    .WithOrigins("some host");
            //});

            app.UseMvc(config =>
            {
                //config.MapRoute("MainAPIRoute","api/{controller}/{action}");
            });

            seeder.Seed().Wait();
        }
    }
}
