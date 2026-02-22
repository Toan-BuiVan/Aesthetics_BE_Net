using Aesthetics.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Aesthetics.Data.AestheticsDbContext
{
    public class AestheticsDbContext : DbContext
	{
		public AestheticsDbContext(DbContextOptions options) : base(options) 
		{

		}
		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<Account>()
				.HasIndex(a => a.UserName)
				.IsUnique();

			builder.Entity<Customer>()
				.HasIndex(c => c.Phone)
				.IsUnique();

			builder.Entity<Customer>()
				.HasIndex(c => c.Email)
				.IsUnique();

			builder.Entity<Customer>()
				.HasIndex(c => c.IDCard)
				.IsUnique();

			builder.Entity<Function>()
				.HasIndex(f => f.FunctionCode)
				.IsUnique();

			builder.Entity<Voucher>()
				.HasIndex(v => v.Code)
				.IsUnique();

			builder.Entity<Permission>()
				.HasIndex(p => new { p.AccountId, p.FunctionId })
				.IsUnique();

			builder.Entity<ClinicStaff>()
				.HasIndex(cs => new { cs.ClinicId, cs.StaffId })
				.IsUnique();

			builder.Entity<Customer>()
				.HasOne(c => c.Account)
				.WithMany(a => a.Customers)
				.HasForeignKey(c => c.AccountId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<Staff>()
				.HasOne(s => s.Account)
				.WithMany(a => a.Staffs)
				.HasForeignKey(s => s.AccountId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<Permission>()
				.HasOne(p => p.Account)
				.WithMany(a => a.Permissions)
				.HasForeignKey(p => p.AccountId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<Permission>()
				.HasOne(p => p.Function)
				.WithMany(f => f.Permissions)
				.HasForeignKey(p => p.FunctionId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<Service>()
				.HasOne(s => s.ServiceType)
				.WithMany(st => st.Services)
				.HasForeignKey(s => s.ServiceTypeId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<Product>()
				.HasOne(p => p.ServiceType)
				.WithMany(st => st.Products)
				.HasForeignKey(p => p.ServiceTypeId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<Product>()
				.HasOne(p => p.Supplier)
				.WithMany(s => s.Products)
				.HasForeignKey(p => p.SupplierId)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Entity<Comment>()
				.HasOne(c => c.Product)
				.WithMany(p => p.Comments)
				.HasForeignKey(c => c.ProductId)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Entity<Comment>()
				.HasOne(c => c.Service)
				.WithMany(s => s.Comments)
				.HasForeignKey(c => c.ServiceId)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Entity<Comment>()
				.HasOne(c => c.Customer)
				.WithMany(cu => cu.Comments)
				.HasForeignKey(c => c.CustomerId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<Appointment>()
				.HasOne(a => a.Customer)
				.WithMany(c => c.Appointments)
				.HasForeignKey(a => a.CustomerId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<Appointment>()
				.HasOne(a => a.Staff)
				.WithMany(s => s.Appointments)
				.HasForeignKey(a => a.StaffId)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Entity<Appointment>()
				.HasOne(a => a.Service)
				.WithMany(s => s.Appointments)
				.HasForeignKey(a => a.ServiceId)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Entity<Cart>()
				.HasOne(c => c.Customer)
				.WithMany(cu => cu.Carts)
				.HasForeignKey(c => c.CustomerId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<CartProduct>()
				.HasOne(cp => cp.Cart)
				.WithMany(c => c.CartProducts)
				.HasForeignKey(cp => cp.CartId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<CartProduct>()
				.HasOne(cp => cp.Product)
				.WithMany(p => p.CartProducts)
				.HasForeignKey(cp => cp.ProductId)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Entity<CartProduct>()
				.HasOne(cp => cp.Service)
				.WithMany(s => s.CartProducts)
				.HasForeignKey(cp => cp.ServiceId)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Entity<Wallet>()
				.HasOne(w => w.Customer)
				.WithMany(c => c.Wallets)
				.HasForeignKey(w => w.CustomerId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<Wallet>()
				.HasOne(w => w.Voucher)
				.WithMany(v => v.Wallets)
				.HasForeignKey(w => w.VoucherId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<AccountSession>()
				.HasOne(asess => asess.Account)
				.WithMany(a => a.AccountSessions)
				.HasForeignKey(asess => asess.AccountId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<Invoice>()
				.HasOne(i => i.Customer)
				.WithMany(c => c.Invoices)
				.HasForeignKey(i => i.CustomerId)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Entity<Invoice>()
				.HasOne(i => i.Staff)
				.WithMany(s => s.Invoices)
				.HasForeignKey(i => i.StaffId)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Entity<Invoice>()
				.HasOne(i => i.Voucher)
				.WithMany(v => v.Invoices)
				.HasForeignKey(i => i.VoucherId)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Entity<InvoiceDetail>()
				.HasOne(id => id.Invoice)
				.WithMany(i => i.InvoiceDetails)
				.HasForeignKey(id => id.InvoiceId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<InvoiceDetail>()
				.HasOne(id => id.Product)
				.WithMany(p => p.InvoiceDetails)
				.HasForeignKey(id => id.ProductId)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Entity<InvoiceDetail>()
				.HasOne(id => id.Service)
				.WithMany(s => s.InvoiceDetails)
				.HasForeignKey(id => id.ServiceId)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Entity<InvoiceDetail>()
				.HasOne(id => id.Voucher)
				.WithMany(v => v.InvoiceDetails)
				.HasForeignKey(id => id.VoucherId)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Entity<ClinicStaff>()
				.HasOne(cs => cs.Clinic)
				.WithMany(c => c.ClinicStaffs)
				.HasForeignKey(cs => cs.ClinicId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<ClinicStaff>()
				.HasOne(cs => cs.Staff)
				.WithMany(s => s.ClinicStaffs)
				.HasForeignKey(cs => cs.StaffId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<AppointmentAssignment>()
				.HasOne(aa => aa.Appointment)
				.WithMany(a => a.AppointmentAssignments)
				.HasForeignKey(aa => aa.AppointmentId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<AppointmentAssignment>()
				.HasOne(aa => aa.Clinic)
				.WithMany(c => c.AppointmentAssignments)
				.HasForeignKey(aa => aa.ClinicId)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Entity<AppointmentAssignment>()
				.HasOne(aa => aa.ServiceType)
				.WithMany(st => st.AppointmentAssignments)
				.HasForeignKey(aa => aa.ServiceTypeId)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Entity<AppointmentAssignment>()
				.HasOne(aa => aa.Staff)
				.WithMany(s => s.AppointmentAssignments)
				.HasForeignKey(aa => aa.StaffId)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Entity<AppointmentAssignment>()
				.HasOne(aa => aa.Service)
				.WithMany(s => s.AppointmentAssignments)
				.HasForeignKey(aa => aa.ServiceId)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Entity<AppointmentAssignment>()
				.HasOne(aa => aa.Equipment)
				.WithMany(e => e.AppointmentAssignments)
				.HasForeignKey(aa => aa.EquipmentId)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Entity<StaffShift>()
				.HasOne(ss => ss.Staff)
				.WithMany(s => s.StaffShifts)
				.HasForeignKey(ss => ss.StaffId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<PerformanceLog>()
				.HasOne(pl => pl.Staff)
				.WithMany(s => s.PerformanceLogs)
				.HasForeignKey(pl => pl.StaffId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<PerformanceLog>()
				.HasOne(pl => pl.Invoice)
				.WithMany(i => i.PerformanceLogs)
				.HasForeignKey(pl => pl.InvoiceId)
				.OnDelete(DeleteBehavior.SetNull);

			builder.Entity<Equipment>()
				.HasOne(e => e.Clinic)
				.WithMany(c => c.Equipments)
				.HasForeignKey(e => e.ClinicId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<ServiceProduct>()
				.HasOne(sp => sp.Service)
				.WithMany(s => s.ServiceProducts)
				.HasForeignKey(sp => sp.ServiceId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<ServiceProduct>()
				.HasOne(sp => sp.Product)
				.WithMany(p => p.ServiceProducts)
				.HasForeignKey(sp => sp.ProductId)
				.OnDelete(DeleteBehavior.Cascade);

		}
		public DbSet<Account> Accounts { get; set; }
		public DbSet<Customer> Customers { get; set; }
		public DbSet<Staff> Staffs { get; set; }
		public DbSet<ServiceType> ServiceTypes { get; set; }
		public DbSet<Function> Functions { get; set; }
		public DbSet<Permission> Permissions { get; set; }
		public DbSet<Service> Services { get; set; }
		public DbSet<Supplier> Suppliers { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<Comment> Comments { get; set; }
		public DbSet<Voucher> Vouchers { get; set; }
		public DbSet<Appointment> Appointments { get; set; }
		public DbSet<Cart> Carts { get; set; }
		public DbSet<CartProduct> CartProducts { get; set; }
		public DbSet<Wallet> Wallets { get; set; }
		public DbSet<AccountSession> AccountSessions { get; set; }
		public DbSet<Invoice> Invoices { get; set; }
		public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
		public DbSet<Clinic> Clinics { get; set; }
		public DbSet<ClinicStaff> ClinicStaffs { get; set; }
		public DbSet<AppointmentAssignment> AppointmentAssignments { get; set; }
		public DbSet<StaffShift> StaffShifts { get; set; }
		public DbSet<PerformanceLog> PerformanceLogs { get; set; }
		public DbSet<Equipment> Equipments { get; set; }
		public DbSet<ServiceProduct> ServiceProducts { get; set; }
	}
}
