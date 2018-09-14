using System;
using System.Collections.Generic;
using System.IO;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Eshopworld.Core;
using Eshopworld.DevOps;
using Eshopworld.Web;
using Eshopworld.Telemetry;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using System.Reflection;

namespace WebAPIService
{
    /// <summary>
    /// Startup class for ASP.NET runtime
    /// </summary>
    public class Startup
    {
        private readonly TelemetrySettings _telemetrySettings = new TelemetrySettings();
        private readonly IBigBrother _bb;
        private readonly IConfigurationRoot _configuration;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="env">hosting environment</param>
        public Startup(IHostingEnvironment env)
        {
            _configuration = EswDevOpsSdk.BuildConfiguration(env.ContentRootPath, env.EnvironmentName);
            _configuration.GetSection("Telemetry").Bind(_telemetrySettings);
            _bb = new BigBrother(_telemetrySettings.InstrumentationKey, _telemetrySettings.InternalKey);
        }

        /// <summary>
        /// configure services to be used by the asp.net runtime
        /// </summary>
        /// <param name="services">service collection</param>
        /// <returns>service provider instance (Autofac provider)</returns>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.AddApplicationInsightsTelemetry(_telemetrySettings.InstrumentationKey);
                services.Configure<ServiceConfigurationOptions>(_configuration.GetSection("ServiceConfigurationOptions"));

                var serviceConfigurationOptions = services.BuildServiceProvider()
                    .GetService<IOptions<ServiceConfigurationOptions>>();

                services.AddMvc(options =>
                {
                    var policy = ScopePolicy.Create(serviceConfigurationOptions.Value.RequiredScopes.ToArray());
                    options.Filters.Add(new AuthorizeFilter(policy));
                });

                AddSwagger(services);

                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddIdentityServerAuthentication(x =>
                {
                    x.ApiName = serviceConfigurationOptions.Value.ApiName;
                    x.ApiSecret = serviceConfigurationOptions.Value.ApiSecret;
                    x.Authority = serviceConfigurationOptions.Value.Authority;
                    x.RequireHttpsMetadata = serviceConfigurationOptions.Value.IsHttps;
                    //TODO: this requires Eshopworld.Beatles.Security to be refactored
                    //x.AddJwtBearerEventsTelemetry(bb); 
                });

                var builder = new ContainerBuilder();
                builder.Populate(services);
                builder.RegisterInstance(_bb).As<IBigBrother>().SingleInstance();

                // add additional services or modules into container here

                var container = builder.Build();
                return new AutofacServiceProvider(container);
            }
            catch (Exception e)
            {
                _bb.Publish(e.ToBbEvent());
                throw;
            }
        }

        /// <summary>
        /// Adds swagger to the endpoints of the app only if the document xml file is created and in the correct path
        /// </summary>
        /// <param name="services"></param>
        private void AddSwagger(IServiceCollection services)
        {
            var (shouldEnable, path) = GetSwaggerPath();
            if (!shouldEnable)
            {
                return;
            }

            services.AddSwaggerGen(c =>
            {
                c.IncludeXmlComments(path);
                c.DescribeAllEnumsAsStrings();
                c.SwaggerDoc("v1", new Info { Version = Assembly.GetExecutingAssembly().GetName().Version.ToString(), Title = "WebAPIService" });
                c.CustomSchemaIds(x => x.FullName);
                c.AddSecurityDefinition("Bearer",
                    new ApiKeyScheme
                    {
                        In = "header",
                        Description = "Please insert JWT with Bearer into field",
                        Name = "Authorization",
                        Type = "apiKey"
                    });

                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                        { "Bearer", Array.Empty<string>() }
                });
            });
        }

        /// <summary>
        /// Gets the path to the swagger file
        /// </summary>
        /// <returns></returns>
        private (bool shouldEnable, string path) GetSwaggerPath()
        {
            //Locate the XML file being generated by ASP.NET...
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            return File.Exists(xmlPath) ? (true, xmlPath) : (false, string.Empty);
        }

        /// <summary>
        /// configure asp.net pipeline
        /// </summary>
        /// <param name="app">application builder</param>
        /// <param name="env">environment</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseBigBrotherExceptionHandler();
            app.UseSwagger(o => o.RouteTemplate = "swagger/{documentName}/swagger.json");
            app.UseSwaggerUI(o =>
            {
                o.SwaggerEndpoint("v1/swagger.json", "WebAPIService");
                o.RoutePrefix = "swagger";
            });
            app.UseAuthentication();
            app.UseMvc();

        }
    }
}
