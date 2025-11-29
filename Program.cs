using AvyyanBackend.Data;
using AvyyanBackend.Extensions;
using AvyyanBackend.Middleware;
using AvyyanBackend.Services;
using AvyyanBackend.WebSockets;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Serilog;
using System.ComponentModel;
using System.Net.WebSockets;


ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;


// Configure Serilog early
var builder = WebApplication.CreateBuilder(args);
builder.ConfigureSerilog();

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // Use PascalCase for property names
    });

 

// Configure Kestrel server options for larger file uploads
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 52428800; // 50 MB
});

// Add custom services using extension methods
builder.Services.AddDatabaseServices(builder.Configuration);
builder.Services.AddRepositoryServices();
builder.Services.AddBusinessServices();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddValidationServices();
builder.Services.AddAutoMapperServices();
builder.Services.AddCorsServices();
builder.Services.AddSignalR();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new() { Title = "Avyyan Knitfab API", Version = "v1" });

	// Add JWT authentication to Swagger
	c.AddSecurityDefinition("Bearer", new()
	{
		Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
		Name = "Authorization",
		In = Microsoft.OpenApi.Models.ParameterLocation.Header,
		Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
		Scheme = "Bearer"
	});

	c.AddSecurityRequirement(new()
	{
		{
			new()
			{
				Reference = new()
				{
					Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			Array.Empty<string>()
		}
	});
});
builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "Avyyan Knitfab API V1");
		c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
	});
}
else
{
	// Use exception logging middleware in production
	app.UseExceptionLogging();
}

app.UseHttpsRedirection();

// Use Serilog request logging
app.UseSerilogRequestLogging();

// Use CORS (must be before UseAuthentication and UseAuthorization)
app.UseCors("AllowAll");

// Configure middleware to handle large file uploads
app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/fg-rolls"), appBuilder =>
{
    appBuilder.Use(async (context, next) =>
    {
        // Disable request size limit for fg-rolls endpoint
        context.Features.Get<IHttpMaxRequestBodySizeFeature>().MaxRequestBodySize = null;
        await next();
    });
});

// Use Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map SignalR hubs
app.MapHub<NotificationHub>("/notificationhub");
app.MapHub<ChatHub>("/chathub");
app.MapHub<WeightHub>("/weighthub"); // Add the WeightHub endpoint

app.MapControllers();

// Ensure database is created and seeded (for development)
if (app.Environment.IsDevelopment())
{
	using var scope = app.Services.CreateScope();
	var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
	// await context.Database.MigrateAsync();

	// Seed initial data
	//var dataSeedService = scope.ServiceProvider.GetRequiredService<DataSeedService>();
	//await dataSeedService.SeedAsync();
}

try
{
	Log.Information("Starting Avyyan Knitfab API");
	app.Run();
}
catch (Exception ex)
{
	Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
	Log.CloseAndFlush();
}