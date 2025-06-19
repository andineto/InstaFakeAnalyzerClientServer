using InstaFakeAnalyzer.Data;
using InstaFakeAnalyzer.Services;
using Microsoft.EntityFrameworkCore;

public class Program
{

    public static string connectionStringPg;
    public static string connectionStringMySql;
    public static string apiUrl;
    public static string baseUrl;
    public static ServiceProvider? ServiceProvider { get; private set; }
    private static void Main(string[] args)
    {
        connectionStringPg = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        connectionStringMySql = Environment.GetEnvironmentVariable("CONNECTION_STRING_MYSQL");
        apiUrl = Environment.GetEnvironmentVariable("ApiUrl");
        baseUrl = Environment.GetEnvironmentVariable("BaseUrl");


        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddEnvironmentVariables();

#if DEBUG
        connectionStringPg = builder.Configuration.GetConnectionString("DefaultConnection");
        connectionStringMySql = builder.Configuration.GetConnectionString("MySql");
        apiUrl = builder.Configuration["Urls:ApiUrl"];
#endif

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionStringPg));

        builder.Services.AddControllers();
        builder.Services.AddHttpClient<DeepSeekService>(client =>
        {
            client.BaseAddress = new Uri(apiUrl?.ToString());
        });

        builder.Services.AddScoped<Service>();

        builder.Services.AddHttpClient<GeminiService>(client =>
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
        app.UseEndpoints(endpoints =>
        {
            _ = endpoints.MapControllers();
        });

        app.Run();
    }
}

#if DEBUG

#endif
