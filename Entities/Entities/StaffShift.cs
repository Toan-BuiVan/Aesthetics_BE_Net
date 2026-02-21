using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class StaffShift
	{
		[Key]
		public int ShiftID { get; set; }

		public int StaffID { get; set; }

		public DateTime StartTime { get; set; }

		public DateTime EndTime { get; set; }

		public DateTime Date { get; set; } 

		public bool Status { get; set; }

		// Navigation properties
		[ForeignKey(nameof(StaffID))]
		public virtual Staff Staff { get; set; } = null!;
	}
}
