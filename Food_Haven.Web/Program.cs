using AutoMapper;
using BusinessLogic.Config;
using BusinessLogic.Mapper;
using BusinessLogic.Services.IngredientTagServices;
using BusinessLogic.Services.TypeOfDishServices;
using Food_Haven.Web.Hubs;
using Food_Haven.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DBContext;
using Repository.IngredientTagRepositorys;
using Repository.TypeOfDishRepositoties;
using Repository.IngredientTagRepositorys;
using System.Security.Claims;
using Quartz;
using Quartz.Simpl;
using Quartz.Spi;
using Microsoft.Extensions.DependencyInjection;

using Food_Haven.Web.Services.Auto;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<FoodHavenDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Register default identity using FoodHavenDbContext for Identity stores
builder.Services.AddIdentity<AppUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<FoodHavenDbContext>().AddDefaultTokenProviders(); ;
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.ConfigureRepository();
builder.Services.ConfigureServices();
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddHttpClient();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian hết hạn session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 9;
    options.Password.RequiredUniqueChars = 1;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;

    options.SignIn.RequireConfirmedAccount = true;
    options.SignIn.RequireConfirmedEmail = true;
});
builder.Services.AddScoped<IPasswordValidator<AppUser>, CustomPasswordValidator<AppUser>>();

// Cấu hình Cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Home/Login";
    options.LogoutPath = "/Home/Logout";
    options.AccessDeniedPath = "/Error/404";
    options.ReturnUrlParameter = "ReturnUrl";
    options.ExpireTimeSpan = TimeSpan.FromDays(14);
    options.SlidingExpiration = true;

});

//Add Authentication

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) 
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login";
        options.LogoutPath = "/Home/Logout";
        options.AccessDeniedPath = "/Error/404";
        options.ReturnUrlParameter = "ReturnUrl";
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.SlidingExpiration = true;
    })
.AddGoogle(options =>
{
    options.ClientId = "175622117461-85t7echfkmokt7rbikpi3tggtjrhiuam.apps.googleusercontent.com";
    options.ClientSecret = "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";
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Lưu thông tin đăng nhập qua Cookies
    options.CallbackPath = "/signin-google";

    // Hiển thị trang chọn tài khoản
    options.Events.OnRedirectToAuthorizationEndpoint = context =>
    {
        context.Response.Redirect(context.RedirectUri + "&prompt=select_account");
        return Task.CompletedTask;
    };

    // Xử lý lỗi khi xác thực
    options.Events.OnRemoteFailure = u =>
    {
        u.Response.Redirect("/Home/Login");
        u.HandleResponse();
        return Task.CompletedTask;
    };
}).AddMicrosoftAccount(microsoftOptions =>
{
    microsoftOptions.ClientId = "847933ee-a194-47e0-abe4-321be2134cda";
    microsoftOptions.ClientSecret = "hsW8Q~aF1aEGXwxetTMQ3AfXFv6yZQqSDf-fzaql";
    microsoftOptions.CallbackPath = "/signin-microsoft";
    microsoftOptions.Scope.Add("email");
    microsoftOptions.Scope.Add("profile");
    microsoftOptions.Scope.Add("openid");
    microsoftOptions.SaveTokens = true;
});

// Thời gian hết hạn token xác nhận email
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromMinutes(10);
});
builder.Services.Configure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, options =>
{
    options.Events.OnSigningIn = async context =>
    {
        var identity = (ClaimsIdentity)context.Principal.Identity;
        var userId = context.Principal.FindFirst("sub")?.Value ?? context.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (!string.IsNullOrEmpty(userId) && identity.FindFirst(ClaimTypes.NameIdentifier) == null)
        {
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId));
        }
    };
});

builder.Services.AddSignalR();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<ITypeOfDishService, TypeOfDishService>();

builder.Services.AddScoped<TypeOfDishRepository>();

builder.Services.AddHostedService<TypingCleanupService>();
builder.Services.AddSingleton<TypingCleanupService>();
builder.Services.AddScoped<IIngredientTagRepository, IngredientTagRepository>();
builder.Services.AddScoped<IngredientTagRepository>(); // 👈 thêm dòng này để inject trực tiếp


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

builder.Services.AddSingleton<RecipeSearchService>(provider => new RecipeSearchService(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\dataset", "full_dataset.csv")));
builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    // Cấu hình job
    var jobKey = new JobKey("ReleasePaymentJob");
    q.AddJob<ReleasePaymentJob>(opts => opts.WithIdentity(jobKey));

    // Cấu hình trigger
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("ReleasePaymentTrigger")
      .WithCronSchedule("0/5 * * * * ?") // Mỗi 15 phút
        .WithDescription("Release payment every 15 minutes"));
});

// Thêm hosted service
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error/404");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
// Chuyển tất cả các lỗi đến HomeController -> NotFoundPage




/*app.UseStatusCodePagesWithRedirects("/Error/404");
app.UseExceptionHandler("/Error/404");

*/

if (!app.Environment.IsEnvironment("Testing"))
{
    await SeedDataAsync(app);
}

app.UseHttpsRedirection();
app.UseStaticFiles();
/*app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/CategoryImage")),
    RequestPath = "/uploads/CategoryImage"
});*/
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapHub<CartHub>("/CartHub");
app.MapHub<ChatHub>("/chathub");
app.MapHub<FollowHub>("/followHub");



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

static async Task SeedDataAsync(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>();

        // Create roles if they don't exist
        string[] roles = { "User", "Admin", "Seller" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }


        // Create a default User user if it doesn't exist
        var User = await userManager.FindByEmailAsync("thanhpqce171732@fpt.edu.vn");
        if (User == null)
        {
            User = new AppUser { UserName = "thanhmax14", Email = "thanhpqce171732@fpt.edu.vn", EmailConfirmed = true };
            var result = await userManager.CreateAsync(User, "Password123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(User, "User");
            }
        }



        // Create a default admin user if it doesn't exist
        var adminUser = await userManager.FindByEmailAsync("admin@gmail.com");
        if (adminUser == null)
        {
            adminUser = new AppUser { UserName = "admin", Email = "admin@gmail.com", EmailConfirmed = true };
            var result = await userManager.CreateAsync(adminUser, "Password123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Create a default CTV user if it doesn't exist
        var ctvUser = await userManager.FindByEmailAsync("ctv@gmail.com");
        if (ctvUser == null)
        {
            ctvUser = new AppUser { UserName = "ctv", Email = "ctv@gmail.com", EmailConfirmed = true };
            var result = await userManager.CreateAsync(ctvUser, "Password123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(ctvUser, "Seller");
            }
        }

        var seller02 = await userManager.FindByEmailAsync("tranthaitansang23122003@gmail.com");
        if (seller02 == null)
        {
            seller02 = new AppUser { UserName = "Shang12345", Email = "tranthaitansang23122003@gmail.com", EmailConfirmed = true };
            var result = await userManager.CreateAsync(seller02, "Password123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(seller02, "Seller");
            }
        }
    }
}
public partial class Program { }