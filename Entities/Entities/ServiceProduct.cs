using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class ServiceProduct : BaseEntity
	{
		public int ServiceId { get; set; }

		public int ProductId { get; set; }

		public int QuantityUsed { get; set; } = 0;

		[ForeignKey(nameof(ServiceId))]
		public virtual Service Service { get; set; }

		[ForeignKey(nameof(ProductId))]
		public virtual Product Product { get; set; }
	}
}
