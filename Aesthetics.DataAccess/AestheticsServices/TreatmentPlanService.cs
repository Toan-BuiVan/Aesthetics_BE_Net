using Aesthetics.Data.AestheticsInterfaces;
using Aesthetics.Data.RepositoryInterfaces;
using Aesthetics.Data.RepositoryServices;
using Aesthetics.Entities.Entities;
using Aesthetics.Entities.Models.RequestModel;
using Aesthetics.Entities.Models.ResponseModel;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Data.AestheticsServices
{
    public class TreatmentPlanService : ITreatmentPlanService
	{
		private readonly ILogger<TreatmentPlanService> _logger;
		private readonly ITreatmentPlanRepository _treatmentPlanRepository;
		private readonly ITreatmentSessionRepository _treatmentSessionRepository;


		public TreatmentPlanService(ILogger<TreatmentPlanService> logger
			, ITreatmentPlanRepository treatmentPlanRepository
			, ITreatmentSessionRepository treatmentSessionRepository)
		{
			_logger = logger;
			_treatmentPlanRepository = treatmentPlanRepository;
			_treatmentSessionRepository = treatmentSessionRepository;
		}

		public async Task<bool> create(CreateTreatmentPlan plan)
		{
			try
			{
				if (!plan.ServiceId.HasValue || string.IsNullOrWhiteSpace(plan.PlanName))
				{
					_logger.LogWarning("Create TreatmentPlan failed: Missing ServiceId or PlanName");
					return false;
				}

				var totalSessions = plan.TotalSessions ?? 0;

				var entity = new TreatmentPlanEntity
				{
					ServiceId = plan.ServiceId.Value,
					PlanName = plan.PlanName,
					TotalSessions = totalSessions,
					Price = plan.Price ?? 0,
					SessionInterval = plan.SessionInterval ?? 0,
					Description = plan.Description,
					DeleteStatus = false
				};

				var created = await _treatmentPlanRepository.CreateEntity(entity);

				if (!created)
				{
					_logger.LogError("Create TreatmentPlan failed at repository level: PlanName {PlanName}", plan.PlanName);
					return false;
				}

				if (totalSessions > 0)
				{
					var sessions = new List<TreatmentSessionEntity>();

					for (int i = 1; i <= totalSessions; i++)
					{
						sessions.Add(new TreatmentSessionEntity
						{
							TreatmentPlanId = entity.Id,
							SessionNumber = i,
							DeleteStatus = false
						});
					}

					await _treatmentSessionRepository.CreateRangeEntities(sessions);
				}

				_logger.LogInformation(
					"Create TreatmentPlan success: PlanName {PlanName} for ServiceId {ServiceId}",
					plan.PlanName,
					plan.ServiceId);

				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Create TreatmentPlan exception: PlanName {PlanName}", plan.PlanName);
				return false;
			}
		}

		public async Task<bool> delete(DeleteTreatmentPlan plan)
		{
			try
			{
				_logger.LogInformation("Start deleting TreatmentPlan");

				if (!plan.Id.HasValue)
				{
					_logger.LogWarning("Delete TreatmentPlan failed: Missing Id");
					return false;
				}

				var existingPlan = await _treatmentPlanRepository.GetById(plan.Id.Value);
				if (existingPlan == null)
				{
					_logger.LogWarning("Delete TreatmentPlan failed: Not found with Id {Id}", plan.Id);
					return false;
				}

				var deleted = await _treatmentPlanRepository.DeleteRangeEntitiesStatus(existingPlan);
				if (!deleted)
				{
					_logger.LogError("Delete TreatmentPlan failed at repository level: Id {Id}", plan.Id);
					return false;
				}

				_logger.LogInformation("Delete TreatmentPlan success: Id {Id}", plan.Id);
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Delete TreatmentPlan exception: Id {Id}", plan.Id);
				return false;
			}
		}

		public async Task<BaseDataCollection<TreatmentPlanEntity>> getlist(TreatmentPlanGet searchPlan)
		{
			try
			{
				Expression<Func<TreatmentPlanEntity, bool>> predicate = x => x.DeleteStatus != true;

				if (searchPlan.Id.HasValue)
				{
					predicate = x => x.Id == searchPlan.Id.Value && x.DeleteStatus != true;
				}
				else if (searchPlan.ServiceId.HasValue)
				{
					predicate = x => x.ServiceId == searchPlan.ServiceId.Value && x.DeleteStatus != true;
				}

				var allMatching = await _treatmentPlanRepository.FindByPredicate(predicate);
				var totalCount = allMatching.Count;

				var pagedData = allMatching
					.OrderBy(x => x.PlanName)
					.Skip((searchPlan.PageNo - 1) * searchPlan.PageSize)
					.Take(searchPlan.PageSize)
					.ToList();

				return new BaseDataCollection<TreatmentPlanEntity>(
					pagedData,
					totalCount,
					searchPlan.PageNo,
					searchPlan.PageSize
				);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "GetList TreatmentPlan exception");
				return new BaseDataCollection<TreatmentPlanEntity>(
					null,
					0,
					searchPlan.PageNo,
					searchPlan.PageSize
				);
			}
		}

		public async Task<bool> update(UpdateTreatmentPlan plan)
		{
			try
			{
				if (!plan.Id.HasValue)
				{
					_logger.LogWarning("Update TreatmentPlan failed: Missing Id");
					return false;
				}

				var existingPlan = await _treatmentPlanRepository.GetById(plan.Id.Value);
				if (existingPlan == null)
				{
					_logger.LogWarning("Update TreatmentPlan failed: Not found with Id {Id}", plan.Id);
					return false;
				}

				var oldTotalSessions = existingPlan.TotalSessions ?? 0;

				bool hasChanges = false;

				if (plan.ServiceId.HasValue && existingPlan.ServiceId != plan.ServiceId.Value)
				{
					existingPlan.ServiceId = plan.ServiceId.Value;
					hasChanges = true;
				}

				if (plan.TotalSessions.HasValue && existingPlan.TotalSessions != plan.TotalSessions.Value)
				{
					existingPlan.TotalSessions = plan.TotalSessions.Value;
					hasChanges = true;
				}

				if (plan.Price.HasValue && existingPlan.Price != plan.Price.Value)
				{
					existingPlan.Price = plan.Price.Value;
					hasChanges = true;
				}

				if (plan.SessionInterval.HasValue && existingPlan.SessionInterval != plan.SessionInterval.Value)
				{
					existingPlan.SessionInterval = plan.SessionInterval.Value;
					hasChanges = true;
				}

				if (!string.IsNullOrWhiteSpace(plan.Description) && existingPlan.Description != plan.Description)
				{
					existingPlan.Description = plan.Description;
					hasChanges = true;
				}

				if (!hasChanges)
				{
					_logger.LogInformation("Update TreatmentPlan: No changes detected for Id {Id}", plan.Id);
					return true;
				}

				var updated = await _treatmentPlanRepository.UpdateEntity(existingPlan);

				if (!updated)
				{
					_logger.LogError("Update TreatmentPlan failed at repository level: Id {Id}", plan.Id);
					return false;
				}

				// HANDLE SESSION CHANGES
				if (plan.TotalSessions.HasValue && plan.TotalSessions.Value != oldTotalSessions)
				{
					var sessions = (await _treatmentSessionRepository
						.FindByPredicate(x => x.TreatmentPlanId == existingPlan.Id && !x.DeleteStatus))
						.OrderBy(x => x.SessionNumber)
						.ToList();

					int newTotal = plan.TotalSessions.Value;

					// CASE 1: Increase sessions
					if (newTotal > oldTotalSessions)
					{
						var newSessions = new List<TreatmentSessionEntity>();

						for (int i = oldTotalSessions + 1; i <= newTotal; i++)
						{
							newSessions.Add(new TreatmentSessionEntity
							{
								TreatmentPlanId = existingPlan.Id,
								SessionNumber = i,
								DeleteStatus = false
							});
						}

						await _treatmentSessionRepository.CreateRangeEntities(newSessions);
					}

					// CASE 2: Decrease sessions
					if (newTotal < oldTotalSessions)
					{
						var sessionsToDelete = sessions
							.Where(x => x.SessionNumber > newTotal)
							.ToList();

						foreach (var session in sessionsToDelete)
						{
							session.DeleteStatus = true;
						}

						await _treatmentSessionRepository.UpdateRangeEntities(sessionsToDelete);
					}
				}

				_logger.LogInformation("Update TreatmentPlan success: Id {Id}", plan.Id);
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Update TreatmentPlan exception: Id {Id}", plan.Id);
				return false;
			}
		}
	}
}
