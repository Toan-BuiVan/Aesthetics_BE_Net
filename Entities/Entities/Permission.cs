using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class Permission
	{
		[Key]
		public int PermissionID { get; set; }

		public int AccountID { get; set; }

		public int FunctionID { get; set; }

		public bool IsView { get; set; }

		public bool IsInsert { get; set; }

		public bool IsUpdate { get; set; }

		public bool IsDelete { get; set; }

		// Navigation properties
		[ForeignKey(nameof(AccountID))]
		public virtual Account Account { get; set; } = null!;

		[ForeignKey(nameof(FunctionID))]
		public virtual Function Function { get; set; } = null!;
	}
}
