using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;
using System;
using APIGateway.Config;
using APIGateway.Services;
using APIGateway.Data;
using Microsoft.EntityFrameworkCore;

namespace APIGateway
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IConfiguration OcelotConfiguration { get; }

        public Startup(IConfiguration configuration, IHostEnvironment hostEnvironment)
        {
            Configuration = configuration;

            //ocelot configuration
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(hostEnvironment.ContentRootPath)
                   .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
                   .AddEnvironmentVariables();

            OcelotConfiguration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<JWTConfig>(Configuration.GetSection("JWT"));

            var dbConnection = Configuration.GetConnectionString("DefaultConnection");
            var dbVersion = Configuration.GetValue<string>("ConnectionMysqlMariaDbVersion");
            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(dbConnection,
                    new MariaDbServerVersion(dbVersion)));

            services.AddAuthentication().AddJwtBearer("users_auth_scheme", options =>
            {
                var jwtConfig = new JWTConfig();
                Configuration.GetSection("JWT").Bind(jwtConfig);
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret)),
                    ValidAudience = "usersAudience",
                    ValidIssuer = "usersIssuer",
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddOcelot(OcelotConfiguration);

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "APIGateway", Version = "v1" });
            });

            services.AddScoped<TokenService>();
            services.AddScoped<AccountService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "APIGateway v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(builder => builder.AllowAnyOrigin());

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseOcelot().Wait();
        }
    }
}
