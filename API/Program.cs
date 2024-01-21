using Common;
using Common.Interfaces;
using Common.Models;
using Data;
using Ganss.Xss;
using IdGen.DependencyInjection;
using Jaeger.Reporters;
using Logproto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTracing;
using Prometheus;
using System.Diagnostics;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Text;

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
            databaseConn = "Host=localhost:5432;Database=shepe;Username=postgres;Password=postgres";

            builder.Services.AddOpenTelemetry().WithTracing(x =>
                x.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(builder.Environment.ApplicationName))
                .AddAspNetCoreInstrumentation(x =>
                {
                    x.EnrichWithHttpRequest = async (activity, httpWebRequest) =>
                    {
                        StringBuilder headersString = new StringBuilder();

                        foreach (var (key, value) in httpWebRequest.Headers)
                        {
                            headersString.Append($"{key}: {string.Join(",", value)}\n");
                        }

                        activity.SetTag("Headers", headersString);

                        if(httpWebRequest.Body.CanSeek == true)
                        {
                            StreamReader reader = new StreamReader(httpWebRequest.Body, Encoding.UTF8);

                            activity.SetTag("Body", await reader.ReadToEndAsync());
                        }

                    };
                })
                .AddHttpClientInstrumentation()
                .AddOtlpExporter(opts => opts.Endpoint = new Uri("http://localhost:4317"))
            );
            builder.Services.AddTransient(provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                const string categoryName = "Any";
                return loggerFactory.CreateLogger(categoryName);
            });
            builder.Services.AddSingleton(TracerProvider.Default.GetTracer(builder.Environment.ApplicationName));
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });

            builder.Services.AddMemoryCache();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<Database>(options =>
                options.UseNpgsql(databaseConn));
            builder.Services.AddScoped<IUserQueries, UserQueries>();
            builder.Services.AddScoped<IBoardQueries, BoardQueries>();
            builder.Services.AddScoped<IBoardDBService,BoardDBService>();
            builder.Services.AddSingleton<Auth0AccessTokenManager>();
            builder.Services.AddIdGen(123);
            builder.Services.AddScoped<IUserInfoUtil, UserInfoUtil>();
            var conf = builder.Configuration.AddEnvironmentVariables();
            builder.Services.AddSingleton<IConfiguration>(conf.Build());
            var app = builder.Build();
            
            var scope = app.Services.CreateScope();
            scope.ServiceProvider.GetRequiredService<Database>().Database.EnsureCreated();
            var db = scope.ServiceProvider.GetRequiredService<Database>();



            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseRouting();

            //app.UseHttpsRedirection();

            app.Use((context, next) => { context.Request.Scheme = "https"; return next(); });
            app.UseCors("AllowAll");

            var cache = app.Services.GetService<IMemoryCache>();
            cache.Set("authURL", authURL);
            cache.Set("authSecret", authSecret);
            app.UseHttpMetrics();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapMetrics();
            });
            app.Run();
        }
    }
}
