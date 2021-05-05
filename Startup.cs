using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.IO;
using Webuno.API.Models;
using Webuno.API.Services;

namespace Webuno.API
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
            services.AddControllers().AddNewtonsoftJson();
            services.AddSignalR();

            ConfigureSwagger(services);
            ConfigureCosmosDatabase(services);
            ConfigureDependencyInjection(services);
            services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_CONNECTIONSTRING"]);
        }

        private void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Webuno API",
                    Version = "v1"
                });
            

                var filePath = Path.Combine(System.AppContext.BaseDirectory, "Webuno.API.xml");
                c.IncludeXmlComments(filePath);

            });
        }
        private static void ConfigureDependencyInjection(IServiceCollection services)
        {
            services.AddScoped<IGameRepository, GameRepository>();
            services.AddScoped<ICardsRepository, CardsRepository>();
            services.AddScoped<IGameHub, GameHub>();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors(builder =>
                builder
                    .WithOrigins("http://localhost:3000")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
            );
            app.UseSwagger();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Webuno API");
                c.DisplayOperationId();
                c.DisplayRequestDuration();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<GameHub>("/gamehub");
            });
        }
        private void ConfigureCosmosDatabase(IServiceCollection services)
        {
            services.AddEntityFrameworkCosmos();
            services.AddDbContext<WebunoDbContext>(options =>options.UseCosmos(
                Configuration["CosmosDbAccount"],
                Configuration["CosmosDbKey"],
                Configuration["CosmosDbDatabaseName"]
                ));
        }
    }
}
