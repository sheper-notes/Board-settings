
using Common.Interfaces;
using Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<Database>(options =>
                options.UseSqlite("Data Source=Application.db;"));
            builder.Services.AddScoped<IUserQueries, UserQueries>();
            builder.Services.AddScoped<IBoardQueries, BoardQueries>();

            var domain = $"https://{builder.Configuration["Auth0:Domain"]}/";
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
             {
                 options.Authority = domain;
             });

            var app = builder.Build();
            
            var scope = app.Services.CreateScope();
            scope.ServiceProvider.GetRequiredService<Database>().Database.EnsureCreated();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors(x => x
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithOrigins("http://localhost:5173", "https://sheper.eu.auth0.com")
                    .AllowCredentials()
                    );

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();
            app.Use((context, next) => { context.Request.Scheme = "https"; return next(); });


            app.MapControllers();

            app.Run();
        }
    }
}