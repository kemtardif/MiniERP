using Microsoft.AspNetCore.Mvc;
using MiniERP.SalesOrderService.Data;
using Microsoft.Extensions.Logging.Console;
using System.Net.Mime;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using MiniERP.SalesOrderService.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MiniERP.SalesOrderService.Protos;
using System.Reflection;
using MiniERP.SalesOrderService.Behaviors.CreateBehavior;
using MediatR;
using MiniERP.SalesOrderService.Commands;
using MiniERP.SalesOrderService.DTOs;
using MiniERP.SalesOrderService.Behaviors;
using MiniERP.SalesOrderService.MessageBus.Sender.Contracts;
using MiniERP.SalesOrderService.MessageBus.Sender;
using StackExchange.Redis;
using StackExchange.Redis.KeyspaceIsolation;
using Polly;
using Polly.Contrib.WaitAndRetry;
using MiniERP.SalesOrderService.Extensions;
using Grpc.Core;
using MiniERP.SalesOrderService.Models;
using MiniERP.SalesOrderService.Grpc;
using MiniERP.SalesOrderService.Caching;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

RegisterLogging();

ConfigureSecrets();

AddControllers();

AddAuthentication();

AddDependencies();

AddRetryPolicies();

AddServices();

AddRabbitMQ();

AddDBCOntext();

AddRedis();

AddGRPC();

/////////////////////////////////////////////////////////////////////////////////

var app = builder.Build();

app.UseExceptionHandler(new ExceptionHandlerOptions()
{
    AllowStatusCode404Response = true,
    ExceptionHandlingPath = "/error"
});

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

Migration.ApplyMigration(app);

app.Run();
////////////////////////////////////////////////////////////////////////////////

void RegisterLogging()
{
    builder.Logging.ClearProviders();
    builder.Logging.AddSimpleConsole(opts =>
    {
        opts.ColorBehavior = LoggerColorBehavior.Enabled;
        opts.TimestampFormat = "HH:mm:ss";
    });
}

void ConfigureSecrets()
{
    builder.Configuration
    .AddJsonFile("secrets/salesorder.appsettings.secrets.json", optional: true)
    .AddEnvironmentVariables();
}


void AddControllers()
{
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
}

void AddAuthentication()
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opts =>
                {
                    opts.Audience = builder.Configuration["AAD:ApplicationId"];
                    opts.Authority = string.Format("{0}{1}", builder.Configuration["AAD:Tenant"],
                                                             builder.Configuration["AAD:TenantId"]);

                });
}

void AddDependencies()
{
    builder.Services.AddAutoMapper(typeof(Program));
    builder.Services.AddMediatR(config => {
        config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        config.AddOpenBehavior(typeof(LoggingBehavior<,>));
        config.AddOpenBehavior(typeof(MessagingBehavior<,>));
        config.AddBehavior<IPipelineBehavior<CreateCommand, Result<SOReadDTO>>, ValidationBehavior>();
    });
}

void AddRetryPolicies()
{
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
}

void AddServices()
{
    builder.Services.AddScoped<IRepository, SORepository>();
    builder.Services.AddScoped<IValidator<SOCreateDTO>, BaseValidator>();
    builder.Services.AddScoped<IValidator<SOCreateDTO>, InventoryValidator>();
    builder.Services.AddScoped<IRPCService, GRPCService>();
    builder.Services.AddScoped<ICacheService, RedisCacheService>();
}

void AddRabbitMQ()
{
    builder.Services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>();
    builder.Services.AddScoped<IRabbitMQClient, RabbitMQClient>();
}

void AddDBCOntext()
{
    builder.Services.AddDbContext<AppDbContext>(opts =>
    {
        opts.UseNpgsql(builder.Configuration.GetConnectionString("salesorderservicePGSQL"));
    });

}

void AddRedis()
{
    builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
    {
        ConfigurationOptions option = new ConfigurationOptions
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
}

void AddGRPC()
{
    builder.Services.AddGrpcClient<GrpcInventoryService.GrpcInventoryServiceClient>(o =>
    {
        o.Address = new Uri(builder.Configuration["GrpcInventoryService"]!);
    });

}

