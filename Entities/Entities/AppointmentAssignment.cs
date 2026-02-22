using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class AppointmentAssignment : BaseEntity
	{
		public int AppointmentId { get; set; }

		public int? ClinicId { get; set; }

		public int? ServiceTypeId { get; set; }

		public int? StaffId { get; set; }

		public int? ServiceId { get; set; }

		public int NumberOrder { get; set; } = 0;

		public DateTime AssignedDate { get; set; } = DateTime.Now;

		public bool Status { get; set; } = false;

		public int QuantityServices { get; set; } = 0;

		public decimal Price { get; set; } = 0;

		public bool PaymentStatus { get; set; } = false;

		public int? EquipmentId { get; set; }

		[ForeignKey(nameof(AppointmentId))]
		public virtual Appointment Appointment { get; set; }

		[ForeignKey(nameof(ClinicId))]
		public virtual Clinic? Clinic { get; set; }

		[ForeignKey(nameof(ServiceTypeId))]
		public virtual ServiceType? ServiceType { get; set; }

		[ForeignKey(nameof(StaffId))]
		public virtual Staff? Staff { get; set; }

		[ForeignKey(nameof(ServiceId))]
		public virtual Service? Service { get; set; }

		[ForeignKey(nameof(EquipmentId))]
		public virtual Equipment? Equipment { get; set; }
	}
}
