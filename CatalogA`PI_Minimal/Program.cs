using CatalogA_PI_Minimal.Context;
using CatalogA_PI_Minimal.Domain.Models;
using CatalogA_PI_Minimal.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    //c.SwaggerDoc("V1", new OpenApiInfo{ Title = "Catalog Minimal API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorizatio header using the Bearer scheme." +
        "\r\n\r\n Enter 'Bearer [SAPCE] and then your token in the text input below." +
        "\r\n\r\nExample: \"Bearer 1234lkjhfgksdhfglkjhdsfg2323@\"",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme, 
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var strConnection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
{
options.UseMySql(strConnection, ServerVersion.AutoDetect(strConnection));
});

builder.Services.AddSingleton<ITokenService>(new TokenService());

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = builder.Configuration["JWTSecret:Issuer"],
                        ValidAudience =  builder.Configuration["JWTSecret:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTSecret:Key"]))
                    };
                });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("/", () => "Catalog Minimal API Online.").WithTags("Test");

app.MapPost("/auth/login", [AllowAnonymous] (UserModel user, ITokenService tokenService) =>
    {
        if (user == null) return Results.BadRequest("Invalid Login!");
        if(user.Username == "nunes" && user.Password == "senha123")
        {
            var tokenString = tokenService.GenerateToken(app.Configuration["JWTSecret:Key"],
                                                         app.Configuration["JWTSecret:Issuer"],
                                                         app.Configuration["JWTSecret:Audience"],
                                                         user);
            
            return Results.Ok(new {token =  tokenString});
        }
        else
        {
            return Results.BadRequest("Invalid Login!!");
        };
    }).WithTags("Auth");

app.MapPost("/categories", async (Category category, AppDbContext db) =>
{
    db.Categories.Add(category);
    await db.SaveChangesAsync();
    return Results.Created($"/categories/{category.CategoryId}", category);
}).WithTags("Categories").RequireAuthorization();

app.MapGet("/category/products/{id}", async (AppDbContext db, Guid categoryId) =>
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

app.MapPost("/products", async (Product product, AppDbContext db) =>
{
    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/products/{product.Id}", product);
}).WithTags("Products").RequireAuthorization();

app.MapGet("/products", (AppDbContext db) =>
{
    return db.Products.AsNoTracking().ToList();
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
    productToUpdate.LastUpdatedAt= DateTime.UtcNow;
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

app.UseAuthentication();
app.UseAuthorization();

app.Run();
