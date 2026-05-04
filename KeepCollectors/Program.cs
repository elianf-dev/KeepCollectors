using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Data;
using DataAccessLayer.DataModels;
using DataAccessLayer;
using Microsoft.AspNetCore.Identity;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<CollectorsKeepDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    b => b.MigrationsAssembly("KeepCollectors")
    ));


builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<CollectorsKeepDbContext>();



var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CollectorsKeepDbContext>();

    // If you use migrations, this will create/update the DB automatically:
    context.Database.Migrate();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // 1) Ensure Admin role exists
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    // 2) Assign Admin role to your user (email from config)
    var adminEmail = builder.Configuration["AdminUser:Email"];

    if (!string.IsNullOrWhiteSpace(adminEmail))
    {
        var user = await userManager.FindByEmailAsync(adminEmail);

        if (user != null)
        {
            if (!await userManager.IsInRoleAsync(user, "Admin"))
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }

    if (!context.Products.Any())
    {
        context.Products.AddRange(
            new Product
            {
                Name = "Spider-Man Comic (Amazing Fantasy #15 Reprint)",
                Description = "Iconic Spider-Man comic for collectors. Great condition reprint for display.",
                Price = 29.99m,
                QuantityAvailable = 10,
                ImagePath = "/images/products/spiderman.jpg",
                Category = Product.ProductCategory.Comics
            },
            new Product
            {
                Name = "Nintendo Game Boy (Classic)",
                Description = "Original handheld console. Tested and working. Perfect for retro collectors.",
                Price = 119.99m,
                QuantityAvailable = 4,
                ImagePath = "/images/products/gameboy.jpg",
                Category = Product.ProductCategory.RetroGames
            },
            new Product
            {
                Name = "Charizard Holo Card (Base Set Style)",
                Description = "Fan-favorite Charizard holo style card. Great for Pokémon collectors.",
                Price = 349.99m,
                QuantityAvailable = 2,
                ImagePath = "/images/products/charizard.jpg",
                Category = Product.ProductCategory.Cards
            }
        );

        context.SaveChanges();
    }
}

app.Run();