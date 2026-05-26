using robot_controller_api.Persistence;
using robot_controller_api.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using robot_controller_api.Authentication;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Robot Controller API",
        Version = "v1",
        Description = "Backend service that provides robot command and map resources for the Moon robot simulator.",
        Contact = new OpenApiContact
        {
            Name = "Trung Quan Tran Nguyen",
            Email = "s225054634@deakin.edu.au"
        }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddScoped<IRobotCommandDataAccess, RobotCommandRepository>();
builder.Services.AddScoped<IMapDataAccess, MapRepository>();
builder.Services.AddScoped<IUserDataAccess, UserRepository>();
builder.Services.AddScoped<RobotContext>();

builder.Services.AddScoped<IPasswordHashService, BCryptPasswordHashService>();

builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(
        "BasicAuthentication",
        options => { }
    );

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Admin"));

    options.AddPolicy("UserOnly", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Admin", "User"));
});

var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI(setup =>
{
    setup.InjectStylesheet("/styles/theme-flattop.css");
});

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();