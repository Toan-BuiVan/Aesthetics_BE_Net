using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Aesthetics.Data.Db_Context
{
    public class Db_Context : Microsoft.EntityFrameworkCore.DbContext
	{
		public Db_Context(DbContextOptions options) : base(options) { }
		protected override void OnModelCreating(ModelBuilder builder)
		{
			////1.Chỉ định mối quan hệ 1-1 của Users & Carts
			//builder.Entity<Users>()
			//	.HasOne(u => u.Carts)
			//	.WithOne(c => c.Users)
			//	.HasForeignKey<Carts>(c => c.UserID);

			////2.Chỉ định mối quan hệ 1-N của Users(1) & Permission(N)
			//builder.Entity<Permission>()
			//	.HasOne(u => u.Users)
			//	.WithMany(p => p.Permissions)
			//	.HasForeignKey(s => s.UserID);

			////3.Chỉ định mối quan hệ N-N của Users & Clinic thông qua bảng trung gian Clinic_Staff
			//builder.Entity<Clinic_Staff>()
			//	.HasKey(cs => cs.ClinicStaffID);
			//builder.Entity<Clinic_Staff>()
			//	.HasOne(u => u.Users)
			//	.WithMany(us => us.Clinic_Staff)
			//	.HasForeignKey(s => s.UserID);
			//builder.Entity<Clinic_Staff>()
			//	.HasOne(a => a.Clinic)
			//	.WithMany(w => w.Clinic_Staff)
			//	.HasForeignKey(v => v.ClinicID);

		}
		//public virtual DbSet<Booking> Booking { get; set; }
		
	}
}
