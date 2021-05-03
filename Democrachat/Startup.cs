using System.Threading.Tasks;
using Democrachat.Auth;
using Democrachat.Chat;
using Democrachat.Db;
using Democrachat.Inventory;
using Democrachat.Kudo;
using Democrachat.Log;
using Democrachat.Power;
using Democrachat.Rewards;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Democrachat
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
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Events.OnRedirectToLogin = context =>
                    {
                        context.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    };
                });
            
            services.AddSingleton<IUserService, UserService>();
            services.AddScoped<ITopicService, TopicService>();
            services.AddScoped<IItemService, ItemService>();
            services.AddScoped<IKudoTableService, KudoTableService>();
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<IKudoService, KudoService>();
            services.AddScoped<MuteService>();
            services.AddScoped<TopicBidService>();
            services.AddScoped<ILogger, Logger>();
            services.AddSingleton<IAuthService, AuthService>();
            services.AddScoped<ITopicNameService, DbTopicNameService>();
            services.AddSingleton<RegisterSpamCheckService>();
            services.AddSingleton<IChatSpamService, ChatSpamService>();
            services.AddSingleton<ActiveUserService>();

            services.AddSingleton<RewardService>();
            services.AddSingleton<PeerService>();

            services.AddSingleton<IUserIdProvider, UserIdProvider>();

            services.AddControllers();
            services.AddSignalR();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Democrachat", Version = "v1"});
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            app.ApplicationServices.GetService<RewardService>(); // Warm up reward service for its timer event
            
            var forwardedHeadersOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            };
            forwardedHeadersOptions.KnownNetworks.Clear();
            forwardedHeadersOptions.KnownProxies.Clear();
            app.UseForwardedHeaders(forwardedHeadersOptions);
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Democrachat v1"));
            }
            app.UseRouting();

            if (env.IsDevelopment())
            {
                app.UseCors(builder =>
                {
                    builder.WithOrigins("http://localhost:1234")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/hub/chat");
            });
        }
    }
}