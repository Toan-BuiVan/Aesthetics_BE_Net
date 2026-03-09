using Aesthetics.Data.AestheticsDbContext;
using Aesthetics.Data.AestheticsInterfaces;
using Aesthetics.Data.AestheticsInterfaces.EmailService;
using Aesthetics.Data.AestheticsServices;
using Aesthetics.Data.AestheticsServices.EmailService;
using Aesthetics.Data.BackgroundServices;
using Aesthetics.Data.RepositoryInterfaces;
using Aesthetics.Data.RepositoryServices;
using Microsoft.AspNetCore.Identity;
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

// Repository
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IAppointmentRepositoty, AppointmentRepositoty>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAppointmentAssignmentRepository, AppointmentAssignmentRepository>();
builder.Services.AddScoped<IAppointmentTimeLockRepository, AppointmentTimeLockRepository>();
builder.Services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
builder.Services.AddScoped<ICartProductRepository, CartProductRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IClinicRepository, ClinicRepository>();
builder.Services.AddScoped<IClinicStaffRepository, ClinicStaffRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ICustomerTreatmentPlansRepository, CustomerTreatmentPlansRepository>();
builder.Services.AddScoped<ICustomerTreatmentSessionsRepository, CustomerTreatmentSessionsRepository>();
builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();
builder.Services.AddScoped<IInventoryAlertRepository, InventoryAlertRepository>();
builder.Services.AddScoped<IInvoiceDetailsRepository, InvoiceDetailsRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IPerformanceLogRepository, PerformanceLogRepository>();
builder.Services.AddScoped<IServiceTypeRepository, ServiceTypeRepository>();
builder.Services.AddScoped<ISessionProductRepository, SessionProductRepository>();
builder.Services.AddScoped<IStaffShiftRepository, StaffShiftRepository>();
builder.Services.AddScoped<ITreatmentPlanRepository, TreatmentPlanRepository>();
builder.Services.AddScoped<ITreatmentSessionRepository, TreatmentSessionRepository>();
builder.Services.AddScoped<IVoucherRepository, VoucherRepository>();
builder.Services.AddScoped<IWalletRepository, WalletRepository>();

// Service
builder.Services.AddScoped<IInventoryAlertService, InventoryAlertService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IInventoryAlertRepository, InventoryAlertRepository>();
builder.Services.AddScoped<ISupplierSevice, SupplierSevice>();

// Email services
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddHostedService<Aesthetics.Data.AestheticsServices.EmailService.AppointmentReminderBackgroundService>();

// Background Service
builder.Services.AddHostedService<InventoryAlertBackgroundService>();

// Additional services you might need
builder.Services.AddScoped<IPasswordHasher<object>, PasswordHasher<object>>();
builder.Services.AddMemoryCache();
builder.Services.AddLogging();

// CORS - moved BEFORE builder.Build()
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAll", builder =>
	{
		builder.AllowAnyOrigin()
			   .AllowAnyMethod()
			   .AllowAnyHeader();
	});
});

// If you need authentication/authorization - moved BEFORE builder.Build()
//builder.Services.AddAuthentication("Bearer")
//	.AddJwtBearer("Bearer", options =>
//	{
//		// JWT configuration
//	});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
