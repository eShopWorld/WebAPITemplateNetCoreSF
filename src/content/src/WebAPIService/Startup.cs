using System;
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
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.OpenApi.Models;

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
        private bool UseOpenApiV2 => true;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="env">hosting environment</param>
        public Startup(IWebHostEnvironment env)
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
                    options.EnableEndpointRouting = false;
                    var policy = ScopePolicy.Create(serviceConfigurationOptions.Value.RequiredScopes.ToArray());

                    var filter = EnvironmentHelper.IsInFabric ? 
                        (IFilterMetadata) new AuthorizeFilter(policy): 
                        new AllowAnonymousFilter();

                    options.Filters.Add(filter);
                });

                services.AddApiVersioning();
                services.AddHealthChecks();

                //Get XML documentation
                var path = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");

                //if not generated throw an event but it's not going to stop the app from starting
                if (!File.Exists(path))
                {
                    BigBrother.Write(new Exception("Swagger XML document has not been included in the project"));
                }
                else
                {
                    services.AddSwaggerGen(c =>
                    {
                        c.IncludeXmlComments(path);
                        c.DescribeAllEnumsAsStrings();
                        c.SwaggerDoc("v1", new OpenApiInfo { Version = Assembly.GetExecutingAssembly().GetName().Version.ToString(), Title = "WebAPIService" });
                        c.CustomSchemaIds(x => x.FullName);
                        c.AddSecurityDefinition("Bearer",
                            new OpenApiSecurityScheme
                            {
                                In = ParameterLocation.Header,
                                Description = "Please insert JWT with Bearer into field",
                                Name = "Authorization",
                                Type = UseOpenApiV2 ? SecuritySchemeType.ApiKey : SecuritySchemeType.Http,
                                Scheme = "bearer",
                                BearerFormat = "JWT",
                            });

                        c.AddSecurityRequirement(new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                                },
                                new string[0]
                            }
                        });
                    });
                }

                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddIdentityServerAuthentication(
                    x =>
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
                _bb.Publish(e.ToExceptionEvent());
                throw;
            }
        }

        /// <summary>
        /// configure asp.net pipeline
        /// </summary>
        /// <param name="app">application builder</param>
        /// <param name="env">environment</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseBigBrotherExceptionHandler();
            app.UseSwagger(o =>
            {
                o.RouteTemplate = "swagger/{documentName}/swagger.json";
                o.SerializeAsV2 = UseOpenApiV2;
            }); 
            app.UseSwaggerUI(o =>
            {
                o.SwaggerEndpoint("v1/swagger.json", "WebAPIService");
                o.RoutePrefix = "swagger";
            });

            app.UseAuthentication();

            app.UseHealthChecks("/probe");

            app.UseMvc();
        }
    }
}
