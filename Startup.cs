using AutoMapper;
using FluentValidation.AspNetCore;
using Kanbanboard.Auth;
using Kanbanboard.Data;
using Kanbanboard.Hubs;
using Kanbanboard.Model;
using Kanbanboard.ViewModels;
using Kanbanboard.ViewModels.Patches;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using NSwag.AspNetCore;
using SimplePatch;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Kanbanboard
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
            services.AddDbContext<BoardContext>(options => 
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddTransient<IRepository, Repository>();
            services.AddTransient<AuthorizedRepository>();
            services.AddSingleton<JwtFactory>();

            var jwtIssuerOptions = Configuration.GetSection(nameof(JwtIssuerOptions));
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtIssuerOptions["SigningKey"]));

            services.Configure<JwtIssuerOptions>(config => {
                config.Issuer = jwtIssuerOptions[nameof(JwtIssuerOptions.Issuer)];
                config.Audience = jwtIssuerOptions[nameof(JwtIssuerOptions.Audience)];
                config.ValidFor = jwtIssuerOptions.GetValue<int>(nameof(JwtIssuerOptions.ValidFor));
                config.SigningCredentials = new SigningCredentials(signingKey, 
                    jwtIssuerOptions.GetValue<string>("SigningAlgorithm"), SecurityAlgorithms.HmacSha256);
            });

            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;

                options.Password = new PasswordOptions {
                    RequireDigit = false,
                    RequireLowercase = false,
                    RequireUppercase = false,
                    RequireNonAlphanumeric = false
                };
            }).AddEntityFrameworkStores<BoardContext>().AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => 
            {
                options.ClaimsIssuer = jwtIssuerOptions[nameof(JwtIssuerOptions.Issuer)];

                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidIssuer = jwtIssuerOptions[nameof(JwtIssuerOptions.Issuer)],
                    ValidAudience = jwtIssuerOptions[nameof(JwtIssuerOptions.Audience)],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey
                };

                options.SaveToken = true;

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/board"))
                            context.Token = accessToken;

                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAutoMapper();

            DeltaConfig.Init(config => {
                config.IgnoreLetterCase();

                config.AddEntity<Board>().Property(x => x.Id).Exclude();
                config.AddEntity<CardList>().Property(x => x.Id).Exclude();
                config.AddEntity<Card>().Property(x => x.Id).Exclude();

                config.AddEntity<BoardPatchViewModel>();
                config.AddEntity<CardListPatchViewModel>();
                config.AddEntity<CardPatchViewModel>();
            });

            services.AddSignalR();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddControllersAsServices()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseAuthentication();
            /* app.UseSwaggerUi3(); */
            app.UseSignalR(routes => routes.MapHub<BoardHub>("/hubs/board"));

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
