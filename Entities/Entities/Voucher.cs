using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class Voucher : BaseEntity
	{
		[Required]
		[StringLength(50)]
		public string? Code { get; set; }

		public string? Description { get; set; }

		public string? VoucherImage { get; set; }

		public decimal? DiscountValue { get; set; } = 0;

		public DateTime? StartDate { get; set; }

		public DateTime? EndDate { get; set; }

		public decimal? MinimumOrderValue { get; set; } = 0;

		public decimal? MaxValue { get; set; } = 0;

		[StringLength(200)]
		public string? RankMember { get; set; }

		public int? RatingPoints { get; set; } = 0;

		public int? AccumulatedPoints { get; set; } = 0;

		public int? UsageLimit { get; set; } = 0;

		public bool? IsActive { get; set; } = false;

		// Navigation properties
		public virtual ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
		public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
		public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
	}
}
