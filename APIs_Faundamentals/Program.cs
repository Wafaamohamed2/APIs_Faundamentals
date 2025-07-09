
using Microsoft.EntityFrameworkCore;

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


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                //app.UseSwagger();
                app.MapSwagger().RequireAuthorization(op => op.RequireRole("admin"));
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
