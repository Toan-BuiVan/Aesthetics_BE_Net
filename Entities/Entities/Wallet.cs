using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class Wallet : BaseEntity
	{
		public int CustomerId { get; set; }

		public int VoucherId { get; set; }

		[ForeignKey(nameof(CustomerId))]
		public virtual Customer Customer { get; set; }

		[ForeignKey(nameof(VoucherId))]
		public virtual Voucher Voucher { get; set; }
	}
}
