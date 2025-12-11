using Confluent.Kafka;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Reflection;
using System.Text;
using Techcore_Internship.Data.Authorization.Policies;
using Techcore_Internship.Data.Authorization.Reqirements;

namespace Techcore_Internship.Data.Utils.Extentions;

public static class ServiceRegistrationExtentions
{
    public static IServiceCollection AddCustomSwaggerWithJwt(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Techcore Internship API",
                Version = "v1",
                Description = "API для стажировки в Techcore"
            });

            c.AddServer(new OpenApiServer { Url = "/" });
            c.AddServer(new OpenApiServer { Url = "http://localhost:5001" });
            c.AddServer(new OpenApiServer { Url = "http://webapi:8080" });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
                c.IncludeXmlComments(xmlPath);

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Введите токен в формате: Bearer {токен}"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                }
            });
        });

        return services;
    }

    public static IServiceCollection AddCustomRedis(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetConnectionString("Redis");
            options.InstanceName = builder.Configuration["RedisSettings:InstanceName"];
        });

        return services;
    }

    public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = false,
                ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                ValidAudience = builder.Configuration["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!))
            };
        });

        return services;
    }

    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(AgePolicies.OLDER_THEN_18, policy =>
            policy.Requirements.Add(new MinimumAgeRequirement(18)));

            options.AddPolicy(AgePolicies.OLDER_THEN_21, policy =>
                policy.Requirements.Add(new MinimumAgeRequirement(21)));
        });

        return services;
    }

    public static IServiceCollection AddCustomMassTransit(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                var configuration = context.GetService<IConfiguration>();
                var host = configuration?["RabbitMQ:Host"] ?? "localhost";

                cfg.Host(host, "/", h =>
                {
                    h.Username("Cruciatus");
                    h.Password("12345qwerty");
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    public static IServiceCollection AddKafka(this IServiceCollection services)
    {
        services.AddSingleton<IProducer<string, string>>(serviceProvider =>
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "kafka:9092",
                MessageTimeoutMs = 5000,
                RequestTimeoutMs = 5000,
                EnableDeliveryReports = true,
                Acks = Acks.All
            };

            return new ProducerBuilder<string, string>(config).Build();
        });

        return services;
    }

    public static IServiceCollection AddCustomOpenTelemetry(this IServiceCollection services, IConfiguration configuration = null)
    {
        var serviceProvider = services.BuildServiceProvider();
        var config = configuration ?? serviceProvider.GetService<IConfiguration>();
        var env = serviceProvider.GetService<IWebHostEnvironment>();

        var serviceName = config?["OpenTelemetry:ServiceName"] ?? env?.ApplicationName ?? "UnknownService";
        var serviceVersion = config?["OpenTelemetry:ServiceVersion"] ?? "1.0.0";
        var zipkinEndpoint = config?["Zipkin:Endpoint"] ?? "http://zipkin:9411/api/v2/spans";

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(serviceName: serviceName, serviceVersion: serviceVersion)
                .AddAttributes(new Dictionary<string, object>
                {
                    ["deployment.environment"] = env?.EnvironmentName.ToLowerInvariant() ?? "development",
                }))
            .WithTracing(tracing =>
            {
                tracing
                    .AddSource(serviceName)
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;
                        options.EnrichWithHttpRequest = (activity, request) =>
                        {
                            activity.SetTag("http.client_ip", request.HttpContext.Connection.RemoteIpAddress?.ToString());
                        };
                    })
                    .AddHttpClientInstrumentation(options =>
                    {
                        options.RecordException = true;
                    })
                    .AddEntityFrameworkCoreInstrumentation(options =>
                    {
                        options.SetDbStatementForText = true;
                    })
                    .AddZipkinExporter(zipkinOptions =>
                    {
                        zipkinOptions.Endpoint = new Uri(zipkinEndpoint);
                    });
            });

        return services;
    }
}
