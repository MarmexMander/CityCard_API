using CityCard_API.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using CityCard_API.Models.DB;
using CityCard_API.Services;
using System.Reflection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>{
    // c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CityCard API", Version = "v1.3" });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
builder.Services.AddHostedService<TokenCleanupService>();
builder.Services.AddDbContext<CityCardDBContext>(b =>
{
    string user = Environment.GetEnvironmentVariable("POSTGRES_USER");
    string pwd = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
    string db = Environment.GetEnvironmentVariable("POSTGRES_DB");
    b.UseNpgsql($"Server=db;Port=5432;Database={db};User Id={user};Password={pwd};");
});
builder.Services.AddDefaultIdentity<CCUser>().AddEntityFrameworkStores<CityCardDBContext>().AddDefaultTokenProviders();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.AddScheme<TerminalTokenAuthenticationHandler>("TerminalToken", "Terminal Token");
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your-secret-key")),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("TerminalPolicy", policy =>
        policy.RequireAuthenticatedUser().AddAuthenticationSchemes("TerminalToken"));
});



var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseAuthentication();
app.MapIdentityApi<CCUser>().WithOpenApi();
app.MapSwagger().RequireAuthorization("Admin");
app.MapControllers();

// app.MapAreaControllerRoute("User", "User", "User/{controller=Api}/{action}").RequireAuthorization();
// app.MapAreaControllerRoute("Admin", "Admin", "Admin/{controller=Api}/{action}").RequireAuthorization("Admin");
// app.MapAreaControllerRoute("Terminal", "Terminal", "Terminal/{controller=Api}/{action}").RequireAuthorization(policy: "TerminalPolicy");

// var summaries = new[]
// {
//     "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
// };

// app.MapGet("/weatherforecast", () =>
// {
//     var forecast =  Enumerable.Range(1, 5).Select(index =>
//         new WeatherForecast
//         (
//             DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//             Random.Shared.Next(-20, 55),
//             summaries[Random.Shared.Next(summaries.Length)]
//         ))
//         .ToArray();
//     return forecast;
// })
// .WithName("GetWeatherForecast")
// .WithOpenApi();

app.Run();

// record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
// {
//     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// }
