using DiplomApplication.Application.Interfaces;
using DiplomApplication.Infrastructure.DependencyInjection;
using DiplomApplication.Infrastructure.Repositories;
using DiplomApplication.Infrastructure.Services;
using DiplomApplication.MapperProfile;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DiplomApplication;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build())
            .Enrich.FromLogContext()
            .CreateLogger();

        try
        {
            Log.Information("Запуск веб-приложения");
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                Args = args,
                WebRootPath = "Web/wwwroot"
            });
            builder.Services.AddApplicationDependencies(builder.Configuration);
            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
            builder.Host.UseSerilog(); // важно


            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(option =>
                {
                    option.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                    option.LoginPath = "/Account/Login";
                    option.AccessDeniedPath = "/Account/Login";
                });

            builder.Services.AddSession(option =>
            {
                option.IdleTimeout = TimeSpan.FromMinutes(5);
                option.Cookie.HttpOnly = true;
                option.Cookie.IsEssential = true;
            });

            builder.Services.AddControllersWithViews()
                .AddRazorOptions(options =>
                {
                    options.ViewLocationFormats.Clear();
                    options.ViewLocationFormats.Add("/Web/Views/{1}/{0}.cshtml");    // Для Views/Home/Index.cshtml
                    options.ViewLocationFormats.Add("/Web/Views/Shared/{0}.cshtml"); // Для Views/Shared/_Layout.cshtml
                });

            var app = builder.Build();

            app.UseSerilogRequestLogging(); // логгирование HTTP-запросов

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Приложение завершилось с фатальной ошибкой");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}