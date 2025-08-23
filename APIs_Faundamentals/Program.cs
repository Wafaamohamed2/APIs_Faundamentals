
using APIs_Faundamentals.Models;
using APIs_Faundamentals.Repository;
using Microsoft.EntityFrameworkCore;
using APIs_Faundamentals.UnitOfWork;
using APIs_Faundamentals.Helper;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using Microsoft.ApplicationInsights.Extensibility;
using System.Configuration;
using Microsoft.AspNetCore.Identity;

using APIs_Faundamentals.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
namespace APIs_Faundamentals
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);



            builder.Services.AddControllers();

            // Register the UnitWork service for dependency injection
            builder.Services.AddScoped<UnitWork>();

            builder.Services.AddScoped<IAuthoServ, AuthoSirv>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();


            // Configure Serilog for logging and Application Insights for telemetry
            builder.Services.AddSwaggerGen(
                opt =>
                {
                    // to include XML comments in Swagger documentation to provide more details about the API endpoints
                    opt.IncludeXmlComments("F:\\VS_APIs\\APIs_Faundamentals\\APIs_Faundamentals\\Mydoc.xml");

                    opt.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
                    {
                        Version = "v1",
                        Title = "API Foundamentals",
                        Description = "api to manage employees with its department",
                        Contact = new Microsoft.OpenApi.Models.OpenApiContact
                        {

                            Email = "mohamedwafaa245@gmail.com"
                        }
                    }

                    );
                }
                );



            builder.Services.AddDbContext<Models.PracticContext>(options =>
                options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("Practcon")));


            // Identity Framework Core with SQL Server and Lazy Loading Proxies
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<Models.PracticContext>()
                .AddDefaultTokenProviders();



            // allow CORS for all origins, methods, and headers for  Ajex call
            builder.Services.AddCors(options =>
            {
                //options.AddPolicy("AllowAllOrigins",
                //    builder => builder.AllowAnyOrigin()
                //                      .AllowAnyMethod()
                //                      .AllowAnyHeader());


                options.AddPolicy("SecurePolicy",

                    builder => builder
                    .WithOrigins("https://localhost:7163")   // Specify the allowed origin(s) for CORS
                    .WithMethods("GET", "POST", "PUT", "DELETE")
                    .WithHeaders("Content-Type", "Authorization")
                    .AllowCredentials() // Allow credentials (cookies, authorization headers, etc.)
                );
            });



            builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));




            builder.Services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                      .RequireAuthenticatedUser()
                      .Build();


                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));

                options.AddPolicy("SuperAdminOnly", policy => policy.RequireClaim("SuperAdmin"));
                options.AddPolicy("ReadOnlyAccess", policy => policy.RequireRole("Admin", "HR", "User"));

                options.AddPolicy("CanManageEmployees", policy => policy.RequireRole("Admin", "HR"));
            });



            // Configure Application Insights for telemetry 
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })

            .AddJwtBearer
                (option =>
                {

                    option.RequireHttpsMetadata = true; // Require HTTPS for security
                    option.SaveToken = false; // Do not save the token in the authentication properties
                    option.TokenValidationParameters = new TokenValidationParameters
                    {

                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["JWT:Issuer"], // Set the valid issuer from configuration
                        ValidAudience = builder.Configuration["JWT:Audience"], // Set the valid audience from configuration
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])), // Set the signing key from configuration
                        RoleClaimType = ClaimTypes.Role,
                        NameClaimType = ClaimTypes.Name,

                        ClockSkew = TimeSpan.Zero,// Disable clock skew for token expiration validation
                        RequireExpirationTime = true,
                        RequireSignedTokens = true,

                    };
                    option.Events = new JwtBearerEvents
                    {


                        OnMessageReceived = context =>
                        {

                            var token = context.Request.Headers["Authorization"].ToString();

                            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                            logger.LogInformation("OnMessageReceived - Authorization header: {AuthHeader}", token?.Length > 100 ? token.Substring(0, 100) + "..." : token);
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {
                            // Log the authentication failure
                            Log.Error("Authentication failed: {Error}", context.Exception.Message);
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            var claimsIdentity = context.Principal.Identity as ClaimsIdentity;

                            if (claimsIdentity == null || !claimsIdentity.IsAuthenticated)
                            {
                                // Log the validation failure
                                Log.Warning("Token validation failed: Claims identity is null or not authenticated.");
                                context.Fail("Invalid token");
                                return Task.CompletedTask;
                            }



                            // Log successful token validation
                            Log.Information("Token validated successfully for user: {User}", context.Principal.Identity.Name);
                            return Task.CompletedTask;
                        },

                        OnChallenge = context =>
                        {
                            //  Custom challenge handling
                            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                            logger.LogWarning("JWT Challenge: {Error}, {ErrorDescription}",
                                context.Error, context.ErrorDescription);
                            return Task.CompletedTask;
                        }
                    };



                });



            builder.Services.AddRateLimiter(options =>
            {

                // Configure rate limiting policies for different scenarios
                options.AddFixedWindowLimiter("AuthPolicy", options =>
                {
                    options.Window = TimeSpan.FromMinutes(1); // Set the time window for rate limiting
                    options.PermitLimit = 10; // Allow 10 requests per window
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst; // Process oldest requests first
                    options.QueueLimit = 0;
                });

                // General rate limiting policy for all users
                options.AddFixedWindowLimiter("GeneralPolicy", options =>
                {
                    options.PermitLimit = 100; // 100 requests
                    options.Window = TimeSpan.FromMinutes(1); // per minute
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 0;

                });

                // Rate limiting policy for admin operations
                options.AddFixedWindowLimiter("AdminPolicy", options =>
                {
                    options.PermitLimit = 20; // 20 admin operations
                    options.Window = TimeSpan.FromMinutes(1); // per minute
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 0;
                });

                // Rate limiting policy for user operations
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
                    httpContext => RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.User?.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 200,
                            Window = TimeSpan.FromMinutes(1),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0
                        }
                    )
                );

                // Configure the options for rejected requests
                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests; // Set the status code for too many requests

                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    {
                        await context.HttpContext.Response.WriteAsync(
                            $"Too many requests. Please try again after {retryAfter.TotalMinutes} minute(s).",
                            cancellationToken: token);
                    }
                    else
                    {
                        await context.HttpContext.Response.WriteAsync(
                            "Too many requests. Please try again later.",
                            cancellationToken: token);
                    }
                };





            });



            builder.Services.AddProblemDetails();



            var app = builder.Build();


            await SeedRolesAndSuperAdminAsync(app);


            if (app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler();
            }


            app.UseForwardedHeaders();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            app.UseSwagger();

            //  app.MapSwagger().RequireAuthorization(op => op.RequireRole("admin"));   used for require authorization for Swagger UI
            app.UseSwaggerUI();
            //}

            app.UseRouting();
            app.UseCors("SecurePolicy");

            app.UseHttpsRedirection();


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRateLimiter();
            app.MapControllers();

             app.Run();
        }

        private static async Task SeedRolesAndSuperAdminAsync(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            string[] roles = { "SuperAdmin", "Admin", "HR", "User" };

            foreach (var role in roles) {

                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(role));
                    if (result.Succeeded)
                    {
                        logger.LogInformation($"Role '{role}' created successfully.");
                    }
                    else
                    {
                        logger.LogError($"Failed to create role '{role}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }

            }

            var superAdminEmail = "User2@gamil.com";
            var superAdminUser = await userManager.FindByEmailAsync(superAdminEmail);

            if (superAdminUser != null) { 
            
               
                if (!await userManager.IsInRoleAsync(superAdminUser, "SuperAdmin"))
                {
                    var result = await userManager.AddToRoleAsync(superAdminUser, "SuperAdmin");
                    if (result.Succeeded)
                    {
                        logger.LogInformation($"User '{superAdminEmail}' assigned to 'SuperAdmin' role successfully.");
                    }
                    else
                    {
                        logger.LogError($"Failed to assign 'SuperAdmin' role to user '{superAdminEmail}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }
            else
            {
                logger.LogWarning($"SuperAdmin user with email '{superAdminEmail}' not found. Please create the user first.");
            
            }


        }
    }

  }
