using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class Appointment
	{
		[Key]
		public int AppointmentID { get; set; }

		public int CustomerID { get; set; }

		public int? StaffID { get; set; }

		public int? ServiceID { get; set; }

		public DateTime StartTime { get; set; }

		public DateTime EndTime { get; set; }

		public DateTime CreationDate { get; set; }

		[MaxLength(50)]
		public string? Status { get; set; }

		public bool PaymentStatus { get; set; }

		public bool DeleteStatus { get; set; }

		// Navigation properties
		[ForeignKey(nameof(CustomerID))]
		public virtual Customer Customer { get; set; } = null!;

		[ForeignKey(nameof(StaffID))]
		public virtual Staff? Staff { get; set; }

		[ForeignKey(nameof(ServiceID))]
		public virtual Service? Service { get; set; }

		public virtual ICollection<AppointmentAssignment> AppointmentAssignments { get; set; } = new List<AppointmentAssignment>();
	}
}
