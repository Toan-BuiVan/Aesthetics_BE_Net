using Aesthetics.Data.AestheticsInterfaces;
using Aesthetics.Data.AestheticsInterfaces.EmailService;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Aesthetics.Data.AestheticsServices
{
	public class EmailService : IEmailService
	{
		private readonly ILogger<EmailService> _logger;
		// TODO: Lấy từ appsettings.json
		private readonly string _smtpHost = "smtp.gmail.com";
		private readonly int _smtpPort = 587;
		private readonly string _smtpUsername = "your-email@gmail.com";
		private readonly string _smtpPassword = "your-app-password";
		private readonly string _fromEmail = "noreply@aesthetics.com";

		public EmailService(ILogger<EmailService> logger)
		{
			_logger = logger;
		}

		/// <summary>
		/// Gửi email cảnh báo tồn kho thấp cho 1 sản phẩm
		/// </summary>
		public async Task<bool> SendLowStockAlert(string toEmail, string productName, int currentQuantity, int minimumStock)
		{
			try
			{
				var subject = "🚨 Cảnh báo tồn kho thấp - " + productName;
				var body = CreateSingleProductAlertEmailBody(productName, currentQuantity, minimumStock);

				return await SendEmail(toEmail, subject, body);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "SendLowStockAlert: Exception for product {ProductName}", productName);
				return false;
			}
		}

		/// <summary>
		/// Gửi email tổng hợp nhiều sản phẩm thiếu tồn kho
		/// </summary>
		public async Task<bool> SendBulkLowStockAlert(string toEmail, List<(string ProductName, int CurrentQuantity, int MinimumStock)> products)
		{
			try
			{
				var subject = $"🚨 Cảnh báo tồn kho thấp - {products.Count} sản phẩm";
				var body = CreateBulkAlertEmailBody(products);

				return await SendEmail(toEmail, subject, body);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "SendBulkLowStockAlert: Exception for {Count} products", products.Count);
				return false;
			}
		}

		/// <summary>
		/// Gửi email thông qua SMTP
		/// </summary>
		private async Task<bool> SendEmail(string toEmail, string subject, string body)
		{
			try
			{
				using var client = new SmtpClient(_smtpHost, _smtpPort)
				{
					Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
					EnableSsl = true
				};

				var mailMessage = new MailMessage
				{
					From = new MailAddress(_fromEmail, "Hệ thống Aesthetics"),
					Subject = subject,
					Body = body,
					IsBodyHtml = true,
					Priority = MailPriority.High
				};

				mailMessage.To.Add(toEmail);

				await client.SendMailAsync(mailMessage);
				_logger.LogInformation("SendEmail: Successfully sent to {ToEmail}", toEmail);
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "SendEmail: Failed to send to {ToEmail}", toEmail);
				return false;
			}
		}

		/// <summary>
		/// Tạo nội dung email cho 1 sản phẩm
		/// </summary>
		private string CreateSingleProductAlertEmailBody(string productName, int currentQuantity, int minimumStock)
		{
			return $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #e74c3c;'>⚠️ Cảnh báo tồn kho thấp</h2>
                        <p>Sản phẩm <strong>{productName}</strong> đang có tồn kho thấp:</p>
                        <table style='border-collapse: collapse; width: 100%; margin: 20px 0;'>
                            <tr style='background-color: #f8f9fa;'>
                                <td style='padding: 10px; border: 1px solid #dee2e6;'><strong>Tồn kho hiện tại:</strong></td>
                                <td style='padding: 10px; border: 1px solid #dee2e6; color: #e74c3c;'><strong>{currentQuantity}</strong></td>
                            </tr>
                            <tr>
                                <td style='padding: 10px; border: 1px solid #dee2e6;'><strong>Tồn kho tối thiểu:</strong></td>
                                <td style='padding: 10px; border: 1px solid #dee2e6;'>{minimumStock}</td>
                            </tr>
                        </table>
                        <p style='color: #dc3545;'><strong>Vui lòng nhập thêm hàng để tránh tình trạng hết hàng!</strong></p>
                        <hr style='margin: 20px 0;'>
                        <p style='font-size: 12px; color: #6c757d;'>
                            Email này được gửi tự động từ hệ thống quản lý Aesthetics.<br>
                            Thời gian: {DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss")}
                        </p>
                    </div>
                </body>
                </html>";
		}

		/// <summary>
		/// Tạo nội dung email cho nhiều sản phẩm
		/// </summary>
		private string CreateBulkAlertEmailBody(List<(string ProductName, int CurrentQuantity, int MinimumStock)> products)
		{
			var tableRows = new StringBuilder();
			foreach (var product in products)
			{
				tableRows.AppendLine($@"
                    <tr>
                        <td style='padding: 10px; border: 1px solid #dee2e6;'>{product.ProductName}</td>
                        <td style='padding: 10px; border: 1px solid #dee2e6; color: #e74c3c; text-align: center;'><strong>{product.CurrentQuantity}</strong></td>
                        <td style='padding: 10px; border: 1px solid #dee2e6; text-align: center;'>{product.MinimumStock}</td>
                    </tr>");
			}

			return $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='max-width: 800px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #e74c3c;'>🚨 Cảnh báo tồn kho thấp - {products.Count} sản phẩm</h2>
                        <p>Các sản phẩm sau đang có tồn kho thấp hơn ngưỡng cho phép:</p>
                        
                        <table style='border-collapse: collapse; width: 100%; margin: 20px 0;'>
                            <thead>
                                <tr style='background-color: #343a40; color: white;'>
                                    <th style='padding: 12px; border: 1px solid #dee2e6;'>Tên sản phẩm</th>
                                    <th style='padding: 12px; border: 1px solid #dee2e6;'>Tồn kho hiện tại</th>
                                    <th style='padding: 12px; border: 1px solid #dee2e6;'>Tồn kho tối thiểu</th>
                                </tr>
                            </thead>
                            <tbody>
                                {tableRows}
                            </tbody>
                        </table>
                        
                        <div style='background-color: #fff3cd; padding: 15px; border-radius: 5px; border-left: 4px solid #ffc107; margin: 20px 0;'>
                            <strong style='color: #856404;'>⚠️ Hành động cần thiết:</strong>
                            <ul style='color: #856404; margin: 10px 0;'>
                                <li>Kiểm tra và liên hệ nhà cung cấp để nhập thêm hàng</li>
                                <li>Xem xét điều chỉnh ngưỡng tồn kho tối thiểu nếu cần</li>
                                <li>Thông báo cho bộ phận bán hàng về tình trạng tồn kho</li>
                            </ul>
                        </div>
                        
                        <hr style='margin: 20px 0;'>
                        <p style='font-size: 12px; color: #6c757d;'>
                            Email này được gửi tự động từ hệ thống quản lý Aesthetics.<br>
                            Thời gian: {DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss")}<br>
                            Email sẽ được gửi lại sau 5 giờ nếu vẫn chưa nhập thêm hàng.
                        </p>
                    </div>
                </body>
                </html>";
		}
	}
}
