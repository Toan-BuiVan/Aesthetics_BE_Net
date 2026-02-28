using Aesthetics.Data.AestheticsInterfaces;
using Aesthetics.Data.AestheticsInterfaces.TokenService;
using Aesthetics.Data.RepositoryInterfaces;
using Aesthetics.Data.RepositoryServices;
using Aesthetics.Entities.Entities;
using Aesthetics.Entities.Enum;
using Aesthetics.Entities.Models.RequestModel;
using Aesthetics.Entities.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Data.AestheticsServices
{
    public class AccountService : IAccountService
	{
		private readonly ILogger<AccountService> _logger;
		private readonly IAccountRepository _accountRepository;
		private IHttpContextAccessor _httpContextAccessor;
		private ITokenService _tokenService;
		private ICustomerRepository _customerRepository;
		private IStaffRepository _staffRepository;
		private ICartRepository _cartRepository;

		public AccountService(ILogger<AccountService> logger
			, IAccountRepository accountRepository
			, IHttpContextAccessor httpContextAccessor
			, ITokenService tokenService
			, ICustomerRepository customerRepository
			, IStaffRepository staffRepository
			, ICartRepository cartRepository)
		{
			_logger = logger;
			_accountRepository = accountRepository;
			_httpContextAccessor = httpContextAccessor;
			_tokenService = tokenService;
			_customerRepository = customerRepository;
			_staffRepository = staffRepository;
			_cartRepository = cartRepository;
		}

		public async Task<bool> create(RequestAccount request)
		{
			try
			{
				_logger.LogInformation("Start Create Account");
				if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.PassWord))
				{
					_logger.LogWarning("Invalid username or password provided.");
					return false;
				}
				var account = new Account
				{
					UserName = request.UserName,
					PassWord = Security.EncryptPassWord(request.PassWord), 
					Creation = DateTime.Now,
					DeleteStatus = false
				};
				await _accountRepository.CreateEntity(account);

				switch (request.AccountType)
				{
					case 0: 
						var customer = new Customer
						{
							AccountId = account.Id,
							ReferralCode = await _accountRepository.GenerateUniqueReferralCode(),
							AccumulatedPoints = 0,
							RatingPoints = 0,
							RankMember = "Bronze",
							DeleteStatus = false
						};
						await _customerRepository.CreateEntity(customer);

						var cart = new Cart
						{
							CustomerId = customer.Id,
							CreationDate = DateTime.Now,
							DeleteStatus = false
						};
						await _cartRepository.CreateEntity(cart);

						var referrerUser = await _customerRepository.GetUserIdByReferralCode(request.ReferralCode);
						if (referrerUser != null)
						{
							await _customerRepository.UpdateAccumulatedPoints(referrerUser.Id);
						}

						/*
						 * Thêm quyền cho khách hàng
						 */
						break;

					case 1: 
						var staff = new Staff
						{
							AccountId = account.Id,
							SalesPoints = 0,
							EmploymentStatus = EmploymentStatus.Active,
							Role = StaffRole.Staff,
							DeleteStatus = false,
						};
						
						await _staffRepository.CreateEntity(staff);
						/*
						 * Thêm quyền cho nhân viên
						 */
						
						break;

					default:
						throw new ArgumentException("Invalid AccountType");
				}

				_logger.LogInformation("Account created successfully");
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating account");
				return false;
			}
		}

		public async Task<bool> delete(DeleteAccount account)
		{
			try
			{
				_logger.LogInformation("Start deleting Account");

				var existingAccount = await _accountRepository.GetById(account.Id);
				if (existingAccount == null)
				{
					_logger.LogWarning("Delete Account failed: Not found with Id {Id}", account.Id);
					return false;
				}

				var deleted = await _accountRepository.DeleteRangeEntitiesStatus(existingAccount);
				if (!deleted)
				{
					_logger.LogError("Delete Account failed at repository level: Id {Id}", account.Id);
					return false;
				}

				_logger.LogInformation("Delete Account success: Id {Id}", account.Id);
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Delete Account exception: Id {Id}", account.Id);
				return false;
			}
		}

		public async Task<BaseDataCollection<Account>> getlist(AccountGet account)
		{
			try
			{
				Expression<Func<Account, bool>> predicate = x => !x.DeleteStatus;

				if (account.Id.HasValue)
				{
					predicate = x => x.Id == account.Id.Value && !x.DeleteStatus;
				}

				if (!string.IsNullOrWhiteSpace(account.UserName))
				{
					var username = account.UserName.ToLower();
					predicate = x =>
						x.UserName.ToLower().Contains(username)
						&& !x.DeleteStatus;
				}

				var allMatching = await _accountRepository.FindByPredicate(predicate);
				var totalCount = allMatching.Count;

				var pagedData = allMatching
					.OrderBy(x => x.UserName)
					.Skip((account.PageNo - 1) * account.PageSize)
					.Take(account.PageSize)
					.ToList();

				return new BaseDataCollection<Account>(
					pagedData,
					totalCount,
					account.PageNo,
					account.PageSize
				);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "GetList Account exception");
				return new BaseDataCollection<Account>(
					null,
					0,
					account.PageNo,
					account.PageSize
				);
			}
		}

		public async Task<bool> update(UpdateAccount account)
		{
			try
			{
				_logger.LogInformation("Start updating Account password");

				var existingAccount = await _accountRepository.GetById(account.Id);
				if (existingAccount == null)
				{
					_logger.LogWarning("Update Account failed: Not found with Id {Id}", account.Id);
					return false;
				}

				if (string.IsNullOrWhiteSpace(account.PassWord))
				{
					_logger.LogWarning("Update Account failed: Password is empty. Id {Id}", account.Id);
					return false;
				}

				existingAccount.PassWord = account.PassWord;

				var updated = await _accountRepository.UpdateEntity(existingAccount);
				if (!updated)
				{
					_logger.LogError("Update Account failed at repository level: Id {Id}", account.Id);
					return false;
				}

				_logger.LogInformation("Update Account password success: Id {Id}", account.Id);
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Update Account password exception: Id {Id}", account.Id);
				return false;
			}
		}
	}
}
