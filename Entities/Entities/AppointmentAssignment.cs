using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class AppointmentAssignment
	{
		[Key]
		public int AssignmentID { get; set; }

		public int AppointmentID { get; set; }

		public int? ClinicID { get; set; }

		public int? ServiceTypeID { get; set; }

		public int? StaffID { get; set; }

		public int? ServiceID { get; set; }

		public int NumberOrder { get; set; }

		public DateTime AssignedDate { get; set; }

		public bool Status { get; set; }

		public bool DeleteStatus { get; set; }

		public int QuantityServices { get; set; }

		public decimal Price { get; set; }

		public bool PaymentStatus { get; set; }

		public int? EquipmentID { get; set; }

		// Navigation properties
		[ForeignKey(nameof(AppointmentID))]
		public virtual Appointment Appointment { get; set; } = null!;

		[ForeignKey(nameof(ClinicID))]
		public virtual Clinic? Clinic { get; set; }

		[ForeignKey(nameof(ServiceTypeID))]
		public virtual ServiceType? ServiceType { get; set; }

		[ForeignKey(nameof(StaffID))]
		public virtual Staff? Staff { get; set; }

		[ForeignKey(nameof(ServiceID))]
		public virtual Service? Service { get; set; }

		[ForeignKey(nameof(EquipmentID))]
		public virtual Equipment? Equipment { get; set; }
	}
}
