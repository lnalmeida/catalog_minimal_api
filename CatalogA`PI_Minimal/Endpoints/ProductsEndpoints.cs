using CatalogA_PI_Minimal.Context;
using CatalogA_PI_Minimal.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogA_PI_Minimal.Endpoints
{
    public static class ProductsEndpoints
    {
        public static void MapProductsEndpoints(this WebApplication app)
        {
            app.MapPost("/products", async (Product product, AppDbContext db) =>
            {
                db.Products.Add(product);
                await db.SaveChangesAsync();
                return Results.Created($"/products/{product.Id}", product);
            }).WithTags("Products").RequireAuthorization();

            app.MapGet("/products", (AppDbContext db) =>
            {
                return db.Products.AsNoTracking().ToListAsync();
            }).WithTags("Products");

            app.MapGet("/products/{id}", async (AppDbContext db, Guid id) =>
            {
                var product = await db.Products.FindAsync(id);
                if (product is null) return Results.NotFound("Product not found");
                return Results.Ok(product);
            }).WithTags("Products");

            app.MapPut("/products/{id}", async (AppDbContext db, Guid id, Product product) =>
            {
                if (product is null) return Results.BadRequest("Product can not be null");
                var productToUpdate = await db.Products.FindAsync(id);
                if (productToUpdate is null) return Results.NotFound("Product not found");
                db.Entry(productToUpdate).CurrentValues.SetValues(product);
                productToUpdate.LastUpdatedAt = DateTime.UtcNow;
                await db.SaveChangesAsync();
                return Results.Ok(productToUpdate);
            }).WithTags("Products").RequireAuthorization();

            app.MapDelete("/products/{id}", async (AppDbContext db, Guid id) =>
            {
                var productToDelete = await db.Products.FindAsync(id);
                if (productToDelete is null) return Results.NotFound("Product not found");
                db.Products.Remove(productToDelete);
                await db.SaveChangesAsync();
                return Results.Ok(productToDelete);
            }).WithTags("Products").RequireAuthorization();
        }
    }
}
