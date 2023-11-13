using CrudUi.Filter.ActionFilter;
using CrudUi.MiddleWars;
using Entities;
using Entities.IdentityIntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContacts;
using Serilog;
using ServiceContract.Interfaces;
using Services;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    //options.Filters.Add<PersonListActionFilter>();
    var Logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();

    options.Filters.Add(new ResponseHeaderActionFilter(Logger, "Clopla-key", "Glopal-Value"));
});

builder.Services.AddScoped<IPersonService, PersonsService>();
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();
builder.Services.AddScoped<PersonListActionFilter>(); // service filter
//Serilog
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
{
    loggerConfiguration
    .ReadFrom.Configuration(context.Configuration) //read configuration settings from built-in IConfiguration
    .ReadFrom.Services(services); //read out current app's services and make them available to serilog
});

builder.Services.AddDbContext<CrudDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("ConStr"));
});

/*
  //access dbcontext in program.cs
    var DbContext = builder.Services.BuildServiceProvider()
    .GetRequiredService<CrudDbContext>();
    var Count = DbContext.Countries.Count();
*/

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(opt =>
{
    opt.Password.RequireDigit = false;
    opt.Password.RequireLowercase = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequiredLength = 5; // Set the desired password length here
    opt.Password.RequireNonAlphanumeric = false;
})
 .AddEntityFrameworkStores<CrudDbContext>()
 .AddDefaultTokenProviders()
 .AddUserStore<UserStore<ApplicationUser, ApplicationRole, CrudDbContext, Guid>>()
 .AddRoleStore<RoleStore<ApplicationRole, CrudDbContext, Guid>>();

//var appDbContextFactory = new ApplicationDbContextFactory().CreateDbContext(new string[] { });
//var Count = appDbContextFactory.Countries.Count();

var app = builder.Build();
//app.Logger.LogDebug("Debug Message");
//app.Logger.LogInformation("LogInformation");
//app.Logger.LogWarning("LogWarning");
//app.Logger.LogError("Debug Message");
//app.Logger.LogCritical("LogCritical");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandling();
}

//app.UseExceptionHandler("/Home/Error");
//// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//app.UseHsts();

app.UseHttpsRedirection();

//app.UseHttpLogging();

app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Person}/{action=Index}/{id?}");

var UserManger = builder.Services.BuildServiceProvider().GetRequiredService<UserManager<ApplicationUser>>();
var RoleManger = builder.Services.BuildServiceProvider().GetRequiredService<RoleManager<ApplicationRole>>();
await IdentityInitializer.Initialize(UserManger, RoleManger);

app.Run();
/*
 * Logging
log level
app.Logger.LogDebug("Debug Message");
app.Logger.LogInformation("LogInformation"); Default Level
app.Logger.LogWarning("LogWarning");
app.Logger.LogError("Debug Message");
app.Logger.LogCritical("LogCritical");
Just Inject Ilooger<ControllerOrServiceName>();
HttpLogging Middelwre The Log Everything in request and response
app.UseHttpLogging();
 */
/*
 Serilog Configuration
1-Serilog Package
2-Serilog.AspNetCore Package
3-Serilog.Sinks.MSSqlServer for db you need to create database first
Serilog Configuration
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
{
    loggerConfiguration
    .ReadFrom.Configuration(context.Configuration) //read configuration settings from built-in IConfiguration
    .ReadFrom.Services(services); //read out current app's services and make them available to serilog
});

appSetting :
1-- Write Log in All Sinks
  "Serilog": {
    "MinimumLevel": "Information",
    "Using": [
      "Serilog.Sinks.Console",
      "Serilog.Sinks.File",
      "Serilog.Sinks.MSSqlServer"
    ],
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "File",
        "Args": {
          "path": "Logs/log.txt",
          "rollingInterval": "Minute",
          "fileSizeLimitBytes": 1048576,
          "rollOnFileSizeLimit": true
        }
      },
      {
        "Name": "MSSqlServer",
        "Args": {
          "connectionString": "Data Source=DESKTOP-45F6T46\\SQLEXPRESS;Initial Catalog=CrudSolutionLogging;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False",
          "tableName": "Logs",
          "autoCreateSqlTable": true
        }
      }

    ]
  },
 */