using System.Text;
using ContractGroupService.Business.Services.Groups;
using ContractGroupService.Business.Services.Votes;
using ContractGroupService.Data.Configurations;
using ContractGroupService.Data.Repositories.Groups;
using ContractGroupService.Data.Repositories.Votes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
// Thêm namespace Repository và Service
// Namespace của GroupRepository

var builder = WebApplication.CreateBuilder(args);

// 1. DbContext (kết nối EvCoOwnership_ContractGroupDb)
var connectionString = builder.Configuration.GetConnectionString("ContractGroupDb");
builder.Services.AddDbContext<ContractGroupDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Đăng ký Services & Repositories (QUAN TRỌNG: Phải có cả 2 dòng này)
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IVoteRepository, VoteRepository>();

builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IVoteService, VoteService>();

builder.Services.AddControllers();

// 3. Swagger + cấu hình Bearer
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Swagger", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token in the textbox below."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// 4. JWT Auth (dùng chung secret với AuthService)
var jwtSection = builder.Configuration.GetSection("Jwt");
var secret = jwtSection["Secret"]!;
var issuer = jwtSection["Issuer"];
var audience = jwtSection["Audience"];

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

// 5. Middleware
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