using robot_controller_api.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IRobotCommandDataAccess, RobotCommandRepository>();
builder.Services.AddScoped<IMapDataAccess, MapRepository>();
builder.Services.AddScoped<RobotContext>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();