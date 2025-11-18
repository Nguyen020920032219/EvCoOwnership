var builder = WebApplication.CreateBuilder(args);

// Đọc config ReverseProxy từ appsettings.json
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.MapReverseProxy();

app.Run();