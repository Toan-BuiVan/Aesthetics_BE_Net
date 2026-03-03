using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Aesthetics.Entities.Entities
{
	public class Service : BaseEntity
	{
		public int? ServiceTypeId { get; set; }

		[StringLength(200)]
		public string? ServiceName { get; set; }

		public string? Description { get; set; }

		public string? ServiceImage { get; set; }

		public decimal? Price { get; set; } = 0;

		public int? Duration { get; set; } = 0;

		[ForeignKey(nameof(ServiceTypeId))]
		public virtual ServiceType ServiceType { get; set; }

		// Navigation properties
		public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
		public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
		public virtual ICollection<CartProduct> CartProducts { get; set; } = new List<CartProduct>();
		public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
		public virtual ICollection<AppointmentAssignment> AppointmentAssignments { get; set; } = new List<AppointmentAssignment>();
		public virtual ICollection<ServiceProduct> ServiceProducts { get; set; } = new List<ServiceProduct>();
	}
}
