using Api.Data;
using Api.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RabbitMQLib;
using System.Linq;

namespace Api
{
    public class Startup
    {
        private RabbitMqService _rabbitMqService = new();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Configuration.GetSection("RabbitMQ").Bind(_rabbitMqService.Config);
            _rabbitMqService.StartConnection("items",  "categories");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "api", Version = "v1" });
                c.ResolveConflictingActions(c => c.First());
            });

            services.AddDbContext<ApplicationDbContext>(c => c.UseNpgsql(Configuration.GetConnectionString("Default")));
            services.AddScoped<CategoryService>();
            services.AddScoped<ItemService>();

            services.AddSingleton(_rabbitMqService);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "api v1"));
            }

            app.UseMiddleware<TokenMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
