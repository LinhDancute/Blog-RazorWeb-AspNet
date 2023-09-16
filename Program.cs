using AppRazor;
using AppRazor.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using AppRazor.Services;
using AppRazor.Security.Requirements;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("MyBlogContextConnection") ?? throw new InvalidOperationException("Connection string 'MyBlogContextConnection' not found.");

// Add services to the container.
builder.Services.AddRazorPages();

// Load appsettings.json configurations
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

// Register the MyBlogContext with the dependency injection container.
builder.Services.AddDbContext<MyBlogContext>(options =>
{
    //options.UseSqlServer(connectionString);
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyBlogContext"));
});

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

builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    // Trên 30 giây truy cập lại sẽ nạp lại thông tin User (Role)
    // SecurityStamp trong bảng User đổi -> nạp lại thông tinn Security
    options.ValidationInterval = TimeSpan.FromSeconds(5);
});

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

builder.Services.AddAuthorization(options => {
    options.AddPolicy("AllowEditRole", policyBuilder => {
        //Dieu kien cua policy
        policyBuilder.RequireAuthenticatedUser();
        //policyBuilder.RequireRole("Administrator");
        // policyBuilder.RequireRole("Editor");

        //policyBuilder.RequireClaim("manage.role", "add", "update");
        policyBuilder.RequireClaim("can-edit", "post");
    });

    options.AddPolicy("InGenZ", policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.Requirements.Add(new GenZRequirement());

        // new GenzRequirement() => Authorization handler
    });

    options.AddPolicy("ShowAdminMenu", pb => {
        pb.RequireRole("Administrator");
    });

    options.AddPolicy("CanUpdateArticle", builder => {
        builder.Requirements.Add(new ArticleUpdateRequirement());
    });
});

// Add other services to the container.
builder.Services.AddSingleton<ProductService>();
builder.Services.AddSingleton<IdentityErrorDescriber, AppIdentityErrorDescriber>();
builder.Services.AddTransient<IAuthorizationHandler, AppAuthorizationHandler>();

var app = builder.Build();

app.UseForwardedHeaders();
app.UseHttpLogging();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

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
     * Role-based authorization - xác thực quyền theo vai trò
      - Role(vai trò): (Admin, Editor, Manager, Member, ...)
        
     * Policy-based authorization
     * Claims-based authorization
        Claims -> Đặc tính, tính chất của đối tượng (user)

      Areas/Admin/Pages/Role
       Index
       Create
       Edit
       Delete

       dotnet new page -n Index -o Areas/Admin/Pages/Role -na RazorWebAspNet.Admin.Role
    
        [Authorize] - Controller, Action, PageModel -> dang nhap
    
    - Quản lý user: Sign Up, User, Role  ...

    



 /Identity/Account/Login
 /Identity/Account/Manage

 dotnet aspnet-codegenerator identity -dc razorweb.models.MyBlogContext

CallbackPath:
 https://localhost:5001/dang-nhap-tu-google
 

*/
