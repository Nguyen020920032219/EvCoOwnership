using System.Text;
using FinanceService.Business.Services.Expenses;
using FinanceService.Business.Services.Funds;
using FinanceService.Business.Services.Invoices;
using FinanceService.Business.Services.Payments;
using FinanceService.Data.Configurations;
using FinanceService.Data.Repositories.Expenses;
using FinanceService.Data.Repositories.Funds;
using FinanceService.Data.Repositories.Invoices;
using FinanceService.Data.Repositories.Payments;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

// Namespaces cho Repo & Service

var builder = WebApplication.CreateBuilder(args);

// 1. DbContext
// Lưu ý: Đổi chuỗi kết nối thành FinanceDb
var connectionString = builder.Configuration.GetConnectionString("FinanceDb")
                       ??
                       "Server=localhost;Database=EvCoOwnership_FinanceDb;Trusted_Connection=True;TrustServerCertificate=True;";

builder.Services.AddDbContext<FinanceDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddHttpContextAccessor();

// 2. Đăng ký Repository (Layer Data)
builder.Services.AddScoped<IFundRepository, FundRepository>();
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IMaintenanceRepository, MaintenanceRepository>();

// 3. Đăng ký Service (Layer Business)
builder.Services.AddScoped<IFundService, FundService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IMaintenanceService, MaintenanceService>();

builder.Services.AddControllers();
builder.Services.AddHttpClient();
// 4. Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Finance Service API", Version = "v1" });

    // Cấu hình JWT cho Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// 5. JWT Authentication
var jwtSection = builder.Configuration.GetSection("Jwt");
var secret = jwtSection["Secret"] ?? "this-is-a-very-long-super-secret-key-123456"; // Fallback nếu chưa config
var issuer = jwtSection["Issuer"];
var audience = jwtSection["Audience"];

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = key
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();