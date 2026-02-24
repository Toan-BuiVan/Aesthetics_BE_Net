using Aesthetics.Data.RepositoryInterfaces;
using Aesthetics.Data.RepositoryServices.Common;
using Aesthetics.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Data.RepositoryServices
{
    public class AccountRepository : CommonRepository<Account>, IAccountRepository
	{
		public AccountRepository(ILogger<CommonRepository<Account>> logger, AestheticsDbContext.AestheticsDbContext dbContext) : base(logger, dbContext)
		{

		}

		public async Task<string> GenerateUniqueReferralCode()
		{
			string referralCode;
			bool exists;
			Random random = new Random();
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

			do
			{
				referralCode = new string(chars.OrderBy(x => random.Next()).Take(5).ToArray());
				exists = await _dbContext.Customers.AnyAsync(s => s.ReferralCode == referralCode);
			} while (exists);

			return referralCode;
		}

		public async Task<Account?> GetByName(string name)
		{
			try
			{
				var clinic = await _dbContext.Accounts
					.AsNoTracking()
					.FirstOrDefaultAsync(x => x.UserName.ToLower() == name.ToLower());
				return clinic;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "GetByName Exception: UserName '{UserName}'", name);
				return null;
			}
		}
	}
}
