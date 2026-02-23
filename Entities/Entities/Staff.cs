using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class Staff : BaseEntity
	{
		public int? AccountId { get; set; }

		[StringLength(50)]
		public string? TypeStaff { get; set; }

		public DateTime? DateBirth { get; set; }

		[StringLength(20)]
		public string? Sex { get; set; }

		[StringLength(250)]
		public string? Phone { get; set; }

		[StringLength(250)]
		public string? Address { get; set; }

		[StringLength(250)]
		public string? IDCard { get; set; }

		public int SalesPoints { get; set; } = 0;

		public string? StaffImage { get; set; }

		[ForeignKey(nameof(AccountId))]
		public virtual Account? Account { get; set; }

		// Navigation properties
		public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
		public virtual ICollection<ClinicStaff> ClinicStaffs { get; set; } = new List<ClinicStaff>();
		public virtual ICollection<AppointmentAssignment> AppointmentAssignments { get; set; } = new List<AppointmentAssignment>();
		public virtual ICollection<StaffShift> StaffShifts { get; set; } = new List<StaffShift>();
		public virtual ICollection<PerformanceLog> PerformanceLogs { get; set; } = new List<PerformanceLog>();
		public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
	}
}
