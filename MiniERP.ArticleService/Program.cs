using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniERP.ArticleService.Data;
using MiniERP.ArticleService.Formatters;
using MiniERP.ArticleService.Models;
using System.Net.Mime;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<IUnitRepository, UnitRepository>();
builder.Services.AddDbContext<AppDbContext>(opts =>
{
    opts.UseNpgsql(builder.Configuration.GetConnectionString("articleservicePGSQL"));
});
builder.Services.AddAutoMapper(typeof(Program));

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

Migration.ApplyMigration(app);

app.Run();
