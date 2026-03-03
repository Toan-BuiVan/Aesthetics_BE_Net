using Aesthetics.Data.AestheticsInterfaces;
using Aesthetics.Data.AestheticsInterfaces.ICommonService;
using Aesthetics.Data.RepositoryInterfaces;
using Aesthetics.Entities.Entities;
using Aesthetics.Entities.Models.RequestModel;
using Aesthetics.Entities.Models.ResponseModel;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using XAct.Expressions;

namespace Aesthetics.Data.AestheticsServices
{
    public class ProductService : IProductService
	{
		private readonly ILogger<ProductService> _logger;
		private readonly IProductRepository _productRepository;
		private readonly ISupplierRepository _supplierRepository;
		private readonly IServiceTypeRepository _serviceTypeRepository;
		private ICommonService _commonService;

		public ProductService(ILogger<ProductService> logger
			, IProductRepository productRepository
			, ISupplierRepository supplierRepository
			, IServiceTypeRepository serviceTypeRepository
			, ICommonService commonService)
		{
			_logger = logger;
			_productRepository = productRepository;
			_supplierRepository = supplierRepository;
			_serviceTypeRepository = serviceTypeRepository;
			_commonService = commonService;
		}

		public async Task<bool> create(CreateProduct product)
		{
			try
			{
				string processedImages = product.ProductImages;
				if (!string.IsNullOrEmpty(product.ProductImages))
				{
					processedImages = await _commonService.BaseProcessingFunction64(product.ProductImages);
				}

				var newProduct = new Product
				{
					ServiceTypeId = product.ServiceTypeId,
					SupplierId = product.SupplierId,
					ProductName = product.ProductName,
					Description = product.Description,
					SellingPrice = product.SellingPrice,
					Quantity = product.Quantity,
					Unit = product.Unit,
					MinimumStock = product.MinimumStock,
					ProductImages = processedImages,
					CostPrice = product.CostPrice
				};

				await _productRepository.CreateEntity(newProduct);
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating product: {ProductName}", product.ProductName);
				return false;
			}
		}

		public async Task<bool> delete(deleteProduct product)
		{
			try
			{
				_logger.LogInformation("Start deleting Product");
				var existingProduct = await _productRepository.GetById(product.Id);
				if (existingProduct == null)
				{
					_logger.LogWarning("Delete Product failed: Not found with Id {Id}", product.Id);
					return false;
				}
				var deleted = await _productRepository.DeleteRangeEntitiesStatus(existingProduct);
				if (!deleted)
				{
					_logger.LogError("Delete Product failed at repository level: Id {Id}", product.Id);
					return false;
				}
				_logger.LogInformation("Delete Product success: Id {Id}", product.Id);
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Delete Product exception: Id {Id}", product.Id);
				return false;
			}
		}

