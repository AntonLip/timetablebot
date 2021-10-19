using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Threading.Tasks;
using timetablebot.Service;
using TimetableBot.Models;
using TimetableBot.Models.Interface;

namespace timetablebot
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<BotSettings>(_configuration.GetSection(nameof(BotSettings)));
            services.Configure<HttpClientSettings>(_configuration.GetSection(nameof(HttpClientSettings)));
            services.AddControllers().AddNewtonsoftJson();            

            services.AddControllers();
            services.AddTransient<IBot, Bot>();
            services.AddTransient<ITimetableService, TimetableService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", context =>
                {
                    context.Response.Redirect("/swagger/");
                    return Task.CompletedTask;
                });
                endpoints.MapControllers();
            });
        }
    }
}
