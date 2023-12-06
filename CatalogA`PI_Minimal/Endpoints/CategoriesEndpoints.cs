using CatalogA_PI_Minimal.Context;
using CatalogA_PI_Minimal.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogA_PI_Minimal.Endpoints
{
    public static class CategoriesEndpoints
    {
        public static void MapCategoriesEndpoints(this WebApplication app)
        {
            app.MapPost("/categories", async (Category category, AppDbContext db) =>
            {
                db.Categories.Add(category);
                await db.SaveChangesAsync();
                return Results.Created($"/categories/{category.CategoryId}", category);
            }).WithTags("Categories").RequireAuthorization();

            app.MapGet("/category/products", async ([FromQuery] Guid categoryId, AppDbContext db) =>
            {
                return db.Products.AsNoTracking().Include(p => p.Category).Where(p => p.CategoryId == categoryId);
            }).WithTags("Categories");

            app.MapGet("/categories", (AppDbContext db) =>
            {
                return db.Categories.AsNoTracking().ToListAsync();
            }).WithTags("Categories");

            app.MapGet("/categories/{id}", async (AppDbContext db, Guid id) =>
            {
                var category = await db.Categories.FindAsync(id);
                if (category is null) return Results.NotFound("Category not found");
                return Results.Ok(category);
            }).WithTags("Categories");

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
            }).WithTags("Categories").RequireAuthorization();

            app.MapDelete("/categories/{id}", async (AppDbContext db, Guid id) =>
            {
                var categoryToDelete = await db.Categories.FindAsync(id);
                if (categoryToDelete is null) return Results.NotFound("Category not found");
                db.Categories.Remove(categoryToDelete);
                await db.SaveChangesAsync();
                return Results.Ok(categoryToDelete);
            }).WithTags("Categories").RequireAuthorization();
        }
    }
}
