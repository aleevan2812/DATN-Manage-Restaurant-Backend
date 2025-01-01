using System.Text;
using API;
using Application;
using Infrastructure;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebServices();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(p =>
    p.AddPolicy("corsapp", builder =>
    {
        builder.WithOrigins("http://localhost:3000", "http://192.168.1.6:3000", "http://192.168.130.229:3000",
                "https://5c32-113-23-103-96.ngrok-free.app")
            .AllowCredentials().AllowAnyMethod().AllowAnyHeader();
        // builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
    }));

builder.Services
    // Thêm dịch vụ xác thực vào ứng dụng và chỉ định rằng sẽ sử dụng xác thực JWT Bearer.
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("Token:AccessTokenSecret"))) // Đổi thành Secret Key của bạn
        };
        // Đặt địa chỉ authority (URL của nhà cung cấp định danh)

        // Tắt yêu cầu phải sử dụng HTTPS cho metadata. Điều này có thể hữu ích trong quá trình phát triển và kiểm thử, nhưng không nên sử dụng trong môi trường sản xuất.
        options.RequireHttpsMetadata = false;
    });

var app = builder.Build();

app.MapHub<NotificationHub>("/notifications");

app.UseExceptionHandler();

// app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("corsapp");

app.MapControllers();

app.UseAuthentication(); // Thêm middleware Authentication
app.UseAuthorization(); // Thêm middleware Authorization

app.Run();