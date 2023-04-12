using Microsoft.AspNetCore.Mvc;
using MiniERP.SalesOrderService.Data;
using Microsoft.Extensions.Logging.Console;
using System.Net.Mime;
using MiniERP.SalesOrderService.Services;
using Microsoft.EntityFrameworkCore;
using MiniERP.SalesOrderService.Protos;
using MiniERP.SalesOrderService.Grpc;
using MiniERP.SalesOrderService.Caching;
using FluentValidation;
using MiniERP.SalesOrderService.Models;
using MiniERP.SalesOrderService.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MiniERP.SalesOrderService.MessageBus;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(opts =>
{
    opts.ColorBehavior = LoggerColorBehavior.Enabled;
    opts.TimestampFormat = "HH:mm:ss";
});

builder.Configuration
    .AddJsonFile("secrets/salesorder.appsettings.secrets.json", optional: false)
    .AddEnvironmentVariables();


builder.Services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();
builder.Services.AddScoped<ISalesOrderService, SalesOrderService>();
builder.Services.Decorate<ISalesOrderService, SalesOrderServiceWithInventory>();
builder.Services.Decorate<ISalesOrderService, SalesOrderServiceWithLogging>();
builder.Services.AddScoped<IGrpcClientAdapter, GrpcClientAdapter>();
builder.Services.AddScoped<IInventoryDataClient, InventoryDataClient>();
builder.Services.AddSingleton<IMessageProcessor, RabbitMQProcessor>();
builder.Services.AddHostedService<RabbitMQSubscriber>();
builder.Services.AddScoped<ICacheRepository, RedisCacheRepository>();
builder.Services.AddScoped<IValidator<SalesOrder>, SalesOrderValidator>();


builder.Services.AddDbContext<AppDbContext>(opts =>
{
    opts.UseNpgsql(builder.Configuration.GetConnectionString("salesorderservicePGSQL"));
});

builder.Services.AddDistributedRedisCache(opts =>
{
    opts.InstanceName = "sosrv_";
    opts.Configuration = builder.Configuration.GetConnectionString("salesorderserviceRedis");
});

builder.Services.AddGrpcClient<GrpcInventory.GrpcInventoryClient>(o =>
{
    o.Address = new Uri(builder.Configuration["GrpcInventoryService"]!);
});

builder.Services.AddControllers()
.ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var result = new UnprocessableEntityObjectResult(context.ModelState);

        result.ContentTypes.Add(MediaTypeNames.Application.Json);
        result.ContentTypes.Add(MediaTypeNames.Application.Xml);

        return result;
    };
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opts =>
                {
                    opts.Audience = builder.Configuration["AAD:ApplicationId"];
                    opts.Authority = string.Format("{0}{1}", builder.Configuration["AAD:Tenant"],
                                                             builder.Configuration["AAD:TenantId"]);

                });

builder.Services.AddAutoMapper(typeof(Program));

// Configure the HTTP request pipeline.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseExceptionHandler(new ExceptionHandlerOptions()
{
    AllowStatusCode404Response = true,
    ExceptionHandlingPath = "/error"
});


// Handled via nginx ingress
//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

Migration.ApplyMigration(app);

app.Run();
 