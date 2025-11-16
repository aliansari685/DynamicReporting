global using Microsoft.EntityFrameworkCore;
global using System;
global using System.Collections.Generic;
global using System.ComponentModel.DataAnnotations;
global using DynamicReporting.Api.Domain.Models;
global using Swashbuckle.AspNetCore.Annotations;
global using Microsoft.OpenApi.Models;
global using System.ComponentModel;
global using System.ComponentModel.DataAnnotations.Schema;
global using DynamicReporting.Api.Application.DTOs;
global using FluentValidation;
global using DynamicReporting.Api.Application.Validators;
global using Microsoft.AspNetCore.Identity;
global using DynamicReporting.Api.Domain;
global using DynamicReporting.Api.Infrastructure;
global using System.Linq.Expressions;
global using DynamicReporting.Api.Domain.Interfaces;


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

            builder.Services.AddValidatorsFromAssemblyContaining<CustomerDtoValidator>();

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                c.EnableAnnotations();
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}