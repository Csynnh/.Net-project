using api.Middleware;
using infrastructure;
using infrastructure.Repositories;
using service;
using infrastructure.MigrationRunner;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddNpgsqlDataSource(Utilities.ProperlyFormattedConnectionString,
        dataSourceBuilder => dataSourceBuilder.EnableParameterLogging());
}

if (builder.Environment.IsProduction())
{
    builder.Services.AddNpgsqlDataSource(Utilities.ProperlyFormattedConnectionString);
}

builder.Services.AddSingleton<BookRepository>();
builder.Services.AddSingleton<BookService>();
builder.Services.AddSingleton<AccountRepository>();
builder.Services.AddSingleton<AccountService>();
builder.Services.AddSingleton<CustomerReviewRepository>();
builder.Services.AddSingleton<CustomerReviewService>();
builder.Services.AddSingleton<InvoiceRepository>();
builder.Services.AddSingleton<InvoiceService>();
builder.Services.AddSingleton<ProductRepository>();
builder.Services.AddSingleton<ProductService>();
builder.Services.AddSingleton<InvoiceDetailRepository>();
builder.Services.AddSingleton<InvoiceDetailService>();

builder.Services.AddSingleton<MigrationRunner>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Run migrations
var connectionString = Utilities.ProperlyFormattedConnectionString;
if (connectionString is not null) {
    Console.WriteLine(connectionString);
    var migrationRunner = app.Services.GetRequiredService<MigrationRunner>();
    await migrationRunner.ApplyMigrationsAsync();

}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(options =>
{
    options.SetIsOriginAllowed(origin => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
});



app.MapControllers();
app.UseMiddleware<GlobalExceptionHandler>();
app.Run();
