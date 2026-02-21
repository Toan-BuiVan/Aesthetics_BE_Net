using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class Comment
	{
		[Key]
		public int CommentID { get; set; }

		public int? ProductID { get; set; }

		public int? ServiceID { get; set; }

		public int CustomerID { get; set; }

		[MaxLength(250)]
		public string? CommentContent { get; set; }

		public int Rating { get; set; }

		public DateTime CreationDate { get; set; }

		// Navigation properties
		[ForeignKey(nameof(ProductID))]
		public virtual Product? Product { get; set; }

		[ForeignKey(nameof(ServiceID))]
		public virtual Service? Service { get; set; }

		[ForeignKey(nameof(CustomerID))]
		public virtual Customer Customer { get; set; } = null!;
	}
}
