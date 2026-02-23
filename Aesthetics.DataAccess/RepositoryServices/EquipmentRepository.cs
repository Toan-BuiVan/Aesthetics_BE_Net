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
    public class EquipmentRepository : CommonRepository<Equipment>, IEquipmentRepository
	{
		public EquipmentRepository(ILogger<CommonRepository<Equipment>> logger, AestheticsDbContext.AestheticsDbContext dbContext) : base(logger, dbContext)
		{

		}

		public async Task<Equipment?> GetByName(string name)
		{
			try
			{
				var supplier = await _dbContext.Equipments
					.AsNoTracking()
					.FirstOrDefaultAsync(x => x.EquipmentName.ToLower() == name.ToLower());
				return supplier;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "GetByName Exception: EquipmentName '{EquipmentName}'", name);
				return null;
			}
		}
	}
}
