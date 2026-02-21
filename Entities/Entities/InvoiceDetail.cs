using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class InvoiceDetail
	{
		[Key]
		public int InvoiceDetailID { get; set; }

		public int InvoiceID { get; set; }

		public int? ProductID { get; set; }

		public int? ServiceID { get; set; }

		public int? VoucherID { get; set; }

		public decimal DiscountValue { get; set; }

		public decimal Price { get; set; }

		public int Quantity { get; set; }

		public decimal TotalMoney { get; set; }

		[MaxLength(50)]
		public string? Status { get; set; }

		public bool DeleteStatus { get; set; }

		[MaxLength(50)]
		public string? Type { get; set; }

		public bool StatusComment { get; set; }

		// Navigation properties
		[ForeignKey(nameof(InvoiceID))]
		public virtual Invoice Invoice { get; set; } = null!;

		[ForeignKey(nameof(ProductID))]
		public virtual Product? Product { get; set; }

		[ForeignKey(nameof(ServiceID))]
		public virtual Service? Service { get; set; }

		[ForeignKey(nameof(VoucherID))]
		public virtual Voucher? Voucher { get; set; }
	}
}
