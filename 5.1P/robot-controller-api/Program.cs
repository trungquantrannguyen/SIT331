using robot_controller_api.Persistence;
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
builder.Services.AddScoped<RobotContext>();

var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI(setup =>
{
    setup.InjectStylesheet("/styles/theme-flattop.css");
});

app.UseStaticFiles();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();