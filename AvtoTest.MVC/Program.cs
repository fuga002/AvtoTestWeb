using AvtoTest.Data.Context;
using AvtoTest.Data.Entities;
using AvtoTest.Data.Repositories;
using AvtoTest.Services.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<TestRepository>();
builder.Services.AddScoped<TestService>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services
    .AddDefaultIdentity<IdentityUser>(options => 
        options.SignIn.RequireConfirmedAccount = false )
    .AddEntityFrameworkStores<AppDbContext>();




/*
builder.Services.AddDefaultIdentity<CustomUser>(options =>
        options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<AppDbContext>()
    .AddUserManager<CustomUser>();*/



/*builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();*/

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();


using (var scopeService = app.Services.CreateScope())
{
    var role = "Admin";
    /*var roleManager = scopeService.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roleModel = await roleManager.FindByNameAsync(role);

    if (roleModel == null)
    {
        roleModel = new()
        {
            Name = role
        };

      await  roleManager.CreateAsync(roleModel);
    }*/

    var userManager = scopeService.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    var email = "admin@admin.com";
    var password = "Asd123.";

    var user =await userManager.FindByEmailAsync(email);

    if (user == null)
    {
        user = new()
        {
            Email = email,
            UserName = email,
        };
        await userManager.CreateAsync(user,password);

       await userManager.AddToRoleAsync(user, role);
    }
}

app.Run();
