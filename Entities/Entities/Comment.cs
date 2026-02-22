using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class Comment : BaseEntity
	{
		public int? ProductId { get; set; }

		public int? ServiceId { get; set; }

		public int CustomerId { get; set; }

		[StringLength(250)]
		public string? CommentContent { get; set; }

		[Range(1, 5)]
		public int Rating { get; set; } = 1;

		public DateTime CreationDate { get; set; } = DateTime.Now;

		[ForeignKey(nameof(ProductId))]
		public virtual Product? Product { get; set; }

		[ForeignKey(nameof(ServiceId))]
		public virtual Service? Service { get; set; }

		[ForeignKey(nameof(CustomerId))]
		public virtual Customer Customer { get; set; }
	}
}
