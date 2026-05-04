using MaisonBean.Application.Auth;
using MaisonBean.Application.Auth.Commands;
using MaisonBean.Application.Common;
using MaisonBean.Application.Interfaces;
using MaisonBean.Application.Orders.Commands;
using MaisonBean.Application.Products.Commands;
using MaisonBean.Domain.Entities;
using MaisonBean.Infrastructure;
using MaisonBean.Infrastructure.Persistence;
using MaisonBean.Infrastructure.Persistence.Repositories;
using MaisonBean.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(PlaceOrderHandler).Assembly));

var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>()!;


JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),

        NameClaimType = ClaimTypes.NameIdentifier,
        RoleClaimType = ClaimTypes.Role,

        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.SetIsOriginAllowed(origin =>
        {
            var uri = new Uri(origin);
            return uri.Host == "localhost" || uri.Host == "127.0.0.1";
        })
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();


builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your JWT token"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddSwaggerGen(c =>
{
    c.UseInlineDefinitionsForEnums();
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();

    await DbSeeder.SeedAdminAsync(userManager, roleManager);
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseMiddleware<TokenVersionMiddleware>();
app.UseAuthorization();
app.UseMiddleware<BlockedUserMiddleware>();

app.MapControllers();

app.Run();