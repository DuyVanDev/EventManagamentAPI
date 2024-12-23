using CloudinaryDotNet;
using EventManagament.Interface;
using EventManagament.Models;
using EventManagament.Service;
using EventManagament.Services;
using System.Security.Principal;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton<EmailService>();
builder.Services.AddSignalR();

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<INotificationHubService, NotificationHubService>();
builder.Services.AddScoped<IConnectionManager, ConnectionManager>();


// Register StoredProcedureService with Scoped lifetime
builder.Services.AddScoped<StoredProcedureService>(provider =>
    new StoredProcedureService(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});


builder.Services.AddSingleton(new Cloudinary(new Account(
    "dqpjoki72",
    "787796799295732",
    "UmBWvTfcxPw9QKe6yGHCFzb5JJE")));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllOrigins");

app.MapHub<NotificationHub>("/notifications");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();