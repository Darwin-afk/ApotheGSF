using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ApotheGSF.Models;
using ApotheGSF.Clases;
using ReflectionIT.Mvc.Paging;
using Rotativa.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentity<AppUsuario, AppRol>(options =>
{
    //Definir las caracteristicas de la contraseña
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 4;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // Determina si se necesita el consentimiento del usuario para cookies no esenciales para una solicitud determinada.
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

//Donde redirecciona cuando no estan autorizados a ver paginas
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Home/Login";  //Cuando alguien no tenga permiso a una pagina, lo enviara aqui.
    options.AccessDeniedPath = "/Home/AccesoDenegado";
    options.Cookie.Name = ".applicationname";
    options.Cookie.HttpOnly = true; // Ser true para prevenir XSS
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

ConfigurationManager configuration = builder.Configuration;

string connStr = configuration.GetConnectionString("prodConn");
if (configuration.GetSection("AppSettings")["EnProduccion"].Equals("NO"))
    connStr = configuration.GetConnectionString("devConn");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connStr), ServiceLifetime.Scoped);

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddPaging(options => {
    options.ViewName = "Bootstrap4";
    options.HtmlIndicatorDown = " <span>&darr;</span>";
    options.HtmlIndicatorUp = " <span>&uarr;</span>";
    options.PageParameterName = "pageindex";
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

app.UseAuthentication();
app.UseAuthorization();
app.UseCookiePolicy();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

RotativaConfiguration.Setup(builder.Environment.WebRootPath, "../Rotativa");

app.Run();
