using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class ClinicStaff
	{
		[Key]
		public int ClinicStaffID { get; set; }

		public int ClinicID { get; set; }

		public int StaffID { get; set; }

		// Navigation properties
		[ForeignKey(nameof(ClinicID))]
		public virtual Clinic Clinic { get; set; } = null!;

		[ForeignKey(nameof(StaffID))]
		public virtual Staff Staff { get; set; } = null!;
	}
}
