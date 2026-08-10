namespace DynamicReporting.Mvc;

public class Program
{
    private const string ApiBaseUrl = "https://localhost:7177";

    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        // Configure HttpClient for API communication
        builder.Services.AddHttpClient<IDynamicReportingApiService, DynamicReportingApiService>(client =>
        {
            client.BaseAddress = new Uri(ApiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        SeriLogConfig(builder);

        await ApplicationConfiguration(builder);
    }

    private static async Task ApplicationConfiguration(WebApplicationBuilder builder)
    {
        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();

        app.MapStaticAssets();

        app.MapControllerRoute(
                "default",
                "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

        await CheckApiConnectionAsync(app);

        await app.RunAsync();
    }

    private static async Task CheckApiConnectionAsync(WebApplication app)
    {
        try
        {
            // var delay = TimeSpan.FromSeconds(20);
            //await Task.Delay(delay);
            var client = app.Services
                .GetRequiredService<IHttpClientFactory>()
                .CreateClient();

            client.BaseAddress = new Uri(ApiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(20);

            var response = await client.GetAsync("health");

            response.EnsureSuccessStatusCode();

            Log.Information(
                "ارتباط MVC با DynamicReporting.Api با موفقیت برقرار شد.");
        }
        catch (Exception ex)
        {
            Log.Error(
                ex,
                "خطا در ارتباط MVC با DynamicReporting.Api");

            throw new Exception(
                "خطا در ارتباط با سامانه.",
                ex);
        }
    }

    /// <summary>
    ///     تنظیم کتابخانه سری لاگ
    /// </summary>
    private static void SeriLogConfig(WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration().MinimumLevel.Information().MinimumLevel
            .Override("Microsoft", LogEventLevel.Warning).MinimumLevel
            .Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning).MinimumLevel
            .Override("Hangfire", LogEventLevel.Warning).WriteTo
            .File("Logs/log-.txt", rollingInterval: RollingInterval.Day).WriteTo.Console().CreateLogger();
        builder.Host.UseSerilog();
    }
}