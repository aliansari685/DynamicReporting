namespace DynamicReporting.Api;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            SetLicenseForPackages();
            var builder = WebApplication.CreateBuilder(args);
            BuilderConfiguration(builder);
            ApplicationConfiguration(builder);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "خطای داخلی برنامه:");
            throw;
        }
    }
    /// <summary>
    /// اعمال تنظیمات و لایسنس ها برای پکیجایی ک نیاز به تنظیم لایسنس دارن
    ///  epplus برای ساخت فایل اکسل
    /// کتابخانه QuestPDF برای ساخت فایل پی دی اف
    /// </summary>
    private static void SetLicenseForPackages()
    {
        ExcelPackage.License.SetNonCommercialPersonal("Ali Ansari");
        QuestPDF.Settings.License = LicenseType.Community;
    }

    private static void BuilderConfiguration(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        SeriLogConfig(builder);
        DiFluentValidationConfiguration(builder);
        DiContextConfiguration(builder);
        HangfireConfiguration(builder);
        DiServicesConfiguration(builder);
        DiSwaggerConfiguration(builder);
    }

    /// <summary>
    ///     پیکربندی ولیدیشن های سمت کنترلر
    /// </summary>
    /// <param name="builder"></param>
    private static void DiFluentValidationConfiguration(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers(options => { options.Filters.Add<GlobalFluentValidationFilter>(); });
        builder.Services.AddValidatorsFromAssemblyContaining<ReportDefinitionDtoValidator>();
    }

    /// <summary>
    ///     پیکربندی سواگر
    /// </summary>
    /// <param name="builder"></param>
    private static void DiSwaggerConfiguration(WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(c =>
        {
            var xmlPath = Path.Combine(AppContext.BaseDirectory,
                $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
            c.IncludeXmlComments(xmlPath);
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "وب سرویس گزارش ساز (گزارش پویا)", Version = "v1" });
            c.EnableAnnotations();
        });
        builder.Services.AddEndpointsApiExplorer();
    }

    /// <summary>
    ///     یکربندی دیتابیس
    /// </summary>
    /// <param name="builder"></param>
    private static void DiContextConfiguration(WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("Default");
        builder.Services.AddDbContext<ShopTestDbContext>(options => options.UseSqlServer(connectionString));
    }

    /// <summary>
    ///     پیکربندی کتبخانه هنگ فایر برای مدیریت جاب ها
    /// </summary>
    /// <param name="builder"></param>
    private static void HangfireConfiguration(WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("Default");
        builder.Services.AddHangfire(config =>
            config.UseSqlServerStorage(connectionString, new SqlServerStorageOptions { DisableGlobalLocks = true })
                .WithJobExpirationTimeout(TimeSpan.FromHours(6)));
        var workerCount = Environment.ProcessorCount * 2;
        builder.Services.AddHangfireServer(options => options.WorkerCount = workerCount);
    }

    /// <summary>
    ///     پیکربندی سرویسا
    /// </summary>
    /// <param name="builder"></param>
    private static void DiServicesConfiguration(WebApplicationBuilder builder)
    {
        builder.Services.AddMemoryCache();
        builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IReportDataService, ReportDataService>();
        builder.Services.AddScoped<IReportExportService, ReportExportService>();
        builder.Services.AddScoped<IReportDefinitionService, ReportDefinitionService>();
        builder.Services.AddScoped<IReportMetadataService, ReportMetadataService>();
        builder.Services.AddScoped<IBaseTableResolver, EfCoreBaseTableResolver>();
        builder.Services.AddScoped<IReportQueryBuilder, SqlServerReportQueryBuilder>();
        builder.Services.AddScoped<IJoinPathResolver, JoinPathResolver>();
        builder.Services.AddScoped<ICacheManager, ReportCacheManager>();
        builder.Services.AddScoped<ISelectJoinBuilder, SelectJoinBuilder>();
        builder.Services.AddScoped<IExportBackgroundJobService, ExportBackgroundJobService>();
        builder.Services.AddScoped<IReportGeneratedService, ReportGeneratedService>();
        builder.Services.AddScoped<IJobQueueService, HangfireJobQueueService>();
        builder.Services.AddScoped<IExportJob, ExportJob>();
        builder.Services.AddKeyedScoped<IExportService, ExcelExportService>(ServiceResolver.ExportType.Excel);
        builder.Services.AddKeyedScoped<IExportService, PdfExportService>(ServiceResolver.ExportType.Pdf);
        builder.Services.AddSignalR();
        builder.Services.AddKeyedScoped<ISqlQueryExecutor, SqlQueryExecutor>(ServiceResolver.ExecutorType.AdoNet);
        builder.Services.AddKeyedScoped<ISqlQueryExecutor, SqlQueryExecutor>(ServiceResolver.ExecutorType.Dapper);
        builder.Services.AddScoped<IReportNotificationService, ReportNotificationService>();
        builder.Services.AddScoped<IServiceResolver, ServiceResolver>();
        builder.Services.AddScoped<IFilterOperatorHelper, FilterOperatorHelper>();
        builder.Services.AddScoped<IReportValidation, ReportValidation>();
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

    private static void ApplicationConfiguration(WebApplicationBuilder builder)
    {
        var app = builder.Build();
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ShopTestDbContext>();
            db.Database.CanConnect(); // فقط تست اتصال
        }

        //todo : 
        if (app.Environment.IsDevelopment()) app.UseExceptionHandler("/Error");
        if (app.Environment.IsProduction()) app.UseMiddleware<GlobalExceptionMiddleware>();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.MapSwagger();
        app.MapGet("", () => Results.Redirect("/swagger"));

        // Configure the HTTP request pipeline.
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
        app.UseHangfireDashboard(); //localhost/hangfire
        app.Lifetime.ApplicationStarted.Register(() =>
        {
            BackgroundJob.Enqueue<CronJobs>(jobs => jobs.CleanupFailedJobsAsync());
            BackgroundJob.Enqueue<CronJobs>(job => job.CleanupExpiredReportsJobAsync());
        });
        RecurringJob.AddOrUpdate<CronJobs>("cleanup-expired-reports", job => job.CleanupExpiredReportsJobAsync(),
            Cron.Hourly);
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.MapHub<ReportHub>("/report-hub");
        app.Run();
    }
}