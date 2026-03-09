using Aesthetics.Data.AestheticsInterfaces;
using Aesthetics.Data.AestheticsInterfaces.EmailService;
using Aesthetics.Data.RepositoryInterfaces;
using Aesthetics.Data.RepositoryServices;
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
		private readonly IEmailService _emailService;
		private readonly ICustomerRepository _customerRepository;
		private readonly IStaffRepository _staffRepository;

		public AppointmentService(ILogger<AppointmentService> logger
			, IAppointmentRepositoty appointmentRepositoty
			, IAppointmentAssignmentRepository appointmentAssignmentRepository
			, IServiceRepository serviceRepository
			, IClinicStaffRepository clinicStaffRepository
			, IServiceTypeRepository serviceTypeRepository
			, IAppointmentTimeLockRepository appointmentTimeLockRepository
			, ITreatmentPlanRepository treatmentPlanRepository
			, IEmailService emailService
			, ICustomerRepository customerRepository
			, IStaffRepository staffRepository)
		{
			_logger = logger;
			_appointmentRepositoty = appointmentRepositoty;
			_appointmentAssignmentRepository = appointmentAssignmentRepository;
			_serviceRepository = serviceRepository;
			_clinicStaffRepository = clinicStaffRepository;
			_serviceTypeRepository = serviceTypeRepository;
			_appointmentTimeLockRepository = appointmentTimeLockRepository;
			_treatmentPlanRepository = treatmentPlanRepository;
			_emailService = emailService;
			_customerRepository = customerRepository;
			_staffRepository = staffRepository;
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
					CreationDate = DateTime.Now,
					IsConfirmationEmailSent = false,
					IsReminderEmailSent = false,
					ReminderHoursBefore = 24
				};

				var created = await _appointmentRepositoty.CreateEntity(entity);

				if (!created)
				{
					_logger.LogError("Create Appointment failed at repository level");
					return false;
				}

				// 6. Tạo AppointmentAssignment cho tất cả appointments
				var assignment = new AppointmentAssignmentEntity
				{
					AppointmentId = entity.Id,
					StaffId = appointment.StaffId.Value,
					ClinicId = clinicId ?? 0,
					ServiceId = appointment.ServiceId.Value,
					ServiceTypeId = service.ServiceTypeId,
					AssignedDate = appointment.StartTime,
					Status = false, // Chưa thực hiện
					QuantityServices = 1,
					Price = service.Price,
					PaymentStatus = false,
					NumberOrder = 1,
					DeleteStatus = false
				};

				var assignmentCreated = await _appointmentAssignmentRepository.CreateEntity(assignment);
				if (!assignmentCreated)
				{
					_logger.LogWarning("Create AppointmentAssignment failed for AppointmentId {AppointmentId}", entity.Id);
				}

				// 7. Nếu là package thì tạo thêm session cho treatment plan
				if (service.IsCourse == (int)EnumTypeCourse.Package)
				{
					var serviceType = (await _serviceTypeRepository
						.FindByPredicate(x => x.Id == service.ServiceTypeId))
						.FirstOrDefault();

					// Tạo session bổ sung cho package (có thể có nhiều session)
					var packageSession = new AppointmentAssignmentEntity
					{
						AppointmentId = entity.Id,
						StaffId = appointment.StaffId.Value,
						ClinicId = clinicId ?? 0,
						ServiceId = appointment.ServiceId.Value,
						ServiceTypeId = service.ServiceTypeId,
						AssignedDate = appointment.StartTime,
						Status = false,
						QuantityServices = 1,
						Price = service.Price,
						PaymentStatus = false,
						NumberOrder = 2, // Thứ tự thứ 2 cho package
						DeleteStatus = false
					};

					await _appointmentAssignmentRepository.CreateEntity(packageSession);
				}

				// 8. Gửi email xác nhận đặt lịch
				await SendConfirmationEmail(entity);

				_logger.LogInformation("Create Appointment success for CustomerId {CustomerId} with AppointmentAssignment", appointment.CustomerId);

				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Create Appointment exception");
				return false;
			}
		}


		/// <summary>
		/// Gửi email xác nhận đặt lịch thành công
		/// </summary>
		private async Task SendConfirmationEmail(AppointmentEntity appointment)
		{
			try
			{
				// Lấy thông tin customer
				var customer = await _customerRepository.GetById(appointment.CustomerId.Value);
				if (customer == null || string.IsNullOrEmpty(customer.Email))
				{
					_logger.LogWarning("SendConfirmationEmail: Customer not found or no email. CustomerId {CustomerId}", appointment.CustomerId);
					return;
				}

				// Lấy thông tin staff
				var staff = await _staffRepository.GetById(appointment.StaffId.Value);
				var staffName = staff?.FullName ?? "Nhân viên";

				// Lấy thông tin service
				var service = await _serviceRepository.GetById(appointment.ServiceId.Value);
				var serviceName = service?.ServiceName ?? "Dịch vụ";

				// Gửi email
				var emailSent = await _emailService.SendAppointmentConfirmation(
					customer.Email,
					customer.FullName ?? "Khách hàng",
					serviceName,
					appointment.StartTime.Value,
					staffName
				);

				if (emailSent)
				{
					// Cập nhật trạng thái đã gửi email
					appointment.IsConfirmationEmailSent = true;
					appointment.ConfirmationEmailSentDate = DateTime.UtcNow;
					await _appointmentRepositoty.UpdateEntity(appointment);

					_logger.LogInformation("SendConfirmationEmail: Successfully sent to {Email} for AppointmentId {AppointmentId}",
						customer.Email, appointment.Id);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "SendConfirmationEmail: Exception for AppointmentId {AppointmentId}", appointment.Id);
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
