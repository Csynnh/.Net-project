using api.Middleware;
using infrastructure;
using infrastructure.Repositories;
using service;
using infrastructure.MigrationRunner;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Access configuration from appsettings.json
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddNpgsqlDataSource(Utilities.ProperlyFormattedConnectionString,
    dataSourceBuilder => dataSourceBuilder.EnableParameterLogging());

if (builder.Environment.IsProduction())
{
    builder.Services.AddNpgsqlDataSource(Utilities.ProperlyFormattedConnectionString);
}

// Register repositories and services
builder.Services.AddSingleton<CustomerReviewRepository>();
builder.Services.AddSingleton<CustomerReviewService>();
builder.Services.AddSingleton<OderRepository>();
builder.Services.AddSingleton<OderService>();
builder.Services.AddSingleton<ProductRepository>();
builder.Services.AddSingleton<ProductService>();
builder.Services.AddSingleton<OderDetailRepository>();
builder.Services.AddSingleton<OderDetailService>();
builder.Services.AddSingleton<CartService>();
builder.Services.AddSingleton<CartRepository>();
builder.Services.AddSingleton<UserStoredInformationRepository>();
builder.Services.AddSingleton<UserStoredInformationService>();
builder.Services.AddSingleton<UserRepository>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<PaymentMethodRepository>();
builder.Services.AddSingleton<PaymentMethodService>();
builder.Services.AddSingleton<ShippingMethodRepository>();
builder.Services.AddSingleton<ShippingMethodService>();
builder.Services.AddSingleton<NotificationRepository>();
builder.Services.AddSingleton<NotificationService>();
builder.Services.AddSingleton<EmailService>();
builder.Services.AddSingleton<OtpService>();
builder.Services.AddSingleton<OtpRepository>();
builder.Services.AddSingleton<EmployeeService>();
builder.Services.AddSingleton<EmployeeRepository>();
builder.Services.AddSingleton<MigrationRunner>();
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Add JWT Bearer authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token with Bearer prefix",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
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
            new string[] {}
        }
    });
});
builder.Services.AddHttpContextAccessor();
// Configure JWT authentication
var jwtSettings = configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        Console.WriteLine(jwtSettings["Key"]);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notificationHub"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User", "Admin"));
});

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILogger<Program>>();

// Check for the --migrate-db argument
if (args.Contains("--migrate-db"))
{
    // Run migrations
    logger.LogInformation("Starting database migration...");
    var connectionString = Utilities.ProperlyFormattedConnectionString;
    if (connectionString is not null)
    {
        Console.WriteLine(connectionString);
        var migrationRunner = app.Services.GetRequiredService<MigrationRunner>();
        await migrationRunner.ApplyMigrationsAsync();
    }

    logger.LogInformation("Database migration completed.");
}

// Add SignalR to the middleware pipeline.
app.MapHub<NotificationHub>("/notificationHub");

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
});

// Enable CORS
app.UseCors(options =>
{
    options.SetIsOriginAllowed(origin => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
});

// Enable authentication and authorization
app.UseAuthentication(); // Add this line
app.UseAuthorization(); // Add this line

app.UseMiddleware<GlobalExceptionHandler>();

app.MapControllers();
app.Run();
