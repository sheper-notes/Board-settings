using Common;
using Common.Interfaces;
using Common.Models;
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
            var db = scope.ServiceProvider.GetRequiredService<Database>();
            if(db.Boards.Where(x => x.Id == 1).Count() == 0)
            {
                var board = new Common.Models.Board()
                {
                    Id = 1,
                    Name = "test",
                    SubscriptionType = Common.Enums.SubscriptionType.Standard,
                    Users = new List<Common.Models.UserRoleRelation>()
                };

                board.Users.Add(new Common.Models.UserRoleRelation() { UserId = "testID", Role = Common.Enums.Role.Owner });
                db.Boards.Add(board);
                db.SaveChanges();
            }
            
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
