using InstaFakeAnalyzer.Data;
using InstaFakeAnalyzer.Services;
using Microsoft.EntityFrameworkCore;

var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
var apiUrl = Environment.GetEnvironmentVariable("ApiUrl");

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

#if DEBUG
connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
apiUrl = builder.Configuration["Urls:ApiUrl"];
#endif



builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();
builder.Services.AddHttpClient<DeepSeekService>(client =>
{
    client.BaseAddress = new Uri(apiUrl?.ToString());
});

builder.Services.AddScoped<NoticiasService>();

builder.Services.AddHttpClient<DeepSeekService>(client =>
{
    client.BaseAddress = new Uri(apiUrl?.ToString());
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});


var app = builder.Build();

app.UseRouting();
app.UseEndpoints(endpoints => {
    _ = endpoints.MapControllers();
});

app.Run();
