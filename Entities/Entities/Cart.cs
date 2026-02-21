using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class Cart
	{
		[Key]
		public int CartID { get; set; }

		public int CustomerID { get; set; }

		public DateTime CreationDate { get; set; }

		// Navigation properties
		[ForeignKey(nameof(CustomerID))]
		public virtual Customer Customer { get; set; } = null!;

		public virtual ICollection<CartProduct> CartProducts { get; set; } = new List<CartProduct>();
	}
}
