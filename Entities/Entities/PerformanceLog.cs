using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class PerformanceLog
	{
		[Key]
		public int LogID { get; set; }

		public int StaffID { get; set; }

		public int? InvoiceID { get; set; }

		public decimal Commission { get; set; }

		public decimal Bonus { get; set; }

		public DateTime LogDate { get; set; }

		public string? Description { get; set; }

		// Navigation properties
		[ForeignKey(nameof(StaffID))]
		public virtual Staff Staff { get; set; } = null!;

		[ForeignKey(nameof(InvoiceID))]
		public virtual Invoice? Invoice { get; set; }
	}
}
