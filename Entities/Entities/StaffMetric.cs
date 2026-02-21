using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class StaffMetric
	{
		[Key]
		public int MetricID { get; set; }

		public int StaffID { get; set; }

		public DateOnly MonthYear { get; set; } // Sử dụng DateOnly nếu .NET 6+, nếu không thì DateTime

		public int AppointmentsCompleted { get; set; }

		public decimal TotalSales { get; set; }

		// Navigation properties
		[ForeignKey(nameof(StaffID))]
		public virtual Staff Staff { get; set; } = null!;
	}
}
