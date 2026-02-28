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
    public class ProductRepository : CommonRepository<Product>, IProductRepository
	{
		public ProductRepository(ILogger<CommonRepository<Product>> logger, AestheticsDbContext.AestheticsDbContext dbContext) : base(logger, dbContext)
		{

		}

		public async Task<Product?> GetByName(string name)
		{
			try
			{
				var product = await _dbContext.Products
					.AsNoTracking()
					.FirstOrDefaultAsync(x => x.ProductName.ToLower() == name.ToLower());
				return product;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "GetByName Exception: ProductName '{ProductName}'", name);
				return null;
			}
		}
	}
}
