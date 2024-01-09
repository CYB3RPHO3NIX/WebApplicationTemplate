using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using Serilog;

namespace WebApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

            var configuration = new ConfigurationBuilder()
            .SetBasePath(builder.Environment.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

            var connectionString = configuration.GetConnectionString("Database");
            // Add services to the container.
            Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.MSSqlServer(
                connectionString: connectionString,
                sinkOptions: new MSSqlServerSinkOptions
                {
                    TableName = "Logs",
                    AutoCreateSqlTable = true
                },
                restrictedToMinimumLevel: LogEventLevel.Information)
            .CreateLogger();
            builder.Services.AddSingleton<IConfiguration>(configuration);
            builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
