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
    public class WalletService : IWalletService
	{
		private readonly IWalletRepository _walletRepository;
		private readonly ILogger<WalletService> _logger;
		private readonly ICustomerRepository _customerRepository;
		private readonly IVoucherRepository _voucherRepository;
		public WalletService(IWalletRepository walletRepository
			, ILogger<WalletService> logger
			, ICustomerRepository customerRepository
			, IVoucherRepository voucherRepository)
		{
			_walletRepository = walletRepository;
			_logger = logger;
			_customerRepository = customerRepository;
			_voucherRepository = voucherRepository;
		}

		public async Task<bool> create(CreateWallet wallet)
		{
			try
			{
				var checkWallets = await _walletRepository.GetWalletById(wallet.VoucherId, wallet.CustomerId);
				var findCustomer = await _customerRepository.GetById(wallet.CustomerId);
				var findVouchers = await _voucherRepository.GetById(wallet.VoucherId);
				if (checkWallets || findCustomer == null || findVouchers == null)
					return false;

				bool isValidRank = false;
				switch (findCustomer.RankMember?.Trim())
				{
					case "Diamond":
						isValidRank = findVouchers.RankMember?.Trim() is "Diamond" or "Gold" or "Silver" or "Bronze";
						break;
					case "Gold":
						isValidRank = findVouchers.RankMember?.Trim() is "Gold" or "Silver" or "Bronze";
						break;
					case "Silver":
						isValidRank = findVouchers.RankMember?.Trim() is "Silver" or "Bronze";
						break;
					case "Bronze":
						isValidRank = findVouchers.RankMember?.Trim() is "Bronze";
						break;
					default:
						isValidRank = false;
						break;
				}

				if (!isValidRank)
				{
					return false;
				}

				var newWallets = new Wallet
				{
					CustomerId = wallet.CustomerId,
					VoucherId = wallet.VoucherId,
					DeleteStatus = false
				};

				await _walletRepository.CreateEntity(newWallets);
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error in create Wallet");
				throw new Exception($"Error in create Wallet Message: {ex.Message} | StackTrace: {ex.StackTrace}", ex);
			}
		}

		public async Task<bool> delete(DeleteWallest wallet)
		{
			try
			{
				_logger.LogInformation("Start deleting Wallet");

				var existingWallet = await _walletRepository.GetById(wallet.WalletsID);
				if (existingWallet == null)
				{
					_logger.LogWarning("Delete Wallet failed: Not found with Id {Id}", wallet.WalletsID);
					return false;
				}

				var deleted = await _walletRepository.DeleteRangeEntitiesStatus(existingWallet);
				if (!deleted)
				{
					_logger.LogError("Delete Wallet failed at repository level: Id {Id}", wallet.WalletsID);
					return false;
				}

				_logger.LogInformation("Delete Wallet success: Id {Id}", wallet.WalletsID);
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Delete Wallet exception: Id {Id}", wallet.WalletsID);
				return false;
			}
		}

		public async Task<BaseDataCollection<Wallet>> getlist(WalletGet searchWallet)
		{
			try
			{
				Expression<Func<Wallet, bool>> predicate = x => !x.DeleteStatus;

				if (searchWallet.CustomerId > 0)
				{
					predicate = x => x.CustomerId == searchWallet.CustomerId && !x.DeleteStatus;
				}

				var allMatching = await _walletRepository.FindByPredicate(predicate);

				var totalCount = allMatching.Count;

				var pagedData = allMatching
					.OrderBy(x => x.CustomerId)                   
					.Skip((searchWallet.PageNo - 1) * searchWallet.PageSize)
					.Take(searchWallet.PageSize)
					.ToList();

				return new BaseDataCollection<Wallet>(
					pagedData,
					totalCount,
					searchWallet.PageNo,
					searchWallet.PageSize
				);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "GetList Wallet exception");
				return new BaseDataCollection<Wallet>(
					null,
					0,
					searchWallet.PageNo,
					searchWallet.PageSize
				);
			}
		}
	}
}
