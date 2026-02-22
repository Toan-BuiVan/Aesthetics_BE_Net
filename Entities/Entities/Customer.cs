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
	public class Customer : BaseEntity
	{
		public int? AccountId { get; set; }

		public DateTime? DateBirth { get; set; }

		[StringLength(20)]
		public string? Sex { get; set; }

		[StringLength(250)]
		public string? Phone { get; set; }

		[StringLength(250)]
		public string? Address { get; set; }

		[StringLength(250)]
		public string? Email { get; set; }

		[StringLength(250)]
		public string? IDCard { get; set; }

		[StringLength(15)]
		public string? ReferralCode { get; set; }

		public int AccumulatedPoints { get; set; } = 0;

		public int RatingPoints { get; set; } = 0;

		[StringLength(200)]
		public string? RankMember { get; set; }

		[ForeignKey(nameof(AccountId))]
		public virtual Account? Account { get; set; }

		// Navigation properties
		public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
		public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
		public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
		public virtual ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
		public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
	}
}
