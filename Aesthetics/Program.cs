using Aesthetics.Data.AestheticsDbContext;
using Aesthetics.Data.AestheticsInterfaces;
using Aesthetics.Data.AestheticsInterfaces.EmailService;
using Aesthetics.Data.AestheticsServices;
using Aesthetics.Data.BackgroundServices;
using Aesthetics.Data.RepositoryInterfaces;
using Aesthetics.Data.RepositoryServices;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
builder.Services.AddDbContext<AestheticsDbContext>(options =>
			   options.UseSqlServer(configuration.GetConnectionString("aesthetics")));
builder.Services.AddHttpContextAccessor();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Repository
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
// Thêm vào Program.cs hoặc Startup.cs
builder.Services.AddScoped<IInventoryAlertService, InventoryAlertService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IInventoryAlertRepository, InventoryAlertRepository>(); 

// Background Service
builder.Services.AddHostedService<InventoryAlertBackgroundService>();


//Service
builder.Services.AddScoped<ISupplierSevice, SupplierSevice>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
