namespace VaporWebAPI;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication;

using AspNetCoreRateLimit;

using System.Security.Claims;
using System.Text;

using VaporWebAPI.Services;
using VaporWebAPI.Data;
using VaporWebAPI.Utils;
using VaporWebAPI.Services.Interfaces;


/// <summary>
/// The main entry point for the Vapor Web API application.
/// Configures services, middleware, authentication, and runs the web host.
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<VaporDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                        .EnableSensitiveDataLogging()      
                        .LogTo(Console.WriteLine, LogLevel.Information); 

        });

        builder.Services.Configure<StripeSettings>(
            builder.Configuration.GetSection("Stripe"));


        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: MyAllowSpecificOrigins,
                policy =>
                {
                    policy.WithOrigins(
                        "https://localhost:3000",
                        "https://localhost:7003",
                        "https://accounts.google.com"
                    )
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
                });
        });

        //builder.Services.AddMemoryCache();
        //builder.Services.Configure<IpRateLimitOptions>(options =>
        //{
        //    options.GeneralRules = new List<RateLimitRule>
        //    {
        //        new RateLimitRule
        //        {
        //            Endpoint = "*",
        //            Limit = 100,
        //            Period = "10m"
        //        }
        //    };
        //});

        builder.Services.AddHttpClient();
        builder.Services.AddScoped<JsonCleaningService>();
        builder.Services.AddScoped<AppImportService>();
        builder.Services.AddScoped<CartService>();
        builder.Services.AddScoped<AuthService>();
        builder.Services.AddScoped<GoogleAuthService>();
        builder.Services.AddScoped<AppService>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<StripeService>();
        builder.Services.AddScoped<PostService>();
        builder.Services.AddScoped<AdminService>();
        builder.Services.AddScoped<WishlistService>();
        builder.Services.AddScoped<NotificationService>();
        builder.Services.AddScoped<IEmailSender, SmtpEmailSenderService>();


        builder.Services.AddHttpClient<YouTubeService>();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Enter 'Bearer' followed by your token",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

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
                    new string[] {}
                }
            });
        });

        builder.Logging.AddConsole();
        var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:Secret"]);
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
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                ValidAudience = builder.Configuration["JwtSettings:Audience"],
                ClockSkew = TimeSpan.Zero
            };
        })
        .AddCookie("External")
        .AddGoogle("Google", options =>
        {
            options.SignInScheme = "External";
            options.ClientId = builder.Configuration["GoogleAuth:ClientId"];
            options.ClientSecret = builder.Configuration["GoogleAuth:ClientSecret"];
            options.CallbackPath = new PathString("/signin-google");
            options.SaveTokens = true;
            options.Scope.Add("email");
            options.Scope.Add("profile");
            options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
            options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
        });

        var app = builder.Build();
        app.UseCors(MyAllowSpecificOrigins);

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.ConfigObject.AdditionalItems["useCookies"] = "true";
            });
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.Run();
    }
}
