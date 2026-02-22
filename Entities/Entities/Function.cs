using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class Function : BaseEntity
	{
		[Required]
		[StringLength(250)]
		public string FunctionCode { get; set; }

		[StringLength(250)]
		public string? FunctionName { get; set; }

		public string? Description { get; set; }

		// Navigation properties
		public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
	}
}
