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
	public class Account
	{
		[Key]
		public int AccountID { get; set; }

		[MaxLength(250)]
		[Required]
		public string UserName { get; set; } = null!;

		[MaxLength(250)]
		[Required]
		public string PassWord { get; set; } = null!; 

		public DateTime Creation { get; set; }

		[MaxLength(250)]
		public string? RefreshToken { get; set; }

		public DateTime? TokenExpired { get; set; }

		public bool DeleteStatus { get; set; }

		// Navigation properties
		public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
		public virtual ICollection<Staff> Staffs { get; set; } = new List<Staff>();
		public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
		public virtual ICollection<AccountSession> AccountSessions { get; set; } = new List<AccountSession>();
	}
}
