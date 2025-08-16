
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
namespace APIs_Faundamentals
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            string text = "";
          


            builder.Services.AddControllers();

            // Register the UnitWork service for dependency injection
            builder.Services.AddScoped<UnitWork>();

            builder.Services.AddScoped<IAuthoServ, AuthoSirv>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();


            // Configure Serilog for logging and Application Insights for telemetry
            builder.Services.AddSwaggerGen(
                opt => {
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


            
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<Models.PracticContext>()
                .AddDefaultTokenProviders();



            // allow CORS for all origins, methods, and headers for  Ajex call
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder => builder.AllowAnyOrigin()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader());
            });


         
            


            
            builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));


            // Configure Application Insights for telemetry 
            builder.Services.AddAuthentication(op => op.DefaultAuthenticateScheme= "MyScheme")
                .AddJwtBearer
                ("MyScheme", option =>
                {
                    //string secretkey = "Welcome to my app wish you interest with us";
                    //var Key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretkey));

                    //option.TokenValidationParameters = new TokenValidationParameters
                    //{
                    //    IssuerSigningKey = Key,
                    //    ValidateIssuer = false,
                    //    ValidateAudience = false
                    //};

                    option.RequireHttpsMetadata = false; // Disable HTTPS metadata requirement for development purposes
                    option.SaveToken = true; // Save the token in the authentication properties
                    option.TokenValidationParameters = new TokenValidationParameters
                    {

                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["JWT:Issuer"], // Set the valid issuer from configuration
                        ValidAudience = builder.Configuration["JWT:Audience"], // Set the valid audience from configuration
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])), // Set the signing key from configuration
                        ClockSkew = TimeSpan.Zero // Disable clock skew for token expiration validation
                    };

                }) ;





            builder.Services.AddProblemDetails();



            var app = builder.Build();

            if(app.Environment.IsDevelopment())
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

            app.UseCors(text);

            app.UseHttpsRedirection();


            app.UseAuthentication();
            app.UseAuthorization();

          
            app.MapControllers();

            app.Run();
        }
    }
}
