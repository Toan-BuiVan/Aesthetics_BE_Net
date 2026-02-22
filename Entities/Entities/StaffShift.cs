using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class StaffShift : BaseEntity
	{
		public int StaffId { get; set; }

		public DateTime StartTime { get; set; }

		public DateTime EndTime { get; set; }

		public DateTime Date { get; set; }

		public bool Status { get; set; } = false;

		[ForeignKey(nameof(StaffId))]
		public virtual Staff Staff { get; set; }
	}
}
