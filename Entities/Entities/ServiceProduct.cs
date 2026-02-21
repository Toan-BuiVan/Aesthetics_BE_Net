using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class ServiceProduct
	{
		[Key]
		public int ID { get; set; }

		public int ServiceID { get; set; }

		public int ProductID { get; set; }

		public int QuantityUsed { get; set; }

		// Navigation properties
		[ForeignKey(nameof(ServiceID))]
		public virtual Service Service { get; set; } = null!;

		[ForeignKey(nameof(ProductID))]
		public virtual Product Product { get; set; } = null!;
	}
}
