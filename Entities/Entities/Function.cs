using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class Function
	{
		[Key]
		public int FunctionID { get; set; }

		[MaxLength(250)]
		public string? FunctionCode { get; set; }

		[MaxLength(250)]
		public string? FunctionName { get; set; }

		public string? Description { get; set; }

		public bool DeleteStatus { get; set; }

		// Navigation properties
		public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
	}
}
