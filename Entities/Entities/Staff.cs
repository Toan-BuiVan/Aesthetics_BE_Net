using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class Staff
	{
		[Key]
		public int StaffID { get; set; }

		public int? AccountID { get; set; }

		[MaxLength(50)]
		public string? TypeStaff { get; set; }

		public DateTime? DateBirth { get; set; }

		[MaxLength(20)]
		public string? Sex { get; set; }

		[MaxLength(250)]
		public string? Phone { get; set; }

		[MaxLength(250)]
		public string? Address { get; set; }

		[MaxLength(250)]
		public string? IDCard { get; set; }

		public int SalesPoints { get; set; }

		// Navigation properties
		[ForeignKey(nameof(AccountID))]
		public virtual Account? Account { get; set; }

		public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
		public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
		public virtual ICollection<ClinicStaff> ClinicStaffs { get; set; } = new List<ClinicStaff>();
		public virtual ICollection<AppointmentAssignment> AppointmentAssignments { get; set; } = new List<AppointmentAssignment>();
		public virtual ICollection<StaffShift> StaffShifts { get; set; } = new List<StaffShift>();
		public virtual ICollection<PerformanceLog> PerformanceLogs { get; set; } = new List<PerformanceLog>();
		public virtual ICollection<StaffMetric> StaffMetrics { get; set; } = new List<StaffMetric>();
	}
}
