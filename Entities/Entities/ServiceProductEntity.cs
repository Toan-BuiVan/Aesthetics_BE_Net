using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Entities
{
	[Table("ServiceProducts")]
	public class ServiceProductEntity : Aesthetics.Entities.BaseEntity.BaseEntity
	{
		/// <summary>FK → Services: dịch vụ nào</summary>
		public int? ServiceId { get; set; }

		/// <summary>FK → Products: sản phẩm nào</summary>
		public int? ProductId { get; set; }

		/// <summary>FK → Clinics: phòng nào (cùng DV nhưng khác phòng có thể dùng SP khác)</summary>
		public int? ClinicId { get; set; }

		/// <summary>Số lượng sản phẩm dùng cho MỖI LẦN thực hiện dịch vụ</summary>
		public int QuantityUsed { get; set; } = 1;

		// Navigation properties
		[ForeignKey(nameof(ServiceId))]
		public virtual ServiceEntity? Service { get; set; }

		[ForeignKey(nameof(ProductId))]
		public virtual ProductEntity? Product { get; set; }

		[ForeignKey(nameof(ClinicId))]
		public virtual ClinicEntity? Clinic { get; set; }
	}
}
