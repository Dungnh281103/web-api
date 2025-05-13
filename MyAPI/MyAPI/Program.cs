using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyAPI.Data;
using MyAPI.Db;
using MyAPI.Dtos.Auth;
using MyAPI.Interface;
using MyAPI.Services;
using MyAPI.Services.Token;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IStoryRepository, StoryRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IChapterRepository, ChapterRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ISavedStoryRepository, SavedStoryRepository>();
builder.Services.AddScoped<IReadingHistoryRepository, ReadingHistoryRepository>();

//builder.Services.AddScoped<IFileService, FileService>();


builder.Services.AddScoped<TokenService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
var connectionString = builder.Configuration.GetConnectionString("MyDB");
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add JWT Settings
builder.Services.Configure<JwtTokenSettings>(
    builder.Configuration.GetSection("JwtTokenSettings"));

// Trong Program.cs, thêm các dịch vụ Identity
builder.Services.AddIdentity<AppUser, AppRole>(options => {
    // Cấu hình tùy chọn nếu cần
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = true;
})

.AddEntityFrameworkStores<MyDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Web App API", Version = "v1" });

    // Add security definition for JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Add security requirement for protected endpoints
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtTokenSettings").Get<JwtTokenSettings>();
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.Key))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Seed data in development environment
    // Trong phần seeding của Program.cs
    //using (var scope = app.Services.CreateScope())
    //{
    //    var services = scope.ServiceProvider;
    //    try
    //    {
    //        var logger = services.GetRequiredService<ILogger<Program>>();
    //        logger.LogInformation("Bắt đầu seed dữ liệu...");

    //        await DataSeeder.SeedData(services);

    //        logger.LogInformation("Seed dữ liệu thành công.");
    //    }
    //    catch (Exception ex)
    //    {
    //        var logger = services.GetRequiredService<ILogger<Program>>();
    //        logger.LogError(ex, "Lỗi khi seed dữ liệu.");
    //    }
    //}
}

app.UseHttpsRedirection();

app.UseCors(builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
});

app.UseStaticFiles();

// Add Authentication middleware before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

