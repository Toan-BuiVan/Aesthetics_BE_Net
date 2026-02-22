using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class Invoice : BaseEntity
	{
		public int? CustomerId { get; set; }

		public int? StaffId { get; set; }

		public int? VoucherId { get; set; }

		public decimal DiscountValue { get; set; } = 0;

		public decimal TotalMoney { get; set; } = 0;

		public DateTime DateCreated { get; set; } = DateTime.Now;

		[StringLength(50)]
		public string? Status { get; set; }

		[StringLength(50)]
		public string? Type { get; set; }

		[StringLength(50)]
		public string? OrderStatus { get; set; }

		[StringLength(50)]
		public string? PaymentMethod { get; set; }

		public decimal OutstandingBalance { get; set; } = 0;

		[ForeignKey(nameof(CustomerId))]
		public virtual Customer? Customer { get; set; }

		[ForeignKey(nameof(StaffId))]
		public virtual Staff? Staff { get; set; }

		[ForeignKey(nameof(VoucherId))]
		public virtual Voucher? Voucher { get; set; }

		// Navigation properties
		public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
		public virtual ICollection<PerformanceLog> PerformanceLogs { get; set; } = new List<PerformanceLog>();
	}
}
