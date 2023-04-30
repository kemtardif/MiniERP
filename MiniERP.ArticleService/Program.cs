using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;
using MiniERP.ArticleService.Behaviors;
using MiniERP.ArticleService.Behaviors.CreateBehavior;
using MiniERP.ArticleService.Behaviors.UpdateBehavior;
using MiniERP.ArticleService.Commands;
using MiniERP.ArticleService.Data;
using MiniERP.ArticleService.DTOs;
using MiniERP.ArticleService.MessageBus.Sender;
using MiniERP.ArticleService.MessageBus.Sender.Contracts;
using MiniERP.ArticleService.Models;
using MiniERP.ArticleService.Validators;
using System.Net.Mime;
using System.Reflection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

RegisterLogging();

ConfigureSecrets();

AddControllers();

AddAuthentication();

AddDependencies();

AddServices();

AddRabbitMQ();

AddDBCOntext();

///////////////Usual stuff////////////////////////////////
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
////////////////////////////////////////////////////////////

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
    .AddJsonFile("secrets/appsettings.secrets.json", optional: true)
    .AddEnvironmentVariables();
}


void AddControllers()
{
    builder.Services.AddControllers()
                .AddNewtonsoftJson()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var result = new UnprocessableEntityObjectResult(context.ModelState);

                        result.ContentTypes.Add(MediaTypeNames.Application.Json);

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
        config.AddBehavior<IPipelineBehavior<CreateCommand, Result<ReadDTO>>, CreateValidationBehavior>();
        config.AddBehavior<IPipelineBehavior<UpdateCommand, Result<ReadDTO>>, UpdateValidationBehavior>();
    });

}

void AddServices()
{
    builder.Services.AddScoped<IRepository, ArticleRepository>();
    builder.Services.AddScoped<IValidator<CreateDTO>, CreateValidator>();
    builder.Services.AddScoped<IValidator<UpdateDTO>, UpdateValidator>();
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
        opts.UseNpgsql(builder.Configuration.GetConnectionString("articleservicePGSQL"));
    });
}


