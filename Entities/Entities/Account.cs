using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class Account : BaseEntity
	{
		[Required]
		[StringLength(250)]
		public string UserName { get; set; }

		[Required]
		[StringLength(250)]
		public string PassWord { get; set; } 

		public DateTime Creation { get; set; } = DateTime.Now;

		[StringLength(250)]
		public string? RefreshToken { get; set; }

		public DateTime? TokenExpired { get; set; }

		// Navigation properties
		public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
		public virtual ICollection<Staff> Staffs { get; set; } = new List<Staff>();
		public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
		public virtual ICollection<AccountSession> AccountSessions { get; set; } = new List<AccountSession>();
	}
}
