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
            string connnectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<Context>(opt => opt.UseSqlServer(connnectionString));
            services.AddScoped<Context, Context>();

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

            app.UseHttpsRedirection();

            app.UseRouting();

            //CORS config for Front-end url
            // var frontEndUrl = Configuration["FrontEndUrl"];
            // app.UseCors(options => options.WithOrigins(frontEndUrl)
            //                             .AllowAnyMethod()
            //                             .AllowAnyHeader()
            //                             .AllowCredentials());

            app.UseCors(options =>options.WithOrigins("http://localhost:3000").AllowAnyMethod().AllowAnyHeader().AllowCredentials());


            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
