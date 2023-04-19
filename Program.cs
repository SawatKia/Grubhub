using Grubhub;
using Grubhub.Data;
using Microsoft.AspNetCore;
//using System.Threading;
using Microsoft.EntityFrameworkCore;
using Grubhub.Controllers;
using Grubhub.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//DB service to connect to DB sql server by GrubhubDBContext
builder.Services.AddDbContext<GrubhubDBContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

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

app.UseSession();

//// create a new timer that calls the DeleteExpiredPosts method every hour
//var timer = new System.Threading.Timer(callback: (state) =>
//{
//    using (var scope = app.Services.CreateScope())
//{
//        var controller = scope.ServiceProvider.GetService<GrabberController>();
//        controller.DeleteExpiredPosts();
//    }
//}, state: null, dueTime: TimeSpan.Zero, period: TimeSpan.FromHours(1));
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=LoginRegis}/{id?}");

app.Run();
