using Feeds.Data;
using Feeds.Models;
using Feeds.Repositories;
using Feeds.Services;
using Feeds.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException(
                           "Connection string 'ApplicationDbContextConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

// Override identity user with application user 
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

// Identity configuration
builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings (lock out account on failure login attempt)
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;

    // options.SignIn.RequireConfirmedEmail = true;
    // options.SignIn.RequireConfirmedAccount = true;
});

// Cookie configuration
builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    // Set the cookie's expiration time to 15 minutes (logout if user stays idle for this period) 
    options.ExpireTimeSpan = TimeSpan.FromMinutes(15);

    // Redirect unauthenticated users
    options.LoginPath = "/Identity/Account/Login";
    // Redirect unauthorized users
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    // Reset cookie's expiration time when user actively using the application
    options.SlidingExpiration = true;
});

// Configure mail by reading json configuration in appsettings into MailConfigurationUtility class
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IMailService, MailService>();

// Add razor pages to application for identity
builder.Services.AddRazorPages();

// Register unit of work in the DI container
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

// Seed user roles to database
using (var scope = app.Services.CreateScope())
{
    var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Invoke user roles seed method
        await ContextSeed.SeedUserRoles(userManager, roleManager);
        // Invoke super user seed method
        await ContextSeed.SeedSuperAdmin(userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "An error occurred while seeding roles to the database.");
    }
}

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
app.UseAuthentication();
app.MapRazorPages();
app.UseAuthorization();

// Register admin area routes
app.MapControllerRoute(
    name: "administration",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();