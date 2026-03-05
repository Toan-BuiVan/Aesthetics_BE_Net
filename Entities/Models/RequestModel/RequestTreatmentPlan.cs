using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Entities.Models.RequestModel
{
    public class CreateTreatmentPlan
    {
		public int? ServiceId { get; set; }
		public string? PlanName { get; set; }
		public int? TotalSessions { get; set; }
		public decimal? Price { get; set; }
		public int? SessionInterval { get; set; }
		public string? Description { get; set; }
	}

	public class UpdateTreatmentPlan
	{
		public int? Id { get; set; }
		public int? ServiceId { get; set; }
		public string? PlanName { get; set; }
		public int? TotalSessions { get; set; }
		public decimal? Price { get; set; }
		public int? SessionInterval { get; set; }
		public string? Description { get; set; }
	}

	public class DeleteTreatmentPlan
	{
		public int? Id { get; set; }
	}

	public class TreatmentPlanGet : BaseSearchModel
	{
		public int? Id { get; set; }
		public int? ServiceId { get; set; }
	}
}
