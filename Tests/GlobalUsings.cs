global using System.Linq.Expressions;
global using DynamicReporting.Api.Application.DTOs;
global using DynamicReporting.Api.Application.Services;
global using DynamicReporting.Api.Application.Validators;
global using DynamicReporting.Api.Domain.Interfaces;
global using DynamicReporting.Api.Domain.Models;
global using DynamicReporting.Api.Domain.Models.Entities;
global using DynamicReporting.Api.Infrastructure.Persistence.Context;
global using DynamicReporting.Api.Infrastructure.Persistence.Helper;
global using DynamicReporting.Api.Infrastructure.Persistence.Query;
global using DynamicReporting.Api.Infrastructure.Persistence.Repository;
global using FluentAssertions;
global using FluentValidation.TestHelper;
global using Microsoft.Data.Sqlite;
global using Microsoft.EntityFrameworkCore;
global using Moq;

namespace Tests;

public class GlobalUsings;