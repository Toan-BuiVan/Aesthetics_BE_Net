using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class CartProduct
	{
		[Key]
		public int CartProductID { get; set; }

		public int CartID { get; set; }

		public int? ProductID { get; set; }

		public int? ServiceID { get; set; }

		public int Quantity { get; set; }

		public decimal PriceAtAdd { get; set; }

		public DateTime CreateDate { get; set; }

		// Navigation properties
		[ForeignKey(nameof(CartID))]
		public virtual Cart Cart { get; set; } = null!;

		[ForeignKey(nameof(ProductID))]
		public virtual Product? Product { get; set; }

		[ForeignKey(nameof(ServiceID))]
		public virtual Service? Service { get; set; }
	}
}
