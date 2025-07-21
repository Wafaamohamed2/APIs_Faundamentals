
using APIs_Faundamentals.Models;
using APIs_Faundamentals.Repository;
using Microsoft.EntityFrameworkCore;
using APIs_Faundamentals.UnitOfWork;
using Microsoft.IdentityModel.Tokens;
using System.Text;
namespace APIs_Faundamentals
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            string text = "";
            // Add services to the container.

            

            builder.Services.AddControllers();


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
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


            // allow CORS for all origins, methods, and headers for  Ajex call
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder => builder.AllowAnyOrigin()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader());
            });

            // Register the EmployeeRepos service for dependency injection
           // builder.Services.AddScoped<Repository.EmployeeRepos>();

            // Register the EmployeeRepos service with the interface IEmployeeRepos for dependency injection
            //builder.Services.AddScoped<IEmployeeRepos,EmployeeRepos>();


            // Register the GenericRepos service for dependency injection
            //builder.Services.AddScoped<GenericRepos<Employee>>();
            //builder.Services.AddScoped<GenericRepos<Department>>();


            // Register the UnitWork service for dependency injection
            builder.Services.AddScoped<UnitWork>();

            builder.Services.AddAuthentication(op => op.DefaultAuthenticateScheme= "MyScheme")
                .AddJwtBearer
                ("MyScheme", option =>
                {
                    string secretkey = "Welcome to my app wish you interest with us";
                    var Key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretkey));

                    option.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = Key,
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };


                }) ;

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
               app.UseSwagger();

               //  app.MapSwagger().RequireAuthorization(op => op.RequireRole("admin"));   used for require authorization for Swagger UI
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseCors(text);
            app.MapControllers();

            app.Run();
        }
    }
}
