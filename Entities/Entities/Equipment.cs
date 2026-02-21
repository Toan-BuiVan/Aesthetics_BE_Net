using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class Equipment
	{
		[Key]
		public int EquipmentID { get; set; }

		[MaxLength(250)]
		public string? EquipmentName { get; set; }

		public int ClinicID { get; set; }

		[MaxLength(20)]
		public string? Status { get; set; }

		// Navigation properties
		[ForeignKey(nameof(ClinicID))]
		public virtual Clinic Clinic { get; set; } = null!;

		public virtual ICollection<AppointmentAssignment> AppointmentAssignments { get; set; } = new List<AppointmentAssignment>();
	}
}
