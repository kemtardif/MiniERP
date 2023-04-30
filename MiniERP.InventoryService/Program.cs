using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.Grpc;
using MiniERP.InventoryService.MessageBus.Subscriber;
using System.Net.Mime;
using System.Reflection;
using MiniERP.InventoryService.MessageBus.Subscriber.Consumer;
using StackExchange.Redis;
using MiniERP.InventoryService.Behaviors;
using MiniERP.InventoryService.Extensions;
using Polly.Contrib.WaitAndRetry;
using Polly;
using MiniERP.InventoryService.MessageBus.Sender.Contracts;
using MiniERP.InventoryService.MessageBus.Sender;
using Quartz;
using MiniERP.InventoryService.Jobs;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

RegisterLogging();

ConfigureSecrets();

ConfigureKestrel();

AddControllers();

AddAuthentication();

AddDependencies();

AddRetryPolicies();

AddServices();

AddRabbitMQ();

AddQuartz();

AddRedis();

AddDBCOntext();

//////////////////////////////////////////////////////////////
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseExceptionHandler(new ExceptionHandlerOptions()
{
    AllowStatusCode404Response = true,
    ExceptionHandlingPath = "/error"
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<GrpcInventoryService>();

Migration.ApplyMigration(app);

app.Run();
///////////////////////////////////////////////////////////////////////

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
    .AddJsonFile("secrets/inventory.appsettings.secrets.json", optional: true)
    .AddEnvironmentVariables();
}

void ConfigureKestrel()
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(80, listenOptions => listenOptions.Protocols = HttpProtocols.Http1); //api
        options.ListenAnyIP(8080, listenOptions => listenOptions.Protocols = HttpProtocols.Http2); //grpc
    });
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
    builder.Services.AddGrpc();

    builder.Services.AddMediatR(config => {
        config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        config.AddOpenBehavior(typeof(LoggingBehavior<,>));
    });
}

void AddRetryPolicies()
{
    builder.Services.AddScoped<ISyncPolicy>(provider =>
    {
        return Policy
            .Handle<RedisTimeoutException>()
            .WaitAndRetry(
                Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 3),
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    context.GetLogger()?
                        .LogWarning("Delaying Cache call for {delay}ms, then making retry {retry}.", timespan.TotalMilliseconds, retryAttempt);
                });
    });
}

void AddServices()
{
    builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
    builder.Services.AddScoped<IStockSourcing, StockSourcing>();

    builder.Services.AddScoped<IJob, AutoOrderJob>();
}

void AddRabbitMQ()
{
    builder.Services.AddSingleton<IConsumerFactory, RabbitMQConsumerFactory>();
    builder.Services.AddHostedService<RabbitMQSubscriber>();

    builder.Services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>();
    builder.Services.AddScoped<IRabbitMQClient, RabbitMQClient>();
}

void AddQuartz()
{
    builder.Services.AddQuartz(q =>
    {
        q.UseMicrosoftDependencyInjectionJobFactory();

        var key = new JobKey(nameof(AutoOrderJob));

        q.AddJob<AutoOrderJob>(j => j.WithIdentity(key));

        q.AddTrigger(t =>
        {
            t.ForJob(key);
            t.WithIdentity($"{key}-trigger");
            t.WithCronSchedule("30 9 * * *");
        });
    });

    builder.Services.AddQuartzHostedService(x => x.WaitForJobsToComplete = true);
}

void AddDBCOntext()
{
    builder.Services.AddDbContext<AppDbContext>(opts =>
    {
        opts.UseNpgsql(builder.Configuration.GetConnectionString("inventoryservicePGSQL"));
    });
}

void AddRedis()
{
    builder.Services.AddDistributedRedisCache(opts =>
    {
        opts.InstanceName = "invsrv:";
        opts.ConfigurationOptions = new ConfigurationOptions()
        {
            AbortOnConnectFail = false,
            ConnectTimeout = 30000,
            ResponseTimeout = 30000,
            Password = builder.Configuration["redisPassword"],
            EndPoints = { builder.Configuration["redisHost"] },
        };
    });
}


