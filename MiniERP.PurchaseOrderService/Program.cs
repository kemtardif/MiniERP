using FluentValidation;
using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;
using MiniERP.PurchaseOrderService.Behaviors;
using MiniERP.PurchaseOrderService.Behaviors.CreateBehavior;
using MiniERP.PurchaseOrderService.Commands;
using MiniERP.PurchaseOrderService.Data;
using MiniERP.PurchaseOrderService.DTOs;
using MiniERP.PurchaseOrderService.Grpc;
using MiniERP.PurchaseOrderService.Grpc.Protos;
using MiniERP.PurchaseOrderService.MessageBus.Sender;
using MiniERP.PurchaseOrderService.MessageBus.Sender.Contracts;
using MiniERP.PurchaseOrderService.Models;
using MiniERP.PurchaseOrderService.Validators;
using MiniERP.PurchaseOrderService.Extensions;
using Polly.Contrib.WaitAndRetry;
using Polly;
using System.Net.Mime;
using System.Reflection;
using StackExchange.Redis;
using StackExchange.Redis.KeyspaceIsolation;
using MiniERP.PurchaseOrderService.Caching;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(opts =>
{
    opts.ColorBehavior = LoggerColorBehavior.Enabled;
    opts.TimestampFormat = "HH:mm:ss";
});

builder.Configuration
    .AddJsonFile("secrets/purchaseorder.appsettings.secrets.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.AddScoped<IRepository, PORepository>();
builder.Services.AddScoped<ICacheService, RedisCacheService>();
builder.Services.AddScoped<IRPCService, GRPCService>();

builder.Services.AddScoped<IValidator<POCreateDTO>, BaseValidator>();
builder.Services.AddScoped<IValidator<POCreateDTO>, InventoryValidator>();

builder.Services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>();
builder.Services.AddScoped<IRabbitMQClient, RabbitMQClient>();

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddMediatR(config => {
    config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
    config.AddOpenBehavior(typeof(MessagingBehavior<,>));
    config.AddBehavior<IPipelineBehavior<CreateCommand, Result<POReadDTO>>, CreateValidationBehavior>();
});

builder.Services.AddDbContext<AppDbContext>(opts =>
{
    opts.UseNpgsql(builder.Configuration.GetConnectionString("purchaseorderservicePGSQL"));
});


builder.Services.AddGrpcClient<GrpcInventoryService.GrpcInventoryServiceClient>(o =>
{
    o.Address = new Uri(builder.Configuration["GrpcInventoryService"]!);
});

//Redis Database
builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
{
    ConfigurationOptions option = new()
    {
        AbortOnConnectFail = false,
        ConnectTimeout = 30000,
        ResponseTimeout = 30000,
        Password = builder.Configuration["redisPassword"],
        EndPoints = { builder.Configuration["redisHost"] }

    };
    return ConnectionMultiplexer.Connect(option);
});
builder.Services.AddScoped(provider =>
    provider.GetRequiredService<IConnectionMultiplexer>().GetDatabase().WithKeyPrefix("invsrv:"));

//Retry policies
builder.Services.AddScoped<ISyncPolicy<InventoryItemCache?>>(provider =>
{
    return Policy<InventoryItemCache?>
        .Handle<RedisTimeoutException>()
        .WaitAndRetry(
            Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 3),
            onRetry: (outcome, timespan, retryAttempt, context) =>
            {
                context.GetLogger()?
                    .LogWarning("Delaying Cache call for {delay}ms, then making retry {retry}.", timespan.TotalMilliseconds, retryAttempt);
            });
});

builder.Services.AddScoped<ISyncPolicy<StockResponse>>(provider =>
{
    return Policy<StockResponse>
        .Handle<RpcException>()
        .WaitAndRetry(
            Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 3),
            onRetry: (outcome, timespan, retryAttempt, context) =>
            {
                context.GetLogger()?
                    .LogWarning("Delaying GRPC call for {delay}ms, then making retry {retry}.", timespan.TotalMilliseconds, retryAttempt);
            });
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


var app = builder.Build();

app.UseExceptionHandler(new ExceptionHandlerOptions()
{
    AllowStatusCode404Response = true,
    ExceptionHandlingPath = "/error"
});


//Handled by NGINX ingress
//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

Migration.ApplyMigration(app);

app.Run();

