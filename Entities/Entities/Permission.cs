using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class Permission : BaseEntity
	{
		public int AccountId { get; set; }

		public int FunctionId { get; set; }

		public bool IsView { get; set; } = false;

		public bool IsInsert { get; set; } = false;

		public bool IsUpdate { get; set; } = false;

		public bool IsDelete { get; set; } = false;

		[ForeignKey(nameof(AccountId))]
		public virtual Account Account { get; set; }

		[ForeignKey(nameof(FunctionId))]
		public virtual Function Function { get; set; }
	}
}
