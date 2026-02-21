using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class AccountSession
	{
		[Key]
		public int SessionID { get; set; }

		public int AccountID { get; set; }

		public string? Token { get; set; }

		[MaxLength(250)]
		public string? DeviceName { get; set; }

		[MaxLength(250)]
		public string? IP { get; set; }

		public DateTime CreateTime { get; set; }

		public DateTime? LastAccess { get; set; }

		public bool DeleteStatus { get; set; }

		// Navigation properties
		[ForeignKey(nameof(AccountID))]
		public virtual Account Account { get; set; } = null!;
	}
}
