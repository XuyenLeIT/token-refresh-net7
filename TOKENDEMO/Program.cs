using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TOKENDEMO.Models;
using TOKENDEMO.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("Jwt")["Key"]);
builder.Services.AddAuthentication(x =>
{
    //Đặt scheme mặc định cho xác thực và challenge là 
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    //Tắt yêu cầu sử dụng HTTPS
    //cho mục đích phát triển (nên bật lại trong môi trường production).
    x.RequireHttpsMetadata = false;
    // lưu trữ token JWT trong các thuộc tính AuthenticationProperties
    // sau khi xác thực thành công.
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        //xác thực khóa ký của token.
        ValidateIssuerSigningKey = true,
        // thiết lập khóa bí mật để xác thực chữ ký token
        IssuerSigningKey = new SymmetricSecurityKey(key),
        // tắt xác thực nhà phát hành (issuer) của token.
        ValidateIssuer = false,
        //tắt xác thực đối tượng nhận (audience) của token.
        ValidateAudience = false,
        //Bật xác thực thời gian sống của token.
        ValidateLifetime = true,
        //thiết lập một khoảng thời gian chênh lệch cho phép
        //giữa thời gian của máy chủ và thời gian của token.
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectDB"));
});
builder.Services.AddScoped<IUserRepository, UserRepository>();
var app = builder.Build();
app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

;app.MapControllers();

app.Run();
