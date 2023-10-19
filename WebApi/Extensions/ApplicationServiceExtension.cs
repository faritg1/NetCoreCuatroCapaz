using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreRateLimit;

namespace WebApi.Extensions;
public static class ApplicationServiceExtension
{
    public static void ConfigureCors(this IServiceCollection services) => 
        services.AddCors(options => {
            options.AddPolicy("CorsPolicy", builder => 
            builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            );
    });
    public static void ConfigureCRatelimiting(this IServiceCollection services){
        services.AddMemoryCache();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        services.AddInMemoryRateLimiting();
        services.Configure<IpRateLimitOptions>(op => {
            op.EnableEndpointRateLimiting = true;
            op.StackBlockedRequests = false;
            op.HttpStatusCode = 429;
            op.RealIpHeader = "X-Real-IP";
            op.GeneralRules = new List<RateLimitRule>{
                new RateLimitRule{
                    Endpoint = "*",
                    Period = "10s",
                    Limit = 2
                }
            };
        });
    }

    public static void AddAplicationServices(this IServiceCollection services){
            //services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}
