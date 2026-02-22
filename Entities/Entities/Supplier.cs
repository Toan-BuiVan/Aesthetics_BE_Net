using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class Supplier : BaseEntity
	{
		[StringLength(200)]
		public string? SupplierName { get; set; }

		[StringLength(250)]
		public string? Address { get; set; }

		[StringLength(250)]
		public string? Phone { get; set; }

		// Navigation properties
		public virtual ICollection<Product> Products { get; set; } = new List<Product>();
	}
}
