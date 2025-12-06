global using System;
global using System.Collections.Generic;
global using System.ComponentModel.DataAnnotations;
global using System.ComponentModel.DataAnnotations.Schema;
global using System.Data;
global using System.Linq.Expressions;
global using System.Reflection;
global using System.Text.Json;
global using DynamicReporting.Api.Application.DTOs;
global using DynamicReporting.Api.Application.Validators;
global using DynamicReporting.Api.Domain.Interfaces;
global using DynamicReporting.Api.Domain.Models;
global using DynamicReporting.Api.Infrastructure.Persistence;
global using DynamicReporting.Api.Infrastructure.Persistence.Repository;
global using DynamicReporting.Api.Shared.Helper;
global using FluentValidation;
global using Mapster;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Migrations;
global using Microsoft.EntityFrameworkCore.Storage;
global using Microsoft.OpenApi;
global using Serilog;
global using Swashbuckle.AspNetCore.Annotations;
global using DynamicReporting.Api.Application.Services;


namespace DynamicReporting.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("ShopTestDb");

            builder.Services.AddDbContext<ShopTestDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Add services to the container.

            builder.Services.AddValidatorsFromAssemblyContaining<CustomerDtoValidator>();

            // --- Register your Services ---
            builder.Services.AddScoped<IReportDataService, ReportDataService>();
            builder.Services.AddScoped<IReportDefinitionService, ReportDefinitionService>();
            builder.Services.AddScoped<IReportMetadataService, ReportMetadataService>();
            builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // builder.Services.AddTransient<IValidator<CustomerDto>, CustomerDtoValidator>();


            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddSwaggerGen(c =>
            {
                // اعمال سامری های بالا متد ک مینویسیم جهت نمایش
                var xmlPath = Path.Combine(AppContext.BaseDirectory,
                    $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
                c.IncludeXmlComments(xmlPath);

                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                c.EnableAnnotations();
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.MapSwagger();
            }
            builder.Services.AddEndpointsApiExplorer();


            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}