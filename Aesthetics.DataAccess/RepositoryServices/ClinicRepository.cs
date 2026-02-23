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
    public class ClinicRepository : CommonRepository<Clinic>, IClinicRepository
	{
		public ClinicRepository(ILogger<CommonRepository<Clinic>> logger, AestheticsDbContext.AestheticsDbContext dbContext) : base(logger, dbContext)
		{

		}

		public async Task<Clinic?> GetByName(string name)
		{
			try
			{
				var clinic = await _dbContext.Clinics					
					.AsNoTracking()
					.FirstOrDefaultAsync(x => x.ClinicName.ToLower() == name.ToLower());
				return clinic;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "GetByName Exception: ClinicName '{ClinicName}'", name);
				return null;
			}
		}
	}
}
