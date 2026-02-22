using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class ClinicStaff : BaseEntity
	{
		public int ClinicId { get; set; }

		public int StaffId { get; set; }

		[ForeignKey(nameof(ClinicId))]
		public virtual Clinic Clinic { get; set; }

		[ForeignKey(nameof(StaffId))]
		public virtual Staff Staff { get; set; }
	}
}
