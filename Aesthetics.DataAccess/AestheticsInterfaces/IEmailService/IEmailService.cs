using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Data.AestheticsInterfaces.EmailService
{
    public interface IEmailService
    {
		/// <summary>
		/// Gửi email cảnh báo tồn kho thấp
		/// </summary>
		Task<bool> SendLowStockAlert(string toEmail, string productName, int currentQuantity, int minimumStock);

		/// <summary>
		/// Gửi email tổng hợp nhiều sản phẩm thiếu tồn kho
		/// </summary>
		Task<bool> SendBulkLowStockAlert(string toEmail, List<(string ProductName, int CurrentQuantity, int MinimumStock)> products);

	}
}
