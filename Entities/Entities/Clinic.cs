using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class Clinic : BaseEntity
	{
		[StringLength(255)]
		public string? ClinicName { get; set; }

		public bool ClinicStatus { get; set; } = false;

		// Navigation properties
		public virtual ICollection<ClinicStaff> ClinicStaffs { get; set; } = new List<ClinicStaff>();
		public virtual ICollection<AppointmentAssignment> AppointmentAssignments { get; set; } = new List<AppointmentAssignment>();
		public virtual ICollection<Equipment> Equipments { get; set; } = new List<Equipment>();
	}
}
