using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class Cart : BaseEntity
	{
		public int CustomerId { get; set; }

		public DateTime CreationDate { get; set; } = DateTime.Now;

		[ForeignKey(nameof(CustomerId))]
		public virtual Customer Customer { get; set; }

		// Navigation properties
		public virtual ICollection<CartProduct> CartProducts { get; set; } = new List<CartProduct>();
	}
}
