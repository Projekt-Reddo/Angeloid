using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

//Entity framework package
using Microsoft.EntityFrameworkCore;

//DbContext
using Angeloid.DataContext;

//Error and Exception handle
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Net;

//Local services
using Angeloid.Services;

//Models
using Angeloid.Models;

namespace Angeloid
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
            //CORS config
            services.AddCors();

            //DB config
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<Context>(opt => opt.UseSqlServer(connectionString));
            services.AddScoped<Context, Context>();

            //Add Email Service
            var emailConfig = Configuration.GetSection("Email").Get<EmailConfig>();
            string frontEndUrl = Configuration["FrontEndUrl"];
            services.AddScoped<IEmailService>(sp => new EmailService(emailConfig, frontEndUrl));

            //Add Token Service Singleton
            services.AddSingleton<ITokenService, TokenService>();
            
            //Add Services to Scope
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ILogInOutService, UserService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<IStudioService, StudioService>();
            services.AddScoped<ISeiyuuService, SeiyuuService>();
            services.AddScoped<ISeasonService, SeasonService>();
            services.AddScoped<ICharacterService, CharacterService>();
            services.AddScoped<IAnimeService, AnimeService>();
            services.AddScoped<IHomePageService, AnimeService>();
            services.AddScoped<IThreadService, ThreadService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<ISearchService, SearchService>();

            //JSON config
            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            //Add Cache to api
            services.AddMemoryCache();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Angeloid", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Angeloid v1"));
            }

            //Config for Exception format
            app.UseExceptionHandler(config =>
            {
                config.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    var error = context.Features.Get<IExceptionHandlerFeature>();
                    if (error != null)
                    {
                        var ex = error.Error;
                        await context.Response.WriteAsync(new ErrorDetails()
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = ex.Message
                        }.ToString());
                    }
                });
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            //CORS config for Front-end url
            var frontEndUrl = Configuration["FrontEndUrl"];
            app.UseCors(options => options.WithOrigins(frontEndUrl)
                                        .AllowAnyMethod()
                                        .AllowAnyHeader()
                                        .AllowCredentials());

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
