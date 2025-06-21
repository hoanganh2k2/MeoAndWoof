using BusinessObject.Models;
using MeoAndWoofClient.Email;
using MeoAndWoofClient.Payment;
using MeoAndWoofClient.Payment.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
namespace MeoAndWoofClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddSession();
            builder.Services.AddDbContext<PostgresContext>(option =>
            {
                option.UseNpgsql(builder.Configuration.GetConnectionString("MeowWoofDb"));
            });
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
                .AddCookie()
                .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
                {
                    options.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientId").Value;
                    options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;
                    options.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
                    options.SaveTokens = true;
                    options.Events.OnCreatingTicket = ctx =>
                    {
                        List<AuthenticationToken> tokens = ctx.Properties.GetTokens().ToList();
                        tokens.Add(new AuthenticationToken { Name = "TicketCreated", Value = DateTime.UtcNow.ToString() });
                        ctx.Properties.StoreTokens(tokens);
                        return Task.CompletedTask;
                    };
                }).AddFacebook(options =>
                {
                    options.AppId = builder.Configuration["Facebook:AppId"];
                    options.AppSecret = builder.Configuration["Facebook:AppSecret"];
                    options.CallbackPath = "/signin-facebook";
                    options.SaveTokens = true;
                });


            EmailConfiguration emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
            builder.Services.AddSingleton(emailConfig);
            builder.Services.AddSingleton<IEmailService, EmailService>();
            builder.Services.AddSingleton<VnPayLibrary>();
            builder.Services.AddSingleton<IVnPayService, VnPayService>();
            builder.Services.AddHttpClient();
            builder.Services.AddHttpContextAccessor();
            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}