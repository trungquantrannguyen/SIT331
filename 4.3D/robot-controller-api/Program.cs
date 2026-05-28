using robot_controller_api.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IRobotCommandDataAccess, RobotCommandEF>();
builder.Services.AddScoped<IMapDataAccess, MapEF>();
builder.Services.AddScoped<RobotContext>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();