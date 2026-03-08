using Aesthetics.Data.AestheticsInterfaces;
using Aesthetics.Data.RepositoryInterfaces;
using Aesthetics.Data.RepositoryServices;
using Aesthetics.Entities.Entities;
using Aesthetics.Entities.Enum;
using Aesthetics.Entities.Models.RequestModel;
using Aesthetics.Entities.Models.ResponseModel;
using LinqKit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Data.AestheticsServices
{
    public class CustomerTreatmentPlansService : ICustomerTreatmentPlansService
	{
		private readonly ILogger<CustomerTreatmentPlansService> _logger;
		private readonly ITreatmentPlanRepository _treatmentPlanRepository;
		private readonly ITreatmentSessionRepository _treatmentSessionRepository;
		private readonly ICustomerTreatmentPlansRepository _customerTreatmentPlansRepository;
		private readonly ICustomerTreatmentSessionsRepository _customerTreatmentSessionsRepository;
		private readonly IServiceRepository _serviceRepository;
		private readonly IInvoiceRepository _invoiceRepository;                       
		private readonly IInvoiceDetailsRepository _invoiceDetailsRepository;

		public CustomerTreatmentPlansService(ILogger<CustomerTreatmentPlansService> logger
			, ICustomerTreatmentPlansRepository customerTreatmentPlansRepository
			, ICustomerTreatmentSessionsRepository customerTreatmentSessionsRepository
			, ITreatmentPlanRepository treatmentPlanRepository
			, ITreatmentSessionRepository treatmentSessionRepository
			, IServiceRepository serviceRepository
			, IInvoiceRepository invoiceRepository
			, IInvoiceDetailsRepository invoiceDetailsRepository)
		{
			_logger = logger;
			_customerTreatmentPlansRepository = customerTreatmentPlansRepository;
			_customerTreatmentSessionsRepository = customerTreatmentSessionsRepository;
			_treatmentPlanRepository = treatmentPlanRepository;
			_treatmentSessionRepository = treatmentSessionRepository;
			_serviceRepository = serviceRepository;
			_invoiceRepository = invoiceRepository;
			_invoiceDetailsRepository = invoiceDetailsRepository;	

		}

		public async Task<bool> create(CreateCustomerTreatment request)
		{
			try
			{
				if (!IsValidRequest(request))
					return false;

				var plan = await GetTreatmentPlan(request.TreatmentPlanId);
				if (plan == null)
					return false;

				var pricing = await GetUnitPrice(plan, request.IsFullPackage ?? false);
				if (!pricing.IsValid)
					return false;

				int? invoiceId = null;

				if (request.TypeInvoice.HasValue &&
					request.TypeInvoice != EnumTreatmentPlans.PayAfterService)
				{
					invoiceId = await CreateInvoiceAsync(request, pricing);
				}

				var entity = new CustomerTreatmentPlanEntity
				{
					CustomerId = request.CustomerId,
					TreatmentPlanId = request.TreatmentPlanId,
					StartDate = request.StartDate ?? DateTime.UtcNow,
					CompletedSessions = 0,
					Status = "ChoDatLich",
					InvoiceId = invoiceId,
					Notes = request.Notes,
					DeleteStatus = false
				};

				await _customerTreatmentPlansRepository.CreateEntity(entity);
				await CloneTreatmentSessions(entity.Id, request.TreatmentPlanId);

				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "CreateCustomerTreatment exception");
				return false;
			}
		}

		public async Task<bool> delete(DeleteCustomerTreatment dto)
		{
			try
			{
				if (dto == null || !dto.Id.HasValue)
				{
					_logger.LogWarning("DeleteCustomerTreatment: invalid payload");
					return false;
				}

				var existing = (await _customerTreatmentPlansRepository.FindByPredicate(x => x.Id == dto.Id.Value)).FirstOrDefault();
				if (existing == null)
				{
					_logger.LogWarning("DeleteCustomerTreatment: not found id={Id}", dto.Id);
					return false;
				}

				await _customerTreatmentPlansRepository.DeleteRangeEntitiesStatus(existing);

				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "DeleteCustomerTreatment: exception");
				return false;
			}
		}

		public async Task<BaseDataCollection<CustomerTreatmentPlanEntity>> getlist(GetCustomerTreatment treatment)
		{
			try
			{
				Expression<Func<CustomerTreatmentPlanEntity, bool>> predicate = x => !x.DeleteStatus;

				if (treatment.CustomerId.HasValue)
				{
					predicate = predicate.And(x => x.CustomerId == treatment.CustomerId.Value);
				}

				var allMatching = await _customerTreatmentPlansRepository.FindByPredicate(predicate);
				var totalCount = allMatching.Count();

				var pagedData = allMatching
					.OrderByDescending(x => x.StartDate)
					.ThenBy(x => x.Id)
					.Skip((treatment.PageNo - 1) * treatment.PageSize)
					.Take(treatment.PageSize)
					.ToList();

				return new BaseDataCollection<CustomerTreatmentPlanEntity>(
					pagedData,
					totalCount,
					treatment.PageNo,
					treatment.PageSize
				);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "GetCustomerTreatment list exception");
				return new BaseDataCollection<CustomerTreatmentPlanEntity>(
					null,
					0,
					treatment.PageNo,
					treatment.PageSize
				);
			}
		}

		private bool IsValidRequest(CreateCustomerTreatment request)
		{
			if (request == null || !request.CustomerId.HasValue)
			{
				_logger.LogWarning("Invalid request payload");
				return false;
			}

			if (!(request.IsFullPackage ?? false) && !request.TreatmentPlanId.HasValue)
			{
				_logger.LogWarning("TreatmentPlanId required when not full package");
				return false;
			}

			return true;
		}

		private async Task<TreatmentPlanEntity?> GetTreatmentPlan(int? planId)
		{
			if (!planId.HasValue)
				return null;

			return (await _treatmentPlanRepository
				.FindByPredicate(x => x.Id == planId))
				.FirstOrDefault();
		}

		private async Task<(bool IsValid, decimal Price, int? ServiceId)> GetUnitPrice(
			TreatmentPlanEntity plan,
			bool isFullPackage)
		{
			if (isFullPackage)
			{
				var service = (await _serviceRepository
					.FindByPredicate(x => x.Id == plan.ServiceId))
					.FirstOrDefault();

				if (service == null)
					return (false, 0, null);

				return (true, service.Price ?? 0, service.Id);
			}

			return (true, plan.Price ?? 0, null);
		}

		private async Task<int?> CreateInvoiceAsync(
			CreateCustomerTreatment request,
			(bool IsValid, decimal Price, int? ServiceId) pricing)
		{
			decimal totalMoney = pricing.Price;

			decimal paidAmount = request.TypeInvoice switch
			{
				EnumTreatmentPlans.PayInAdvance => totalMoney,
				EnumTreatmentPlans.PartialPayment => request.PaidAmount ?? 0,
				_ => 0
			};

			if (request.TypeInvoice == EnumTreatmentPlans.PartialPayment && paidAmount <= 0)
				return null;

			string status = GetInvoiceStatus(paidAmount, totalMoney);

			var invoice = new InvoiceEntity
			{
				CustomerId = request.CustomerId.Value,
				StaffId = request.StaffId ?? 0,
				TotalMoney = totalMoney,
				PaidAmount = paidAmount,
				OutstandingBalance = totalMoney - paidAmount,
				DateCreated = DateTime.UtcNow,
				Status = status,
				Type = "BanHang",
				OrderStatus = "DichVu",
				DeleteStatus = false
			};

			await _invoiceRepository.CreateEntity(invoice);

			var detail = new InvoiceDetailEntity
			{
				InvoiceId = invoice.Id,
				ServiceId = pricing.ServiceId,
				TreatmentPlanId = request.TreatmentPlanId,
				Price = pricing.Price,
				Quantity = 1,
				TotalMoney = pricing.Price,
				Status = status,
				Type = "BanHang",
				DeleteStatus = false
			};

			await _invoiceDetailsRepository.CreateEntity(detail);

			return invoice.Id;
		}
		private string GetInvoiceStatus(decimal paid, decimal total)
		{
			if (paid == 0)
				return "ChuaThanhToan";

			if (paid < total)
				return "ThanhToanMotPhan";

			return "DaThanhToan";
		}

		private async Task CloneTreatmentSessions(int customerPlanId, int? treatmentPlanId)
		{
			if (!treatmentPlanId.HasValue)
				return;

			var templateSessions = await _treatmentSessionRepository
				.FindByPredicate(x => x.TreatmentPlanId == treatmentPlanId && !x.DeleteStatus);

			if (!templateSessions.Any())
				return;

			var sessions = templateSessions.Select(x => new CustomerTreatmentSessionEntity
			{
				CustomerTreatmentPlanId = customerPlanId,
				TreatmentSessionId = x.Id,
				Status = "ChuaThucHien",
				DeleteStatus = false
			}).ToList();

			await _customerTreatmentSessionsRepository.CreateRangeEntities(sessions);
		}
	}
}
