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

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(opts => 
{
    opts.ColorBehavior = LoggerColorBehavior.Enabled;
    opts.TimestampFormat = "HH:mm:ss";
});

builder.Configuration
    .AddJsonFile("secrets/appsettings.secrets.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.AddScoped<IRepository, ArticleRepository>();

builder.Services.AddScoped<IValidator<CreateDTO>, CreateValidator>();
builder.Services.AddScoped<IValidator<UpdateDTO>, UpdateValidator>();

builder.Services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>();
builder.Services.AddScoped<IRabbitMQClient, RabbitMQClient>();


builder.Services.AddDbContext<AppDbContext>(opts =>
{
    opts.UseNpgsql(builder.Configuration.GetConnectionString("articleservicePGSQL"));
});


builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddMediatR(config => {
    config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
    config.AddOpenBehavior(typeof(MessagingBehavior<,>));
    config.AddBehavior<IPipelineBehavior<CreateCommand, Result<ReadDTO>>, CreateValidationBehavior>();
    config.AddBehavior<IPipelineBehavior<UpdateCommand, Result<ReadDTO>>, UpdateValidationBehavior>();
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opts => 
                {
                    opts.Audience = builder.Configuration["AAD:ApplicationId"];
                    opts.Authority = string.Format("{0}{1}", builder.Configuration["AAD:Tenant"],
                                                             builder.Configuration["AAD:TenantId"]);

                });

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler(new ExceptionHandlerOptions()
    {
        AllowStatusCode404Response = true, 
        ExceptionHandlingPath = "/error"
    });
}

// Handles by Ingress K8s
//app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

Migration.ApplyMigration(app);

app.Run();
