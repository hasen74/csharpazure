global using Microsoft.EntityFrameworkCore;
global using System.Text;
global using csharp.Data;
global using csharp.Models;
global using csharp.Dtos.User;
global using csharp.Services.UserService;
global using csharp.Services.AuthService;
global using csharp.Policies;
global using AutoMapper;
global using Azure.Identity;
global using Microsoft.IdentityModel.Tokens;
global using Azure.Security.KeyVault.Secrets;
global using Microsoft.AspNetCore.Authorization;

using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

var credential = new DefaultAzureCredential();
var client = new SecretClient(new Uri("https://ticwin1-vault.vault.azure.net/"), credential);
KeyVaultSecret secret = await client.GetSecretAsync("TIC-WIN1-KEY");
string secretKey = secret.Value;

// Add services to the container.
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddAuthentication().AddJwtBearer(options => {
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateAudience = false,
        ValidateIssuer = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});
builder.Services.AddSingleton<IAuthorizationHandler, SamePersonOrAdminHandler>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SamePersonOrAdminPolicy", policy => policy.Requirements.Add(new SamePersonOrAdminRequirement()));
});

var app = builder.Build();
app.UseSwagger();
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();

app.Run();
