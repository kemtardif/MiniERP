using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;
using MiniERP.InventoryService.Data;
using MiniERP.InventoryService.Grpc;
using MiniERP.InventoryService.MessageBus.Subscriber;
using MiniERP.InventoryService.Services;
using System.Net.Mime;
using System.Reflection;
using MiniERP.InventoryService.MessageBus.Subscriber.Consumer;
using StackExchange.Redis;
using MiniERP.InventoryService.Behaviors;
using MiniERP.InventoryService.Extensions;
using Polly.Contrib.WaitAndRetry;
using Polly;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(opts =>
{
    opts.ColorBehavior = LoggerColorBehavior.Enabled;
    opts.TimestampFormat = "HH:mm:ss";
});

builder.Configuration
    .AddJsonFile("secrets/inventory.appsettings.secrets.json", optional: true)
    .AddEnvironmentVariables();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80, listenOptions => listenOptions.Protocols = HttpProtocols.Http1); //api
    options.ListenAnyIP(8080, listenOptions => listenOptions.Protocols = HttpProtocols.Http2); //grpc
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
builder.Services.AddGrpc();

builder.Services.AddMediatR(config => {
    config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

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

builder.Services.AddScoped<IRepository, ConcreteRepository>();

builder.Services.AddScoped<IInventoryService, InventoryService>();

builder.Services.AddSingleton<IConsumerFactory, RabbitMQConsumerFactory>();
builder.Services.AddHostedService<RabbitMQSubscriber>();

builder.Services.AddDbContext<AppDbContext>(opts =>
{
    opts.UseNpgsql(builder.Configuration.GetConnectionString("inventoryservicePGSQL"));
});

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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler(new ExceptionHandlerOptions()
    {
        AllowStatusCode404Response = true,
        ExceptionHandlingPath = "/error"
    });
}

//Handled via Ingress K8
//app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<GrpcInventoryService>();

Migration.ApplyMigration(app);

app.Run();
