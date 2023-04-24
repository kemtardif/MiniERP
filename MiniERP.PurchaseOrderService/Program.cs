using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;
using MiniERP.PurchaseOrderService.Data;
using MiniERP.PurchaseOrderService.Grpc;
using MiniERP.PurchaseOrderService.Grpc.Protos;
using MiniERP.PurchaseOrderService.Models;
using MiniERP.PurchaseOrderService.Services;
using MiniERP.PurchaseOrderService.Validators;
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
    .AddJsonFile("secrets/purchaseorder.appsettings.secrets.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.AddScoped<IPOService, POService>();
builder.Services.AddScoped<IRepository, PORepository>();
builder.Services.AddScoped<IDataClient, GrpcDataClient>();

builder.Services.AddScoped<IValidator<PurchaseOrder>, POValidator>();   

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());
});

builder.Services.AddDbContext<AppDbContext>(opts =>
{
    opts.UseNpgsql(builder.Configuration.GetConnectionString("purchaseorderservicePGSQL"));
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

