namespace DynamicReporting.Mvc;

public class Program
{
    private const string ApiBaseUrl = "https://localhost:7177";

    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        DiServicesConfiguration(builder);

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


    /// <summary>
    ///     پیکربندی سرویسا
    /// </summary>
    /// <param name="builder"></param>
    private static void DiServicesConfiguration(WebApplicationBuilder builder)
    {
        // Add services to the container.
        builder.Services.AddControllersWithViews();

        // Configure HttpClient for API communication
        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            http.ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri(ApiBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(60);
            });
        });

        builder.Services.AddHttpClient<IReportDataService, ReportDataService>();
        builder.Services.AddHttpClient<IReportDefinitionService, ReportDefinitionService>();
        builder.Services.AddHttpClient<IReportExportService, ReportExportService>();
        builder.Services.AddHttpClient<IReportGeneratedService, ReportGeneratedService>();
        builder.Services.AddHttpClient<IMetadataService, ReportMetadataService>();
    }

    private static async Task CheckApiConnectionAsync(WebApplication app)
    {
        var client = app.Services
            .GetRequiredService<IHttpClientFactory>()
            .CreateClient();

        const int maxAttempts = 60;
        var delay = TimeSpan.FromSeconds(2);

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                Log.Information(
                    "در حال بررسی اتصال به DynamicReporting.Api... تلاش {Attempt}/{MaxAttempts}",
                    attempt,
                    maxAttempts);

                using var response = await client.GetAsync(
                    "health",
                    HttpCompletionOption.ResponseHeadersRead);

                if (response.IsSuccessStatusCode)
                {
                    Log.Information(
                        "ارتباط MVC با DynamicReporting.Api با موفقیت برقرار شد.");

                    return;
                }

                Log.Warning(
                    "API در دسترس است اما HealthCheck با وضعیت {StatusCode} پاسخ داد.",
                    response.StatusCode);
            }
            catch (HttpRequestException)
            {
                Log.Warning(
                    "DynamicReporting.Api هنوز آماده نیست. تلاش {Attempt}/{MaxAttempts}.",
                    attempt,
                    maxAttempts);
            }
            catch (TaskCanceledException)
            {
                Log.Warning(
                    "درخواست HealthCheck تایم‌اوت شد. تلاش {Attempt}/{MaxAttempts}.",
                    attempt,
                    maxAttempts);
            }

            await Task.Delay(delay);
        }

        throw new Exception(
            "DynamicReporting.Api پس از چندین تلاش در دسترس قرار نگرفت.");
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