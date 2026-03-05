using Aesthetics.Data.AestheticsInterfaces;
using Aesthetics.Data.AestheticsInterfaces.ICommonService;
using Aesthetics.Data.RepositoryInterfaces;
using Aesthetics.Entities.Entities;
using Aesthetics.Entities.Enum;
using Aesthetics.Entities.Models.RequestModel;
using Aesthetics.Entities.Models.ResponseModel;
using LinqKit;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Data.AestheticsServices
{
	public class ServicesService : IServicesService
	{
		private readonly ILogger<ServicesService> _logger;
		private readonly IServiceRepository _serviceRepository;
		private ICommonService _commonService;
		private readonly ITreatmentPlanRepository _treatmentPlanRepository;
		private readonly ITreatmentSessionRepository _treatmentSessionRepository;

		public ServicesService(ILogger<ServicesService> logger
			, IServiceRepository serviceRepository
			, ICommonService commonService
			, ITreatmentPlanRepository treatmentPlanRepository
			, ITreatmentSessionRepository treatmentSessionRepository)
		{
			_logger = logger;
			_serviceRepository = serviceRepository;
			_commonService = commonService;
			_treatmentPlanRepository = treatmentPlanRepository;
			_treatmentSessionRepository = treatmentSessionRepository;
		}
		public async Task<bool> create(CreateService service)
		{
			try
			{
				if (service == null)
				{
					_logger.LogWarning("CreateService request is null.");
					return false;
				}
				string processedImage = service.ServiceImage;
				if (!string.IsNullOrEmpty(service.ServiceImage))
				{
					processedImage = await _commonService.BaseProcessingFunction64(service.ServiceImage);
				}
				var serviceName = _serviceRepository.GetByName(service.ServiceName);
				if (serviceName != null)
				{
					_logger.LogWarning("Create Service failed: Service with name '{ServiceName}' already exists.", service.ServiceName);
					return false;
				}
				var newService = new ServiceEntity
				{
					ServiceTypeId = service.ServiceTypeId,
					ServiceName = service.ServiceName,
					Description = service.Description,
					ServiceImage = processedImage,
					Price = service.Price ?? 0,
					Duration = service.Duration ?? 0,
					IsCourse = (int)service.IsCourse
				};
				await _serviceRepository.CreateEntity(newService);
				if (newService.IsCourse == (int)EnumTypeCourse.Package)
				{
					var newPlan = new TreatmentPlanEntity
					{
						ServiceId = newService.Id,
						DeleteStatus = false
					};
					await _treatmentPlanRepository.CreateEntity(newPlan);

					var newSession = new TreatmentSessionEntity
					{
						TreatmentPlanId = newPlan.Id,
						DeleteStatus = false
					};
					await _treatmentSessionRepository.CreateEntity(newSession);
				}
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating service: {ServiceName}", service.ServiceName);
				return false;
			}
		}

		public async Task<bool> delete(DeleteService service)
		{
			try
			{
				_logger.LogInformation("Start deleting Service");
				var existingService = await _serviceRepository.GetById(service.Id.Value);
				if (existingService == null)
				{
					_logger.LogWarning("Delete Service failed: Not found with Id {Id}", service.Id);
					return false;
				}
				var deleted = await _serviceRepository.DeleteRangeEntitiesStatus(existingService);
				if (!deleted)
				{
					_logger.LogError("Delete Service failed at repository level: Id {Id}", service.Id);
					return false;
				}
				_logger.LogInformation("Delete Service success: Id {Id}", service.Id);
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Delete Service exception: Id {Id}", service.Id);
				return false;
			}
		}

		public async Task<byte[]> ExportToExcelAsync(ServiceGet service)
		{
			try
			{
				_logger.LogInformation("Start exporting Services to Excel");
				Expression<Func<ServiceEntity, bool>> predicate = x => x.DeleteStatus != true;
				if (service.Id.HasValue)
				{
					predicate = predicate.And(x => x.Id == service.Id.Value);
				}
				if (!string.IsNullOrWhiteSpace(service.ServiceName))
				{
					var name = service.ServiceName.ToLower();
					predicate = predicate.And(x => x.ServiceName.ToLower().Contains(name));
				}
				var allServices = await _serviceRepository.FindByPredicate(predicate);
				var allServicesList = allServices.ToList();
				using (var package = new ExcelPackage())
				{
					var worksheet = package.Workbook.Worksheets.Add("Services");
					worksheet.Cells[1, 1].Value = "Id";
					worksheet.Cells[1, 2].Value = "ServiceTypeName";
					worksheet.Cells[1, 3].Value = "ServiceName";
					worksheet.Cells[1, 4].Value = "Description";
					worksheet.Cells[1, 5].Value = "ServiceImage";
					worksheet.Cells[1, 6].Value = "Price";
					worksheet.Cells[1, 7].Value = "Duration";
					for (int i = 0; i < allServicesList.Count; i++)
					{
						var row = i + 2;
						worksheet.Cells[row, 1].Value = allServicesList[i].Id;
						worksheet.Cells[row, 2].Value = allServicesList[i].ServiceType?.ServiceTypeName;
						worksheet.Cells[row, 3].Value = allServicesList[i].ServiceName;
						worksheet.Cells[row, 4].Value = allServicesList[i].Description;
						worksheet.Cells[row, 5].Value = allServicesList[i].ServiceImage;
						worksheet.Cells[row, 6].Value = allServicesList[i].Price;
						worksheet.Cells[row, 7].Value = allServicesList[i].Duration;
					}
					worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
					return package.GetAsByteArray();
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Export Services to Excel exception");
				return null;
			}
		}

		public async Task<BaseDataCollection<ServiceEntity>> getlist(ServiceGet service)
		{
			try
			{
				Expression<Func<ServiceEntity, bool>> predicate = x => x.DeleteStatus != true;
				if (service.Id.HasValue)
				{
					predicate = predicate.And(x => x.Id == service.Id.Value);
				}
				if (!string.IsNullOrWhiteSpace(service.ServiceName))
				{
					var name = service.ServiceName.ToLower();
					predicate = predicate.And(x => x.ServiceName.ToLower().Contains(name));
				}
				var allMatching = await _serviceRepository.FindByPredicate(predicate);
				var totalCount = allMatching.Count;
				var pagedData = allMatching
					.OrderBy(x => x.ServiceName)
					.Skip((service.PageNo - 1) * service.PageSize)
					.Take(service.PageSize)
					.ToList();
				return new BaseDataCollection<ServiceEntity>(
					pagedData,
					totalCount,
					service.PageNo,
					service.PageSize
				);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "GetList Service exception");
				return new BaseDataCollection<ServiceEntity>(
					null,
					0,
					service.PageNo,
					service.PageSize
				);
			}
		}

		public async Task<bool> update(UpdateService service)
		{
			try
			{
				if (service == null || !service.Id.HasValue)
				{
					_logger.LogWarning("UpdateService request invalid.");
					return false;
				}

				var existingService = await _serviceRepository.GetById(service.Id.Value);

				if (existingService == null)
				{
					_logger.LogWarning("Update Service failed: Not found with Id {Id}", service.Id);
					return false;
				}

				int oldIsCourse = (int)existingService.IsCourse;

				if (!string.IsNullOrWhiteSpace(service.ServiceName)
					&& existingService.ServiceName != service.ServiceName)
				{
					var duplicate = await _serviceRepository.GetByName(service.ServiceName);
					if (duplicate != null)
					{
						_logger.LogWarning("Update Service failed: Duplicate ServiceName {ServiceName}", service.ServiceName);
						return false;
					}
					existingService.ServiceName = service.ServiceName.Trim();
				}

				if (service.ServiceTypeId.HasValue)
					existingService.ServiceTypeId = service.ServiceTypeId.Value;

				if (!string.IsNullOrWhiteSpace(service.Description))
					existingService.Description = service.Description.Trim();

				if (service.Price.HasValue)
					existingService.Price = service.Price.Value;

				if (service.Duration.HasValue)
					existingService.Duration = service.Duration.Value;

				if (service.IsCourse.HasValue)
					existingService.IsCourse = (int)service.IsCourse.Value;

				if (!string.IsNullOrWhiteSpace(service.ServiceImage)
					&& existingService.ServiceImage != service.ServiceImage)
				{
					var processedImage = await _commonService.BaseProcessingFunction64(service.ServiceImage);
					existingService.ServiceImage = processedImage;
				}
				var updated = await _serviceRepository.UpdateEntity(existingService);

				if (!updated)
				{
					_logger.LogError("Update Service failed at repository level: Id {Id}", service.Id);
					return false;
				}

				await HandleTreatmentPlanByCourse(existingService.Id, oldIsCourse, existingService.IsCourse ?? 0);
				_logger.LogInformation("Update Service success: Id {Id}", service.Id);
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Update Service exception: Id {Id}", service.Id);
				return false;
			}
		}

		private async Task HandleTreatmentPlanByCourse(int serviceId, int oldIsCourse, int newIsCourse)
		{
			if (oldIsCourse == newIsCourse)
				return;

			var plans = await _treatmentPlanRepository
				.FindByPredicate(x => x.ServiceId == serviceId);


			if (newIsCourse == (int)EnumTypeCourse.Package)
			{
				if (!plans.Any())
				{
					var newPlan = new TreatmentPlanEntity
					{
						ServiceId = serviceId,
						DeleteStatus = false
					};

					await _treatmentPlanRepository.CreateEntity(newPlan);

					var newSession = new TreatmentSessionEntity
					{
						TreatmentPlanId = newPlan.Id,
						DeleteStatus = false
					};

					await _treatmentSessionRepository.CreateEntity(newSession);
				}
				else
				{
					foreach (var plan in plans)
						plan.DeleteStatus = false;

					await _treatmentPlanRepository.UpdateRangeEntities(plans);

					var planIds = plans.Select(x => x.Id).ToList();

					var sessions = await _treatmentSessionRepository
						.FindByPredicate(x => planIds.Contains(x.TreatmentPlanId ?? 0));

					foreach (var session in sessions)
						session.DeleteStatus = false;

					await _treatmentSessionRepository.UpdateRangeEntities(sessions);
				}
			}

			// PACKAGE → SINGLE
			else if (newIsCourse == (int)EnumTypeCourse.Single)
			{
				if (!plans.Any())
					return;

				foreach (var plan in plans)
					plan.DeleteStatus = true;

				await _treatmentPlanRepository.UpdateRangeEntities(plans);

				var planIds = plans.Select(x => x.Id).ToList();

				var sessions = await _treatmentSessionRepository
					.FindByPredicate(x => planIds.Contains(x.TreatmentPlanId ?? 0));

				foreach (var session in sessions)
					session.DeleteStatus = true;

				await _treatmentSessionRepository.UpdateRangeEntities(sessions);
			}
		}
	}
}
