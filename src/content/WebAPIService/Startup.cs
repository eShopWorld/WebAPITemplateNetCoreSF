using System;
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

namespace WebAPIService
{
    public class Startup
    {
        private readonly TelemetrySettings _telemetrySettings = new TelemetrySettings();
        private readonly IBigBrother _bb;
        private readonly IConfigurationRoot _configuration;

        private const string CorsPolicyName = "CorsPolicy";

        public Startup(IHostingEnvironment env)
        {
            _configuration = EswDevOpsSdk.BuildConfiguration(env.ContentRootPath, env.EnvironmentName);
            _configuration.GetSection("Telemetry").Bind(_telemetrySettings);
            _bb = new BigBrother(_telemetrySettings.InstrumentationKey, _telemetrySettings.InternalKey);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
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

                services.AddSwaggerGen(c =>
                {
                    c.IncludeXmlComments("WebAPIService.xml");
                    c.DescribeAllEnumsAsStrings();
                    c.SwaggerDoc("v1", new Info { Version = "1.0.0", Title = "WebAPIService" });
                    c.CustomSchemaIds(x => x.FullName);
                    c.AddSecurityDefinition("Bearer",
                        new ApiKeyScheme
                        {
                            In = "header",
                            Description = "Please insert JWT with Bearer into field",
                            Name = "Authorization",
                            Type = "apiKey"
                        });
                });

                services.AddCors(options =>
                {
                    options.AddPolicy(CorsPolicyName,
                        policyConfigure => policyConfigure.WithOrigins(serviceConfigurationOptions.Value.Origins.ToArray())
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials());
                });

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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseBigBrotherExceptionHandler();
            app.UseSwagger();
            app.UseSwaggerUI(o => o.SwaggerEndpoint("v1/swagger.json", "WebAPIService"));
            //app.UseCors(CorsPolicyName);
            app.UseAuthentication();
            app.UseMvc();
                        
        }
    }
}
