using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BugTracker
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            
            if (env.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);
            //Add DB Context
            var connectionStringBuilder = new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder { DataSource = "mcp.db" };
            var connectionString = connectionStringBuilder.ToString();

            
            
            services.AddDbContext<McpDbContext>(options =>
                options.UseSqlite(connectionString));

            services.AddMvc();
            services.AddSignalR(options => { options.Hubs.EnableDetailedErrors = true; });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseSignalR();

            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                //Adding Seeder Data
               // AddTestData(app.ApplicationServices.GetService<McpDbContext>());
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private static void AddTestData(McpDbContext context)
        {
            var testUser1 = new DbModels.Try.User
            {
                Id = "abc123",
                FirstName = "Luke",
                LastName = "Skywalker"
            };

            context.Users.Add(testUser1);

            var testPost1 = new DbModels.Try.Post
            {
                Id = "def234",
                UserId = testUser1.Id,
                Content = "What a piece of junk!"
            };

            context.Posts.Add(testPost1);

            context.SaveChanges();
        }
    }
}
