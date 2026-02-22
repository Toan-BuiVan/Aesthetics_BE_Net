using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class PerformanceLog : BaseEntity
	{
		public int StaffId { get; set; }

		public int? InvoiceId { get; set; }

		public decimal Commission { get; set; } = 0;

		public decimal Bonus { get; set; } = 0;

		public DateTime LogDate { get; set; } = DateTime.Now;

		public string? Description { get; set; }

		[ForeignKey(nameof(StaffId))]
		public virtual Staff Staff { get; set; }

		[ForeignKey(nameof(InvoiceId))]
		public virtual Invoice? Invoice { get; set; }
	}
}
