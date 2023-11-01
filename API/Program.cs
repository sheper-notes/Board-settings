
using Common.Interfaces;
using Data;
using Logic;
using Microsoft.EntityFrameworkCore;

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
            builder.Services.AddScoped<IUserLogic, UserLogic>();
            builder.Services.AddScoped<IUserQueries, UserQueries>();
            var app = builder.Build();
            
            var scope = app.Services.CreateScope();
            scope.ServiceProvider.GetRequiredService<Database>().Database.EnsureCreated();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
