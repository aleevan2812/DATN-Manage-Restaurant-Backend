using System.Text;
using API;
using Application;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebServices();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(p =>
    p.AddPolicy("corsapp", builder => { builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader(); }));

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
                    Encoding.UTF8.GetBytes("hoc_lap_trinh_edu_duthanhduoc_com_access")) // Đổi thành Secret Key của bạn
        };
        // Đặt địa chỉ authority (URL của nhà cung cấp định danh)

        // Tắt yêu cầu phải sử dụng HTTPS cho metadata. Điều này có thể hữu ích trong quá trình phát triển và kiểm thử, nhưng không nên sử dụng trong môi trường sản xuất.
        options.RequireHttpsMetadata = false;
    });

var app = builder.Build();

app.UseExceptionHandler();

app.UseCors("AllowAll");

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