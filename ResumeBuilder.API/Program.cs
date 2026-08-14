using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using ResumeBuilder.API.Data;
using ResumeBuilder.API.Helpers;
using ResumeBuilder.API.Models.JWT.Models;
using ResumeBuilder.API.Repositories;
using ResumeBuilder.API.Repositories.Interfaces;
using ResumeBuilder.API.Services;
using ResumeBuilder.API.Services.Interfaces;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// 🟢 CRITICAL ADDITION: This maps your Controller Actions into Metadata explorer endpoints
builder.Services.AddEndpointsApiExplorer();

// 1. Add the native Microsoft OpenAPI services with Scalar's custom transformers
builder.Services.AddOpenApi(options =>
{
    options.AddScalarTransformers(); // Injected from Scalar.AspNetCore.Microsoft
});


builder.Services.AddDbContext<ResumeBuilderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IResumebuilderServices, ResumebuilderServices>();

//

// *********************** New Service Code Below ***********************

//

//builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

//Register the JWT settings and service:

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddScoped<IJwtService, JwtService>();

// Configure authentication:
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;

    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,

        ValidateAudience = true,

        ValidateLifetime = true,

        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],

        ValidAudience = builder.Configuration["Jwt:Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
/*
// Swagger JWT Configuration : Replace the existing AddSwaggerGen()

using Microsoft.OpenApi.Models;

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "Resume Builder API",
            Version = "v1"
        });

    options.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",

            Type = SecuritySchemeType.Http,

            Scheme = "bearer",

            BearerFormat = "JWT",

            In = ParameterLocation.Header,

            Description = "Enter JWT Token"
        });

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference =
                        new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,

                            Id = "Bearer"
                        }
                },

                Array.Empty<string>()
            }
        });
});
*/
// ... (keep the rest of your app building/mapping logic identical)

//*********************** Add services to the container end.***********************
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Map the document route (/openapi/v1.json)
    app.MapOpenApi();

    // Map the interactive browser UI (/scalar/v1)
    //app.MapScalarApiReference();
    // 2. Point Scalar exactly to that JSON file 
    app.MapScalarApiReference(options =>
    {
        // This links Scalar directly to the file mapped above
        options.WithOpenApiRoutePattern("/openapi/v1.json");
    });

    // app.UseSwagger();
    // app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();