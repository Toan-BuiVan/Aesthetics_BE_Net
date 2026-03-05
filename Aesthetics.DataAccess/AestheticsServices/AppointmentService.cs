using Aesthetics.Data.AestheticsInterfaces;
using Aesthetics.Data.RepositoryInterfaces;
using Aesthetics.Entities.Entities;
using Aesthetics.Entities.Enum;
using Aesthetics.Entities.Models.RequestModel;
using Aesthetics.Entities.Models.ResponseModel;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Data.AestheticsServices
{
    public class AppointmentService : IAppointmentService
	{
		private readonly ILogger<AppointmentService> _logger;
		private readonly IAppointmentRepositoty _appointmentRepositoty;
		private readonly IAppointmentAssignmentRepository _appointmentAssignmentRepository;
		private readonly IServiceRepository _serviceRepository;
		private readonly IClinicStaffRepository _clinicStaffRepository;
		private readonly IServiceTypeRepository _serviceTypeRepository;
		private readonly IAppointmentTimeLockRepository _appointmentTimeLockRepository;
		private readonly ITreatmentPlanRepository _treatmentPlanRepository;

		public AppointmentService(ILogger<AppointmentService> logger
			, IAppointmentRepositoty appointmentRepositoty
			, IAppointmentAssignmentRepository appointmentAssignmentRepository
			, IServiceRepository serviceRepository
			, IClinicStaffRepository clinicStaffRepository
			, IServiceTypeRepository serviceTypeRepository
			, IAppointmentTimeLockRepository appointmentTimeLockRepository
			, ITreatmentPlanRepository treatmentPlanRepository)
		{
			_logger = logger;
			_appointmentRepositoty = appointmentRepositoty;
			_appointmentAssignmentRepository = appointmentAssignmentRepository;
			_serviceRepository = serviceRepository;
			_clinicStaffRepository = clinicStaffRepository;
			_serviceTypeRepository = serviceTypeRepository;
			_appointmentTimeLockRepository = appointmentTimeLockRepository;
			_treatmentPlanRepository = treatmentPlanRepository;
		}

		public async Task<bool> create(CreateAppointment appointment)
		{
			try
			{
				// 1. Validate input
				if (!appointment.CustomerId.HasValue ||
					!appointment.StaffId.HasValue ||
					!appointment.ServiceId.HasValue ||
					!appointment.StartTime.HasValue)
				{
					_logger.LogWarning(
						"Create Appointment failed: Missing required fields. CustomerId {CustomerId}, StaffId {StaffId}, ServiceId {ServiceId}, StartTime {StartTime}",
						appointment.CustomerId,
						appointment.StaffId,
						appointment.ServiceId,
						appointment.StartTime
					);
					return false;
				}

				// 2. Lấy service
				var service = await _serviceRepository.GetById(appointment.ServiceId.Value);
				if (service == null)
				{
					_logger.LogWarning("Create Appointment failed: Service not found {ServiceId}", appointment.ServiceId);
					return false;
				}

				var start = appointment.StartTime.Value;

				// 3. Lấy clinic của staff
				var clinicStaff = (await _clinicStaffRepository
					.FindByPredicate(x => x.StaffId == appointment.StaffId.Value))
					.FirstOrDefault();

				var clinicId = clinicStaff?.ClinicId;

				// 4. Check lock time
				var locks = await _appointmentTimeLockRepository.FindByPredicate(x =>
					x.DeleteStatus != false &&
					x.StartTime <= start &&
					x.EndTime >= start &&
					x.ClinicId == clinicId);

				if (locks.Any())
				{
					_logger.LogWarning("Create Appointment failed: Time or clinic locked");
					return false;
				}

				// 5. Tạo appointment
				var entity = new AppointmentEntity
				{
					CustomerId = appointment.CustomerId,
					ServiceId = appointment.ServiceId,
					StartTime = appointment.StartTime,
					Status = (int)AppointmentStatus.Booked,
					PaymentStatus = false,
					DeleteStatus = false,
					CreationDate = DateTime.Now
				};

				var created = await _appointmentRepositoty.CreateEntity(entity);

				if (!created)
				{
					_logger.LogError("Create Appointment failed at repository level");
					return false;
				}

				// 6. Tạo assignment cho staff
				var assignment = new AppointmentAssignmentEntity
				{
					AppointmentId = entity.Id,
					StaffId = appointment.StaffId.Value,
					ClinicId = clinicId ?? 0,
					DeleteStatus = false
				};

				await _appointmentAssignmentRepository.CreateEntity(assignment);

				// 7. Nếu là package thì tạo thêm session
				if (service.IsCourse == (int)EnumTypeCourse.Package)
				{
					var serviceType = (await _serviceTypeRepository
						.FindByPredicate(x => x.Id == service.ServiceTypeId))
						.FirstOrDefault();

					var session = new AppointmentAssignmentEntity
					{
						AppointmentId = entity.Id,
						StaffId = appointment.StaffId.Value,
						ClinicId = clinicId ?? 0,
						ServiceId = serviceType?.Id ?? 0,
						DeleteStatus = false
					};

					await _appointmentAssignmentRepository.CreateEntity(session);
				}

				_logger.LogInformation("Create Appointment success for CustomerId {CustomerId}", appointment.CustomerId);

				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Create Appointment exception");
				return false;
			}
		}

		public Task<bool> delete(DeleteAppointment appointment)
		{
			throw new NotImplementedException();
		}

		public Task<BaseDataCollection<AppointmentEntity>> getlist(AppointmentGet appointment)
		{
			throw new NotImplementedException();
		}
	}
}
