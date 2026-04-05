using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Metrics;
using StackExchange.Redis;
using UserService.API.Exceptions;
using UserService.Application.Interfaces.Cachings;
using UserService.Application.Interfaces.Messages;
using UserService.Application.Interfaces.Repositories;
using UserService.Application.Interfaces.Services;
using UserService.Application.Options;
using UserService.Application.Services;
using UserService.Domain.Entities;
using UserService.Domain.Events;
using UserService.Infrastructure.Caching;
using UserService.Infrastructure.Contexts;
using UserService.Infrastructure.Kafka;
using UserService.Infrastructure.Options;
using UserService.Infrastructure.Repositories;
using AuthenticationService = UserService.Application.Services.AuthenticationService;

namespace UserService.API.Extensions;

public static class ProgramExtension
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddAppInfrastructure(IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("Postgres")));
        
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
                options.InstanceName = "micro_user";
            });

            services.AddSingleton<IConnectionMultiplexer>(sp => 
                ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis")!));
        
            services.AddScoped<IDatabase>(sp =>
                sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase());

            return services;
        }

        public IServiceCollection AddBusinessServices(IConfiguration configuration)
        {
            services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
            
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            services.AddScoped<IHashCachingService, HashCachingService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();

            services.AddProducer<PersonCreateEvent>(configuration.GetRequiredSection("Kafka:PersonCreated"));
            services.AddProducer<PersonDeleteEvent>(configuration.GetRequiredSection("Kafka:PersonDeleted"));
            services.AddProducer<PersonUpdateEvent>(configuration.GetRequiredSection("Kafka:PersonUpdated"));
            
            return services;
        }

        public IServiceCollection AddSecurityConfiguration(IConfigurationSection jwtConfig)
        {
            services.AddIdentity<Person, IdentityRole>(options =>
            {   
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
            }).AddEntityFrameworkStores<AppDbContext>();

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtConfig["Issuer"],
                        ValidateAudience = true,
                        ValidAudience = jwtConfig["Audience"],
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Secret"]!)),
                        ValidateIssuerSigningKey = true
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            context.Token = context.Request.Cookies["jwt"];
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization();
            return services;
        }
        
        public IServiceCollection AddAppTelemetry()
        {
            services.AddOpenTelemetry()
                .WithMetrics(metrics => metrics
                    .AddAspNetCoreInstrumentation()
                    .AddPrometheusExporter());

            return services;
        }

        public IServiceCollection AddWebPresentation(IConfiguration configuration)
        {
            services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddProblemDetails();
            services.AddExceptionHandler<ExceptionHandler>();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyMethod()
                    .AllowAnyHeader());
                options.AddPolicy(
                    "YarpPolice", 
                    policy => policy
                        .WithOrigins(configuration.GetConnectionString("Yarp") ?? string.Empty)
                        .AllowAnyMethod().AllowAnyHeader());
            });

            return services;
        }
        
        private void AddProducer<TMessage>(IConfigurationSection configuration)
        {
            services.Configure<KafkaProducerOptions>(typeof(TMessage).Name, configuration);
            services.AddSingleton<IMessageProducer<TMessage>, KafkaProducer<TMessage>>();
        }

        private void AddConsumer<TMessage, THandler>(IConfigurationSection configuration)
            where THandler : class, IMessageHandler<TMessage>
        {
            services.Configure<KafkaConsumerOptions>(typeof(TMessage).Name, configuration);
            services.AddHostedService<KafkaConsumer<TMessage>>();
            services.AddSingleton<IMessageHandler<TMessage>, THandler>();
        }
    }
}