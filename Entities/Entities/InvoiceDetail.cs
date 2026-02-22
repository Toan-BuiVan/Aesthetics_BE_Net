using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class InvoiceDetail : BaseEntity
	{
		public int InvoiceId { get; set; }

		public int? ProductId { get; set; }

		public int? ServiceId { get; set; }

		public int? VoucherId { get; set; }

		public decimal DiscountValue { get; set; } = 0;

		public decimal Price { get; set; } = 0;

		public int Quantity { get; set; } = 0;

		public decimal TotalMoney { get; set; } = 0;

		[StringLength(50)]
		public string? Status { get; set; }

		[StringLength(50)]
		public string? Type { get; set; }

		public bool StatusComment { get; set; } = false;

		[ForeignKey(nameof(InvoiceId))]
		public virtual Invoice Invoice { get; set; }

		[ForeignKey(nameof(ProductId))]
		public virtual Product? Product { get; set; }

		[ForeignKey(nameof(ServiceId))]
		public virtual Service? Service { get; set; }

		[ForeignKey(nameof(VoucherId))]
		public virtual Voucher? Voucher { get; set; }
	}
}
