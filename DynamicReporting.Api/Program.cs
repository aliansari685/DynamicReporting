namespace DynamicReporting.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //For Test Commit
            var builder = WebApplication.CreateBuilder(args);

            BuilderConfiguration(builder);

            ApplicationConfiguration(builder);
        }

        private static void BuilderConfiguration(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();

            DiFluentValidationConfiguration(builder);
            SeriLogConfig(builder);
            DiContextConfiguration(builder);
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

            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.MapSwagger();
            app.MapGet("", () => Results.Redirect("/swagger"));

            // Configure the HTTP request pipeline.
            //   if (app.Environment.IsDevelopment())

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
            var connectionString = builder.Configuration.GetConnectionString("ShopTestDb");

            builder.Services.AddDbContext<ShopTestDbContext>(options =>
                options.UseSqlServer(connectionString));
        }

        /// <summary>
        /// پیکربندی سرویسا
        /// </summary>
        /// <param name="builder"></param>
        private static void DiServicesConfiguration(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<IReportDataService, ReportDataService>();
            builder.Services.AddScoped<IReportDefinitionService, ReportDefinitionService>();
            builder.Services.AddScoped<IReportMetadataService, ReportMetadataService>();
            builder.Services.AddScoped<IBaseTableResolver, EfCoreBaseTableResolver>();
            builder.Services.AddScoped<IReportQueryBuilder, EfReportQueryBuilder>();
            builder.Services.AddScoped<ISqlQueryExecutor, SqlQueryExecutor>();
        }

        /// <summary>
        /// تنظیم کتابخانه سری لاگ
        /// </summary>
        private static void SeriLogConfig(WebApplicationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Error()
                .WriteTo.File(
                    path: "Logs/log-.txt",
                    rollingInterval: RollingInterval.Day
                )
                .CreateLogger();

            builder.Host.UseSerilog();
        }
    }
}