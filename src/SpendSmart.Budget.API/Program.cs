using System.Text;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Extensions.Http;
using SpendSmart.Budget.API.Application.Services;
using SpendSmart.Budget.API.Infrastructure.BackgroundServices;
using SpendSmart.Budget.API.Infrastructure.Data;
using SpendSmart.Budget.API.Infrastructure.HttpClients;
using SpendSmart.Budget.API.Infrastructure.Messaging;
using SpendSmart.Budget.API.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var allowedOrigins = builder.Configuration["Cors:AllowedOrigins"]?.Split(",") 
    ?? new[] { "http://localhost:4200" };
 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SpendSmart Budget API",
        Version = "v1",
        Description = "Budget management microservice for SpendSmart platform"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Enter your JWT token here. Example: Bearer your-token"
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
                }
            },
            []
        }
    });
});

builder.Services.AddDbContext<BudgetDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IBudgetPlanRepository, BudgetPlanRepository>();
builder.Services.AddScoped<IBudgetPlanService, BudgetPlanService>();

builder.Services.AddHttpClient<INotificationHttpClient, NotificationHttpClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["NotificationApi:BaseUrl"] ?? "http://localhost:5007");
    client.Timeout = TimeSpan.FromSeconds(10);
})
.AddPolicyHandler(GetRetryPolicy());

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<BudgetCheckConsumer>();
    x.UsingAzureServiceBus((context, cfg) =>
    {
        cfg.Host(builder.Configuration["ServiceBus:ConnectionString"]);
        cfg.ReceiveEndpoint("budget-check-queue", e =>
        {
            e.ConfigureConsumer<BudgetCheckConsumer>(context);
        });
        cfg.ConfigureEndpoints(context);
    });
});


builder.Services.AddHostedService<BudgetResetService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.Headers.Append("Token-Expired", "true");
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BudgetDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.UseCors("AllowFrontend");

await app.RunAsync();