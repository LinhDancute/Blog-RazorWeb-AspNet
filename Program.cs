using System;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using AppRazor;
using AppRazor.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using AppRazor.models;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("MyBlogContextConnection") ?? throw new InvalidOperationException("Connection string 'MyBlogContextConnection' not found.");

// Add services to the container.
builder.Services.AddRazorPages();

// builder.Services.AddHttpLogging(options =>
// {
//     options.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders;
// });

// Load appsettings.json configurations
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();



// Register the MyBlogContext with the dependency injection container.
builder.Services.AddDbContext<MyBlogContext>(options =>
{
    //options.UseSqlServer(connectionString);
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection"));
});

//builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<MyBlogContext>();

//đăng kí Identity
//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<MyBlogContext>();
// builder.Services.AddIdentity<AppUser, IdentityRole>()
//     .AddEntityFrameworkStores<MyBlogContext>()
//     .AddDefaultTokenProviders();


builder.Services.AddIdentity<AppUser, IdentityRole>()
                    .AddEntityFrameworkStores<MyBlogContext>()
                    .AddDefaultTokenProviders();


// Truy cập IdentityOptions
builder.Services.Configure<IdentityOptions>(options =>
{
    // Thiết lập về Password
    options.Password.RequireDigit = false; // Không bắt phải có số
    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
    options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không bắt buộc chữ in
    options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

    // Cấu hình Lockout - khóa user
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
    options.Lockout.MaxFailedAccessAttempts = 3; // Thất bại 5 lầ thì khóa
    options.Lockout.AllowedForNewUsers = true;

    // Cấu hình về User.
    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;  // Email là duy nhất

    // Cấu hình đăng nhập.
    options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
    options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
    options.SignIn.RequireConfirmedAccount = true;         //Xác thực tài khoản
});

var mailSetting = builder.Configuration.GetSection("MailSettings");
builder.Services.Configure<MailSettings>(mailSetting);
builder.Services.AddSingleton<IEmailSender, SendMailService>();
builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/login/";
            options.LogoutPath = "/logout/";
            options.AccessDeniedPath = "/khongduoctruycap.html";
        });


// builder.Services.AddAuthentication(options =>
//                     {
//                         options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//                         options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//                         options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//                     })
//                     .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
//                     {
//                         //   ... 
//                     })
//                     .AddGoogle(options =>
//                     {
//                         IConfigurationSection googleAuthNSection =
//                             configuration.GetSection("Authentication:Google");

//                         options.ClientId = googleAuthNSection["ClientId"];
//                         options.ClientSecret = googleAuthNSection["ClientSecret"];
//                         options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//                         options.CorrelationCookie.SameSite = SameSiteMode.Lax;
//                         options.CallbackPath = "/dang-nhap-tu-google";
//                     });

builder.Services.AddAuthentication().AddCookie()
                .AddGoogle(options =>
                {
                    var gconfig = configuration.GetSection("Authentication:Google");
                    options.ClientId = gconfig["ClientId"];
                    options.ClientSecret = gconfig["ClientSecret"];
                    options.CorrelationCookie.SameSite = SameSiteMode.Lax;
                    options.CallbackPath = "/dang-nhap-tu-google"; // Relative path instead of absolute URL
                })
                .AddFacebook(options =>
                {
                    var fconfig = configuration.GetSection("Authentication:Facebook");
                    options.AppId = fconfig["AppId"];
                    options.AppSecret = fconfig["AppSecret"];
                    options.CallbackPath = "/dang-nhap-tu-facebook";
                });
// .AddTwitter()
// .AddMicrosoftAccount();

builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
            });


// Add other services to the container.
builder.Services.AddSingleton<ProductService>();

var app = builder.Build();

app.UseForwardedHeaders();
app.UseHttpLogging();

// app.Use(async (context, next) =>
// {
//     // Connection: RemoteIp
//     app.Logger.LogInformation("Request RemoteIp: {RemoteIpAddress}",
//         context.Connection.RemoteIpAddress);

//     await next(context);
// });

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// var cookiePolicyOptions = new CookiePolicyOptions
// {
//     MinimumSameSitePolicy = SameSiteMode.Strict,
// };

// app.UseCookiePolicy(CookiePolicyOptions);

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();

app.MapRazorPages();

app.Run();

/*
CREATE, READ, UPDATE, DELETE (CRUD)

dotnet aspnet-codegenerator razorpage -m razorweb.models.Article -dc razorweb.models.MyBlogContext -outDir Pages/Blog -udl --referenceScriptLibraries


Identity:
    - Athentication: Xác định danh tính  -> Login, Logout ...
    - Authorization: Xác thực quyền truy cập
    - Quản lý user: Sign Up, User, Role  ...




 /Identity/Account/Login
 /Identity/Account/Manage

 dotnet aspnet-codegenerator identity -dc razorweb.models.MyBlogContext

CallbackPath:
 https://localhost:5001/dang-nhap-tu-google
 

*/
