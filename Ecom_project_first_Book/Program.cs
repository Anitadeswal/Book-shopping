
using Ecom_project_first_Book.DataAccess.Data;
using Ecom_project_first_Book.DataAccess.Repositry;
using Ecom_project_first_Book.DataAccess.Repositry.IRepositry;
using Ecom_project_first_Book.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("conStr");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders().AddEntityFrameworkStores<ApplicationDbContext>();



builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.AddRazorPages();

//builder.Services.AddScoped<ICategoryRepositry,CategoryRepositry>();
//builder.Services.AddScoped<ICoverTypeRepositry, CoverTypeRepositry>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();

//builder.Services.AddScoped<ISmsService,TwilioService>();

builder.Services.Configure<StripeSetting>
    (builder.Configuration.GetSection("StripeSetting"));

//builder.Services.Configure<SMSSettings>
//    (builder.Configuration.GetSection("SMSSettings"));


builder.Services.Configure<EmailSettings>
    (builder.Configuration.GetSection("EmailSettings"));


builder.Services.ConfigureApplicationCookie(Option =>
{
    Option.LoginPath = $"/Identity/Account/Login";
    Option.LogoutPath =$"/Identity/Account/Logout";
    Option.AccessDeniedPath = $"/Identity/Account/AccessDenied";

});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly= true ;
    options.Cookie.IsEssential= true ;

});

//builder.Services.AddAuthentication().AddFacebook(options =>
//{
//    options.AppId = "";
//    options.AppSecret = "";

//});

builder.Services.AddAuthentication().AddGoogle(options =>
{
    options.ClientId = "1042963166165-g30111fmkopoo3blmrn4lfpqj1a3slg3.apps.googleusercontent.com";
    options.ClientSecret = "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";

});
//builder.Services.AddAuthentication().AddInstagram(options =>
//{
//    options.ClientId = "624789910127116";
//    options.ClientSecret = "448cd3e32d1d7381e799790eb1d9344f";

//});

builder.Services.AddAuthentication().AddSpotify(options =>
{
    options.ClientId = "bd76b991d1214f339e27718648a3a4f3";
    options.ClientSecret = "c72c099e09cc4230925201104790cbd4";

});
builder.Services.AddAuthentication().AddGitHub(options =>
{
    options.ClientId = "Ov23liwDIK2ecs98vU2z";
    options.ClientSecret = "c66e993bba73cc5ba260936ff0e1d869fa2f320a";

});

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
StripeConfiguration.ApiKey = builder.Configuration.GetSection("StripeSetting")["SecretKey"];
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
