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
	public class Product : BaseEntity
	{
		public int ServiceTypeId { get; set; }

		public int? SupplierId { get; set; }

		[StringLength(250)]
		public string? ProductName { get; set; }

		public string? Description { get; set; }

		public decimal SellingPrice { get; set; } = 0;

		public int Quantity { get; set; } = 0;

		[StringLength(50)]
		public string? Unit { get; set; }

		public int MinimumStock { get; set; } = 0;

		public string? ProductImages { get; set; }

		public decimal CostPrice { get; set; } = 0;

		[ForeignKey(nameof(ServiceTypeId))]
		public virtual ServiceType ServiceType { get; set; }

		[ForeignKey(nameof(SupplierId))]
		public virtual Supplier? Supplier { get; set; }

		// Navigation properties
		public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
		public virtual ICollection<CartProduct> CartProducts { get; set; } = new List<CartProduct>();
		public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
		public virtual ICollection<ServiceProduct> ServiceProducts { get; set; } = new List<ServiceProduct>();
	}
}
