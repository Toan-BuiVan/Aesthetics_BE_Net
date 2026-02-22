using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class BaseEntity
	{
		[Key]
		public int Id { get; set; }
		public bool DeleteStatus { get; set; }
	}
}
