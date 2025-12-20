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
global using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
global using Microsoft.EntityFrameworkCore.Metadata;
global using System.Net;
global using DynamicReporting.Api.Shared;
global using System.Runtime.CompilerServices;
global using System.Collections.Concurrent;
global using System.Text;
global using DynamicReporting.Api.Infrastructure.Persistence.DbContext;
global using DynamicReporting.Api.Infrastructure.Persistence.Helper;
global using DynamicReporting.Api.Infrastructure.Persistence.Query;
global using DynamicReporting.Api.Domain.Models.Entities;



namespace DynamicReporting.Api.Shared;

public class GlobalUsings;