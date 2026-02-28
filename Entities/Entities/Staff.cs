using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aesthetics.Entities.Enum;

namespace Aesthetics.Entities.Entities
{
	public class Staff : BaseEntity
	{
		public int AccountId { get; set; } 
		/// 
		/// 0: Staff
		/// 1: Admin
		/// 2: Doctor
		/// 
		public StaffRole Role { get; set; }  
		public DateTime? DateBirth { get; set; }
		/// 
		/// 0: Nam
		/// 1: Nu
		/// 2: Khac
		/// 
		public Gender? Sex { get; set; }
		[StringLength(250)]
		public string? Phone { get; set; }
		[StringLength(250)]
		public string? Address { get; set; }
		[StringLength(250)]
		public string? IDCard { get; set; }
		public int SalesPoints { get; set; } = 0;
		public string? StaffImage { get; set; }
		/// 
		/// 0: Active
		/// 1: Probation
		/// 2: Resigned
		/// 3: Leave
		/// 
		public EmploymentStatus EmploymentStatus { get; set; }  // Sửa: NOT NULL + enum default
														 
		/// 
		/// 0: Y tá
		/// 1: Bác sĩ
		/// 
		public DoctorLevel? DoctorLevel { get; set; }  
		[StringLength(250)]
		public string? Degree { get; set; }  // Thêm: Bằng cấp
		[StringLength(250)]
		public string? Specialization { get; set; }  // Thêm: Chuyên khoa
		[StringLength(100)]
		public string? LicenseNumber { get; set; }  // Thêm: Số giấy phép, UNIQUE ở DB
		public int? ExperienceYears { get; set; }  // Thêm: Số năm kinh nghiệm
		public string? Biography { get; set; }  // Thêm: Giới thiệu
		[ForeignKey(nameof(AccountId))]
		public virtual Account? Account { get; set; }
		// Navigation properties (giữ nguyên)
		public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
		public virtual ICollection<ClinicStaff> ClinicStaffs { get; set; } = new List<ClinicStaff>();
		public virtual ICollection<AppointmentAssignment> AppointmentAssignments { get; set; } = new List<AppointmentAssignment>();
		public virtual ICollection<StaffShift> StaffShifts { get; set; } = new List<StaffShift>();
		public virtual ICollection<PerformanceLog> PerformanceLogs { get; set; } = new List<PerformanceLog>();
		public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
	}
}
