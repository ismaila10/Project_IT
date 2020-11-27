using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebApplication.Data;
using Microsoft.OpenApi.Models;
using APILibrary.Core.IdentityUserModel;
using APILibrary.Core.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace WebApplication
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
            //ajout de la d?p. EatDbContext. Configuration avec le type de bdd et chaine de connexion
            services.AddDbContext<EatDbContext>(db =>
                    
                    db.UseLoggerFactory(EatDbContext.SqlLogger)
                    .UseSqlServer(Configuration.GetConnectionString("EatConnectionString"))
            );

            services.AddSwaggerGen(c=>
                 c.SwaggerDoc("v1", new OpenApiInfo
                 {
                     Version = "v1",
                     Title = "ToDo API",
                     Description = "A simple example ASP.NET Core Web API",
                     TermsOfService = new Uri("https://example.com/terms"),
                     Contact = new OpenApiContact
                     {
                         Name = "Shayne Boyer",
                         Email = string.Empty,
                         Url = new Uri("https://twitter.com/spboyer"),
                     },
                     License = new OpenApiLicense
                     {
                         Name = "Use under LICX",
                         Url = new Uri("https://example.com/license"),
                     }
                 })
                
                
                );


           // services.AddIdentity<User, Role>().AddEntityFrameworkStores<EatDbContext>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            //ajout du swagger ? notre application
            var swaggerOptions = new SwaggerOptions();

            Configuration.GetSection(nameof(swaggerOptions)).Bind(swaggerOptions);
            app.UseSwagger(options => options.RouteTemplate = swaggerOptions.JsonRoute);
            app.UseSwaggerUI(options => options.SwaggerEndpoint(swaggerOptions.UIEndpoint, swaggerOptions.Description));

            app.UseStaticFiles();
            app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
