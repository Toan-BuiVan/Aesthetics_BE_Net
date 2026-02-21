using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class Invoice
	{
		[Key]
		public int InvoiceID { get; set; }

		public int? CustomerID { get; set; }

		public int? StaffID { get; set; }

		public int? VoucherID { get; set; }

		public decimal DiscountValue { get; set; }

		public decimal TotalMoney { get; set; }

		public DateTime DateCreated { get; set; }

		[MaxLength(50)]
		public string? Status { get; set; }

		public bool DeleteStatus { get; set; }

		[MaxLength(50)]
		public string? Type { get; set; }

		[MaxLength(50)]
		public string? OrderStatus { get; set; }

		[MaxLength(50)]
		public string? PaymentMethod { get; set; }

		public decimal OutstandingBalance { get; set; }

		// Navigation properties
		[ForeignKey(nameof(CustomerID))]
		public virtual Customer? Customer { get; set; }

		[ForeignKey(nameof(StaffID))]
		public virtual Staff? Staff { get; set; }

		[ForeignKey(nameof(VoucherID))]
		public virtual Voucher? Voucher { get; set; }

		public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
		public virtual ICollection<PerformanceLog> PerformanceLogs { get; set; } = new List<PerformanceLog>();
	}
}
