using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniERP.InventoryService.Data;
using System.Net.Mime;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();


builder.Services.AddDbContext<AppDbContext>(opts =>
{
    opts.UseNpgsql(builder.Configuration.GetConnectionString("inventoryservicePGSQL"));
});


builder.Services.AddDistributedRedisCache(opts =>
{
    opts.InstanceName = "invsrv_";
    opts.Configuration = builder.Configuration.GetConnectionString("inventoryserviceRedis");
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

//Handled via Ingress K8
//app.UseHttpsRedirection();



app.UseAuthorization();

app.MapControllers();

Migration.ApplyMigration(app);

app.Run();
