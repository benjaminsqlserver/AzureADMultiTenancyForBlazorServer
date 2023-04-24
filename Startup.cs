using Microsoft.EntityFrameworkCore;
using Radzen;
using SimplifiedNorthwind.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace SimplifiedNorthwind
{
    public partial class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        partial void OnConfigureServices(IServiceCollection services);
        public void ConfigureServices(IServiceCollection services)
        {
            // Add services to the container.
            services.AddHttpContextAccessor();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddScoped<DialogService>();
            services.AddScoped<NotificationService>();
            services.AddScoped<TooltipService>();
            services.AddScoped<ContextMenuService>();
            services.AddScoped<SimplifiedNorthwind.ConDataService>();
            services.AddDbContext<SimplifiedNorthwind.Data.ConDataContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("ConDataConnection"));
            });
            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme).AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAd"));
            services.AddAuthorization();
            services.AddHttpClient("SimplifiedNorthwind").AddHeaderPropagation(o => o.Headers.Add("Cookie"));
            services.AddHeaderPropagation(o => o.Headers.Add("Cookie"));
            services.AddScoped<AuthenticationStateProvider, SimplifiedNorthwind.ApplicationAuthenticationStateProvider>();
            services.AddControllersWithViews();
            services.AddScoped<SimplifiedNorthwind.SecurityService>();
            OnConfigureServices(services);
        }

        partial void OnConfigure(IApplicationBuilder app, IWebHostEnvironment env);
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ConDataContext condataDbContext)
        {
            // Configure the HTTP request pipeline.
            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseHeaderPropagation();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
            condataDbContext.Database.Migrate();
        }
    }
}