using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class Wallet
	{
		[Key]
		public int WalletID { get; set; }

		public int CustomerID { get; set; }

		public int VoucherID { get; set; }

		// Navigation properties
		[ForeignKey(nameof(CustomerID))]
		public virtual Customer Customer { get; set; } = null!;

		[ForeignKey(nameof(VoucherID))]
		public virtual Voucher Voucher { get; set; } = null!;
	}
}
