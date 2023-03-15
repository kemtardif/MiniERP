using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;
using MiniERP.ArticleService.Data;
using MiniERP.ArticleService.Formatters;
using MiniERP.ArticleService.MessageBus;
using MiniERP.ArticleService.Models;
using System.Net.Mime;

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

builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<IUnitRepository, UnitRepository>();

builder.Services.AddSingleton<IMessageBusClient, RabbitMQClient>();
builder.Services.AddSingleton<IMessageBusSender<Article>, RabbitMQArticleSender>();

builder.Services.AddDbContext<AppDbContext>(opts =>
{
    opts.UseNpgsql(builder.Configuration.GetConnectionString("articleservicePGSQL"));
});
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opts => 
                {
                    opts.Audience = builder.Configuration["AAD:ApplicationId"];
                    opts.Authority = string.Format("{0}{1}", builder.Configuration["AAD:Tenant"],
                                                             builder.Configuration["AAD:TenantId"]);

                });

// Add services to the container.

builder.Services.AddControllers(opts =>
{
    opts.InputFormatters.Insert(0, JsonPatchFormatter.GetJsonPatchInputFormatter());
})
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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Handles by Ingress K8s
//app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

Migration.ApplyMigration(app);

app.Run();
