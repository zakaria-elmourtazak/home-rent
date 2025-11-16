using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyMvcAuthProject.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// --- Start of change ---
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("IdentityDb")); // You can name the in-memory database anything
// --- End of change ---

// 3. Add Razor Pages support (Identity UI is built on Razor Pages)
builder.Services.AddRazorPages();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
