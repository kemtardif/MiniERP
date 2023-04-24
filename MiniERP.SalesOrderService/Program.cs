using Microsoft.AspNetCore.Mvc;
using MiniERP.SalesOrderService.Data;
using Microsoft.Extensions.Logging.Console;
using System.Net.Mime;
using Microsoft.EntityFrameworkCore;
using MiniERP.SalesOrderService.Grpc;
using FluentValidation;
using MiniERP.SalesOrderService.Models;
using MiniERP.SalesOrderService.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MiniERP.SalesOrderService.MessageBus;
using MiniERP.SalesOrderService.Protos;
using System.Reflection;
using MiniERP.SalesOrderService.Behaviors.CreateBehavior;
using MediatR;
using MiniERP.SalesOrderService.Commands;
using MiniERP.SalesOrderService.DTOs;
using MiniERP.SalesOrderService.Behaviors;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(opts =>
{
    opts.ColorBehavior = LoggerColorBehavior.Enabled;
    opts.TimestampFormat = "HH:mm:ss";
});

builder.Configuration
    .AddJsonFile("secrets/salesorder.appsettings.secrets.json", optional: true)
    .AddEnvironmentVariables();


builder.Services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();
builder.Services.AddScoped<IValidator<SalesOrder>, SOValidator>();
builder.Services.AddScoped<IValidator<Inventory>, InventoryValidator>();

builder.Services.AddScoped<IDataClient, GrpcDataClient>();

builder.Services.AddSingleton<IMessageProcessor, RabbitMQProcessor>();
builder.Services.AddHostedService<RabbitMQSubscriber>();



builder.Services.AddDbContext<AppDbContext>(opts =>
{
    opts.UseNpgsql(builder.Configuration.GetConnectionString("salesorderservicePGSQL"));
});


builder.Services.AddGrpcClient<GrpcInventoryService.GrpcInventoryServiceClient>(o =>
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

builder.Services.AddMediatR(config => {
    config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
    config.AddBehavior<IPipelineBehavior<CreateCommand, Result<SOReadDTO>>, CreateValidationBehavior>();
});

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
 