		public async Task<byte[]> ExportToExcelAsync(getproduct product)
		{
			try
			{
				_logger.LogInformation("Start exporting Products to Excel");
				Expression<Func<Product, bool>> predicate = x => x.DeleteStatus != true;
				if (product.Id.HasValue)
				{
					predicate = predicate.And(x => x.Id == product.Id.Value);
				}
				if (!string.IsNullOrWhiteSpace(product.ProductName))
				{
					var name = product.ProductName.ToLower();
					predicate = predicate.And(x => x.ProductName.ToLower().Contains(name));
				}
				if (!string.IsNullOrWhiteSpace(product.SupplierName))
				{
					var supplierName = product.SupplierName.ToLower();
					predicate = predicate.And(x => x.Supplier.SupplierName.ToLower().Contains(supplierName));
				}
				if (!string.IsNullOrWhiteSpace(product.ServiceTypeName))
				{
					var serviceTypeName = product.ServiceTypeName.ToLower();
					predicate = predicate.And(x => x.ServiceType.ServiceTypeName.ToLower().Contains(serviceTypeName));
				}
				var allProducts = await _productRepository.FindByPredicate(predicate);
				var allProductsList = allProducts.ToList();
				using (var package = new ExcelPackage())
				{
					var worksheet = package.Workbook.Worksheets.Add("Products");
					worksheet.Cells[1, 1].Value = "Id";
					worksheet.Cells[1, 2].Value = "ServiceTypeName";  
					worksheet.Cells[1, 3].Value = "SupplierName";    
					worksheet.Cells[1, 4].Value = "ProductName";
					worksheet.Cells[1, 5].Value = "Description";
					worksheet.Cells[1, 6].Value = "SellingPrice";
					worksheet.Cells[1, 7].Value = "Quantity";
					worksheet.Cells[1, 8].Value = "Unit";
					worksheet.Cells[1, 9].Value = "MinimumStock";
					worksheet.Cells[1, 10].Value = "ProductImages";
					worksheet.Cells[1, 11].Value = "CostPrice";
					for (int i = 0; i < allProductsList.Count; i++)
					{
						var row = i + 2;
						worksheet.Cells[row, 1].Value = allProductsList[i].Id;
						worksheet.Cells[row, 2].Value = allProductsList[i].ServiceType?.ServiceTypeName; 
						worksheet.Cells[row, 3].Value = allProductsList[i].Supplier?.SupplierName;      
						worksheet.Cells[row, 4].Value = allProductsList[i].ProductName;
						worksheet.Cells[row, 5].Value = allProductsList[i].Description;
						worksheet.Cells[row, 6].Value = allProductsList[i].SellingPrice;
						worksheet.Cells[row, 7].Value = allProductsList[i].Quantity;
						worksheet.Cells[row, 8].Value = allProductsList[i].Unit;
						worksheet.Cells[row, 9].Value = allProductsList[i].MinimumStock;
						worksheet.Cells[row, 10].Value = allProductsList[i].ProductImages;
						worksheet.Cells[row, 11].Value = allProductsList[i].CostPrice;
					}
					worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
					return package.GetAsByteArray();
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Export Products to Excel exception");
				return null;
			}
		}

		public async Task<BaseDataCollection<Product>> getlist(getproduct product)
		{
			try
			{
				Expression<Func<Product, bool>> predicate = x => x.DeleteStatus != true;

				if (product.Id.HasValue)
				{
					predicate = predicate.And(x => x.Id == product.Id.Value);
				}

				if (!string.IsNullOrWhiteSpace(product.ProductName))
				{
					var name = product.ProductName.ToLower();
					predicate = predicate.And(x => x.ProductName.ToLower().Contains(name));
				}

				if (!string.IsNullOrWhiteSpace(product.SupplierName))
				{
					var supplierName = product.SupplierName.ToLower();
					predicate = predicate.And(x => x.Supplier.SupplierName.ToLower().Contains(supplierName));
				}

				if (!string.IsNullOrWhiteSpace(product.ServiceTypeName))
				{
					var serviceTypeName = product.ServiceTypeName.ToLower();
					predicate = predicate.And(x => x.ServiceType.ServiceTypeName.ToLower().Contains(serviceTypeName));
				}

				var allMatching = await _productRepository.FindByPredicate(predicate);
				var totalCount = allMatching.Count;
				var pagedData = allMatching
					.OrderBy(x => x.ProductName)
					.Skip((product.PageNo - 1) * product.PageSize)
					.Take(product.PageSize)
					.ToList();

				return new BaseDataCollection<Product>(
					pagedData,
					totalCount,
					product.PageNo,
					product.PageSize
				);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "GetList Product exception");
				return new BaseDataCollection<Product>(
					null,
					0,
					product.PageNo,
					product.PageSize
				);
			}
		}

		public async Task<bool> update(updateProduct product)
		{
			try
			{
				var existingProduct = await _productRepository.GetById(product.Id);
				if (existingProduct == null)
				{
					_logger.LogWarning("Update Product failed: Not found with Id {Id}", product.Id);
					return false;
				}

				if (!string.IsNullOrWhiteSpace(product.ProductName)
					&& existingProduct.ProductName != product.ProductName)
				{
					var duplicate = await _productRepository.GetByName(product.ProductName);
					if (duplicate != null)
					{
						_logger.LogWarning("Update Product failed: Duplicate ProductName {ProductName}", product.ProductName);
						return false;
					}
					existingProduct.ProductName = product.ProductName.Trim();
				}

				if (product.ServiceTypeId.HasValue)
				{
					existingProduct.ServiceTypeId = product.ServiceTypeId.Value;
				}

				if (product.SupplierId.HasValue)
				{
					existingProduct.SupplierId = product.SupplierId.Value;
				}

				if (!string.IsNullOrWhiteSpace(product.Description))
				{
					existingProduct.Description = product.Description.Trim();
				}

				if (product.SellingPrice.HasValue)
				{
					existingProduct.SellingPrice = product.SellingPrice.Value;
				}

				if (product.Quantity.HasValue)
				{
					existingProduct.Quantity = product.Quantity.Value;
				}

				if (!string.IsNullOrWhiteSpace(product.Unit))
				{
					existingProduct.Unit = product.Unit.Trim();
				}

				if (product.MinimumStock.HasValue)
				{
					existingProduct.MinimumStock = product.MinimumStock.Value;
				}

				if (!string.IsNullOrWhiteSpace(product.ProductImages)
					&& existingProduct.ProductImages != product.ProductImages)
				{
					var processedImages = await _commonService.BaseProcessingFunction64(product.ProductImages);
					existingProduct.ProductImages = processedImages;
				}

				if (product.CostPrice.HasValue)
				{
					existingProduct.CostPrice = product.CostPrice.Value;
				}

				var updated = await _productRepository.UpdateEntity(existingProduct);
				if (!updated)
				{
					_logger.LogError("Update Product failed at repository level: Id {Id}", product.Id);
					return false;
				}

				_logger.LogInformation("Update Product success: Id {Id}", product.Id);
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Update Product exception: Id {Id}", product.Id);
				return false;
			}
		}
	}
}
