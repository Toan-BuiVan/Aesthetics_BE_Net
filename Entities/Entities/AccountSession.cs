using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class AccountSession : BaseEntity
	{
		public int AccountId { get; set; }

		public string? Token { get; set; }

		[StringLength(250)]
		public string? DeviceName { get; set; }

		[StringLength(250)]
		public string? IP { get; set; }

		public DateTime CreateTime { get; set; } = DateTime.Now;

		public DateTime LastAccess { get; set; } = DateTime.Now;

		[ForeignKey(nameof(AccountId))]
		public virtual Account Account { get; set; }
	}
}
