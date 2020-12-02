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
using APILibrary.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Reflection;
using System.IO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using APILibrary.Core.IdentityUserModel;
using APILibrary.Options;

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

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Groupe ProjetIT API",
                    Description = "A simple CRUD (Create, Read, Update, Delete) ASP.NET Core Web API",
                    TermsOfService = new Uri("https://example.com/terms"),
                    });

                // Pour activer l'autorisation swagger sur(JWT)
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    BearerFormat = "JWT",
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}

                    }
                });


                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    c.IncludeXmlComments(xmlPath);
            }
                
                );


            /*Ajouter les services n?cessaire pour g?rer nos Utilisateurs*/
            services.AddIdentity<User, Role>().AddEntityFrameworkStores<EatDbContext>();
            /*Configuration des jetons JWT pour prot?ger nos Apis*/
            /*Installer Microsoft.AspNetCoreAuthentification*/
           
            services.AddAuthentication(options=>{ 
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            })
              .AddJwtBearer("JwtBearer", options =>
              {
                  //options.Authority = "https://localhost:5001";

                  options.RequireHttpsMetadata = false;
                  options.SaveToken = true;
                  //options.Audience = "testapi";
                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateAudience = false,
                      ValidateIssuer = false,
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("ICImachainesecreteTreslongue2020")),
                      ValidateIssuerSigningKey = true,
                      ValidateLifetime = true,
                      ClockSkew = TimeSpan.FromMinutes(5)
                  };

              });



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
            /*app.UseSwagger(options => options.RouteTemplate = swaggerOptions.JsonRoute);
            app.UseSwaggerUI(options => options.SwaggerEndpoint(swaggerOptions.UIEndpoint, swaggerOptions.Description));*/

            app.UseStaticFiles();
            ///app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseRouting();
            
            //global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

            });
        }
    }
}
