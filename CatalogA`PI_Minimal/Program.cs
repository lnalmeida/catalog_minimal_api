using CatalogA_PI_Minimal.Context;
using CatalogA_PI_Minimal.Domain.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var strConnection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
{
options.UseMySql(strConnection, ServerVersion.AutoDetect(strConnection));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Catalog Minimal API Online.");

app.MapPost("/categories", async (Category category, AppDbContext db) =>
{
    db.Categories.Add(category);
    await db.SaveChangesAsync();
    return Results.Created($"/categories/{category.CategoryId}", category);
});

app.MapGet("/categories", (AppDbContext db) =>
{
    return db.Categories.ToList();
});

app.MapGet("/categories/{id}", async (AppDbContext db, Guid id) =>
{
    var category = await db.Categories.FindAsync(id);
    if (category is null) return Results.NotFound("Category not found");
    return Results.Ok(category);
});

app.MapPut("/categories/{id}", async (AppDbContext db, Guid id, Category category) =>
{
    var categoryToUpdate = await db.Categories.FindAsync(id);
    if (category is null) return Results.BadRequest("Category can not be null");
    if (categoryToUpdate is null) return Results.NotFound("Category not found");
    categoryToUpdate.Name = category.Name;
    categoryToUpdate.Description = category.Description;
    categoryToUpdate.UpdatedAt = DateTime.UtcNow.Date;
    await db.SaveChangesAsync();
    return Results.Ok(categoryToUpdate);
});

app.MapDelete("/categories/{id}", async (AppDbContext db, Guid id) =>
{
    var categoryToDelete = await db.Categories.FindAsync(id);
    if (categoryToDelete is null) return Results.NotFound("Category not found");
    db.Categories.Remove(categoryToDelete);
    await db.SaveChangesAsync();
    return Results.Ok(categoryToDelete);
});


app.Run();
