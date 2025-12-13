namespace DynamicReporting.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            BuilderConfiguration(builder);

            ApplicationConfiguration(builder);
        }

        private static void BuilderConfiguration(WebApplicationBuilder builder)
        {

            DiContextConfiguration(builder);

            builder.Services.AddValidatorsFromAssemblyContaining<CustomerDtoValidator>();
            // builder.Services.AddTransient<IValidator<CustomerDto>, CustomerDtoValidator>();

            DiConfiguration(builder);

            builder.Services.AddControllers();

            DiSwaggerConfiguration(builder);
        }

        private static void ApplicationConfiguration(WebApplicationBuilder builder)
        {
            var app = builder.Build();
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
        private static void DiConfiguration(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IReportDataService, ReportDataService>();
            builder.Services.AddScoped<IReportDefinitionService, ReportDefinitionService>();
            builder.Services.AddScoped<IReportMetadataService, ReportMetadataService>();
            builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}