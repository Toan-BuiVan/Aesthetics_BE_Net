using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class Voucher
	{
		[Key]
		public int VoucherID { get; set; }

		[MaxLength(50)]
		public string? Code { get; set; }

		public string? Description { get; set; }

		public string? VoucherImage { get; set; }

		public decimal DiscountValue { get; set; }

		public DateTime? StartDate { get; set; }

		public DateTime? EndDate { get; set; }

		public decimal MinimumOrderValue { get; set; }

		public decimal MaxValue { get; set; }

		[MaxLength(200)]
		public string? RankMember { get; set; }

		public int RatingPoints { get; set; }

		public int AccumulatedPoints { get; set; }

		public int UsageLimit { get; set; }

		public bool IsActive { get; set; }

		// Navigation properties
		public virtual ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
		public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
		public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
	}
}
