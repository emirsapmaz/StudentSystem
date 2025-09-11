using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentSystem.Data;
using StudentSystem.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<user>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;

})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddControllersWithViews();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    //options.LogoutPath = "/Account/Logout";
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<user>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await ContextSeed.SeedRolesAsync(userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<user>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    var admin = new user
    {
        UserName = "admin@pusula.com",
        Email = "admin@pusula.com",
        FirstName = "emirAdmin",
        LastName = "sapmazAdmin",
        gender = "Male",
        roles = Enums.Roles.Admin.ToString()
    };
    if (userManager.Users.All(u => u.Id != admin.Id))
    {
        var user = await userManager.FindByEmailAsync(admin.Email);
        if (user == null)
        {
            await userManager.CreateAsync(admin, "pusula");//password is pusula
            await userManager.AddToRoleAsync(admin, Enums.Roles.Admin.ToString());

        }

    }
    var student = new user
    {
        UserName = "student@pusula.com",
        Email = "student@pusula.com",
        FirstName = "emirStudent",
        LastName = "sapmazStudent",
        gender = "Male",
        roles = Enums.Roles.Student.ToString()
    };
    if (userManager.Users.All(u => u.Id != student.Id))
    {
        var user = await userManager.FindByEmailAsync(student.Email);
        if (user == null)
        {
            await userManager.CreateAsync(student, "pusula");//password is pusula
            await userManager.AddToRoleAsync(student, Enums.Roles.Student.ToString());

        }

    }
    var teacher = new user
    {
        UserName = "teacher@pusula.com",
        Email = "teacher@pusula.com",
        FirstName = "emirTeacher",
        LastName = "sapmazTeacher",
        gender = "Male",
        roles = Enums.Roles.Teacher.ToString()
    };
    if (userManager.Users.All(u => u.Id != teacher.Id))
    {
        var user = await userManager.FindByEmailAsync(teacher.Email);
        if (user == null)
        {
            await userManager.CreateAsync(teacher, "pusula");//password is pusula
            await userManager.AddToRoleAsync(teacher, Enums.Roles.Teacher.ToString());

        }

    }

}


app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
