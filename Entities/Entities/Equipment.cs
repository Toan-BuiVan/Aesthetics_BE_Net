using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class Equipment : BaseEntity
	{
		[StringLength(250)]
		public string? EquipmentName { get; set; }

		public int ClinicId { get; set; }

		[StringLength(20)]
		public string? Status { get; set; }

		[ForeignKey(nameof(ClinicId))]
		public virtual Clinic Clinic { get; set; }

		// Navigation properties
		public virtual ICollection<AppointmentAssignment> AppointmentAssignments { get; set; } = new List<AppointmentAssignment>();
	}
}
