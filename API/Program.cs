using Common;
using Common.Interfaces;
using Data;
using IdGen.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var databaseConn = builder.Configuration.GetValue<string>("databaseConn");
            var authURL = builder.Configuration.GetValue<string>("authURL");
            var authSecret = builder.Configuration.GetValue<string>("authSecret");

            builder.Services.AddMemoryCache();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<Database>(options =>
                options.UseNpgsql(databaseConn));
            builder.Services.AddScoped<IUserQueries, UserQueries>();
            builder.Services.AddScoped<IBoardQueries, BoardQueries>();
            builder.Services.AddSingleton<Auth0AccessTokenManager>();
            builder.Services.AddLogging();
            builder.Services.AddIdGen(123);

            var app = builder.Build();
            
            var scope = app.Services.CreateScope();
            scope.ServiceProvider.GetRequiredService<Database>().Database.EnsureCreated();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
 

            //app.UseHttpsRedirection();

            app.Use((context, next) => { context.Request.Scheme = "https"; return next(); });


            app.MapControllers();
            var cache = app.Services.GetService<IMemoryCache>();
            cache.Set("authURL", authURL);
            cache.Set("authSecret", authSecret);
            app.Run();



        }
    }
}
