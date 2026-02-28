using Aesthetics.Entities.Entities;
using Aesthetics.Entities.Models.RequestModel;
using Aesthetics.Entities.Models.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Data.AestheticsInterfaces
{
    public interface IProductService
    {
		Task<bool> create(CreateProduct product);

		Task<bool> update(updateProduct product);

		Task<bool> delete(deleteProduct product);

		Task<BaseDataCollection<Product>> getlist(getproduct product);
		Task<byte[]> ExportToExcelAsync(getproduct product);
	}
}
