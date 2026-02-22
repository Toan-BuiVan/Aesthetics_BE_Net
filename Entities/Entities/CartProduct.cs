using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class CartProduct : BaseEntity
	{
		public int CartId { get; set; }

		public int? ProductId { get; set; }

		public int? ServiceId { get; set; }

		public int Quantity { get; set; } = 1;

		public decimal PriceAtAdd { get; set; } = 0;

		public DateTime CreateDate { get; set; } = DateTime.Now;

		[ForeignKey(nameof(CartId))]
		public virtual Cart Cart { get; set; }

		[ForeignKey(nameof(ProductId))]
		public virtual Product? Product { get; set; }

		[ForeignKey(nameof(ServiceId))]
		public virtual Service? Service { get; set; }
	}
}
