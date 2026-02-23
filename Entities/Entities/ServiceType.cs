using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	public class ServiceType : BaseEntity
	{
		[StringLength(250)]
		public string ServiceTypeName { get; set; }

		[StringLength(20)]
		public string ServiceCategory { get; set; }

		public string Description { get; set; }

		// Navigation properties
		public virtual ICollection<Service> Services { get; set; } = new List<Service>();
		public virtual ICollection<Product> Products { get; set; } = new List<Product>();
		public virtual ICollection<AppointmentAssignment> AppointmentAssignments { get; set; } = new List<AppointmentAssignment>();
	}
}
