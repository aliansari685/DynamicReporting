namespace DynamicReporting.Api
{
    public class Program
    {
        private const string DbName = "ShopTestDb";
        public static void Main(string[] args)
        {
            try
            {
                ExcelPackage.License.SetNonCommercialPersonal("Ali Ansari");

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

        private static void DiFluentValidationConfiguration(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers(options => options.Filters.Add<GlobalFluentValidationFilter>());
            builder.Services.AddValidatorsFromAssemblyContaining<ReportDefinitionDtoValidator>();
        }

        private static void ApplicationConfiguration(WebApplicationBuilder builder)
        {
            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ShopTestDbContext>();
                db.Database.CanConnect(); // فقط تست اتصال
            }

            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.MapSwagger();
            app.MapGet("", () => Results.Redirect("/swagger"));

            // Configure the HTTP request pipeline.
            //   if (app.Environment.IsDevelopment())
            app.UseHangfireDashboard();//localhost/hangfire
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }

        /// <summary>
        /// پیکربندی سواگر
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
        /// یکربندی دیتابیس
        /// </summary>
        /// <param name="builder"></param>
        private static void DiContextConfiguration(WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString(DbName);

            builder.Services.AddDbContext<ShopTestDbContext>(options =>
                options.UseSqlServer(connectionString));
        }

        /// <summary>
        /// پیکربندی کتبخانه هنگ فایر برای مدیریت جاب ها
        /// </summary>
        /// <param name="builder"></param>
        private static void HangfireConfiguration(WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString(DbName);

            builder.Services.AddHangfire(config => config.UseSqlServerStorage(connectionString
                , new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

            var workerCount = Environment.ProcessorCount * 2;
            builder.Services.AddHangfireServer(options => options.WorkerCount = workerCount);
        }

        /// <summary>
        /// پیکربندی سرویسا
        /// </summary>
        /// <param name="builder"></param>
        private static void DiServicesConfiguration(WebApplicationBuilder builder)
        {
            builder.Services.AddMemoryCache();

            builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<IReportDataService, ReportDataService>();
            builder.Services.AddScoped<IReportDefinitionService, ReportDefinitionService>();
            builder.Services.AddScoped<IReportMetadataService, ReportMetadataService>();
            builder.Services.AddScoped<IBaseTableResolver, EfCoreBaseTableResolver>();
            builder.Services.AddScoped<IReportQueryBuilder, EfReportQueryBuilder>();
            builder.Services.AddScoped<ISqlQueryExecutor, SqlQueryExecutor>();
            builder.Services.AddScoped<IJoinPathResolver, JoinPathResolver>();
            builder.Services.AddScoped<IQueryCacheManager, QueryCacheManager>();
            builder.Services.AddScoped<ISelectJoinBuilder, SelectJoinBuilder>();
            builder.Services.AddScoped<IReportExportService, ReportExportService>();

        }

        /// <summary>
        /// تنظیم کتابخانه سری لاگ
        /// </summary>
        private static void SeriLogConfig(WebApplicationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
                .WriteTo.Console()
                .CreateLogger();

            builder.Host.UseSerilog();
        }
    }
}