using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Aesthetics.Entities.Entities
{
	public class Product
	{
		[Key]
		public int ProductID { get; set; }

		public int ServiceTypeID { get; set; }

		public int? SupplierID { get; set; }

		[MaxLength(250)]
		public string? ProductName { get; set; }

		public string? Description { get; set; }

		public decimal SellingPrice { get; set; }

		public int Quantity { get; set; }

		[MaxLength(50)]
		public string? Unit { get; set; }

		public int MinimumStock { get; set; }

		public string? ProductImages { get; set; }

		public decimal CostPrice { get; set; }

		public bool DeleteStatus { get; set; }

		// Navigation properties
		[ForeignKey(nameof(ServiceTypeID))]
		public virtual ServiceType ServiceType { get; set; } = null!;
		[ForeignKey(nameof(SupplierID))]
		public virtual Supplier? Supplier { get; set; }

		public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
		public virtual ICollection<CartProduct> CartProducts { get; set; } = new List<CartProduct>();
		public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
		public virtual ICollection<ServiceProduct> ServiceProducts { get; set; } = new List<ServiceProduct>();
	}
}
