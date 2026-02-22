using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class Appointment : BaseEntity
	{
		public int CustomerId { get; set; }

		public int? StaffId { get; set; }

		public int? ServiceId { get; set; }

		public DateTime StartTime { get; set; }

		public DateTime EndTime { get; set; }

		public DateTime CreationDate { get; set; } = DateTime.Now;

		[StringLength(50)]
		public string? Status { get; set; }

		public bool PaymentStatus { get; set; } = false;

		[ForeignKey(nameof(CustomerId))]
		public virtual Customer Customer { get; set; }

		[ForeignKey(nameof(StaffId))]
		public virtual Staff? Staff { get; set; }

		[ForeignKey(nameof(ServiceId))]
		public virtual Service? Service { get; set; }

		// Navigation properties
		public virtual ICollection<AppointmentAssignment> AppointmentAssignments { get; set; } = new List<AppointmentAssignment>();
	}
}
