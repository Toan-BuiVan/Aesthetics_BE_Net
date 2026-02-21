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
	public class Customer
	{
		[Key]
		public int CustomerID { get; set; }

		public int? AccountID { get; set; }

		public DateTime? DateBirth { get; set; }

		[MaxLength(20)]
		public string? Sex { get; set; }

		[MaxLength(250)]
		public string? Phone { get; set; }

		[MaxLength(250)]
		public string? Address { get; set; }

		[MaxLength(250)]
		public string? Email { get; set; }

		[MaxLength(250)]
		public string? IDCard { get; set; }

		[MaxLength(15)]
		public string? ReferralCode { get; set; }

		public int AccumulatedPoints { get; set; }

		public int RatingPoints { get; set; }

		[MaxLength(200)]
		public string? RankMember { get; set; }

		[MaxLength(200)]
		public string? CustomerGroup { get; set; }

		public string? Preferences { get; set; }

		public string? History { get; set; }

		public string? TreatmentPlans { get; set; }

		public string? SMSHistory { get; set; }

		[MaxLength(100)]
		public string? LeadSource { get; set; }

		[MaxLength(50)]
		public string? AdCampaignID { get; set; }

		[ForeignKey(nameof(AccountID))]
		public virtual Account? Account { get; set; }

		public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
		public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
		public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
		public virtual ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
		public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
	}
}
