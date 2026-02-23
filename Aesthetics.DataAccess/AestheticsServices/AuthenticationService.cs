using Aesthetics.Data.AestheticsInterfaces;
using Aesthetics.Data.RepositoryInterfaces;
using Aesthetics.Entities.Models.RequestModel;
using Aesthetics.Entities.Models.ResponseModel;
using Microsoft.Extensions.Configuration;  
using Castle.Core.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using XAct.Messages;
using Microsoft.AspNetCore.Http;
using Aesthetics.Entities.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore.Storage;
using StackExchange.Redis;

namespace Aesthetics.Data.AestheticsServices
{
	public class AuthenticationService : IAuthenticationService
	{
		private readonly ILogger<AuthenticationService> _logger;
		private readonly IAuthenticationRepository _authenticationRepository;
		private Microsoft.Extensions.Configuration.IConfiguration _configuration;
		private IHttpContextAccessor _httpContextAccessor;
		private readonly IDistributedCache _cache;

		public AuthenticationService(ILogger<AuthenticationService> logger
			, IAuthenticationRepository authenticationRepository
			, Microsoft.Extensions.Configuration.IConfiguration configuration
			, IHttpContextAccessor httpContextAccessor
			, IDistributedCache cache)
		{
			_logger = logger;
			_authenticationRepository = authenticationRepository;
			_configuration = configuration;
			_httpContextAccessor = httpContextAccessor;
			_cache = cache;
		}
		public async Task<UserLoginResponse> login(RequestLogin request)
		{
			var responseData = new UserLoginResponse();
			try
			{
				var user = await _authenticationRepository.login(request);
				if (user == null)
				{
					return responseData;
				}
				var authClaims = new List<Claim>
				{
					new Claim(ClaimTypes.Name, user.UserName),
					new Claim(ClaimTypes.PrimarySid, user.Id.ToString()),
				};

				//Lưu RefreshToken
				var newToken = await CreateToken(authClaims);
				_ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);
				var refeshToken = await GenerateRefreshToken();
				await _authenticationRepository.UpdateRefeshToken(user.Id, refeshToken, DateTime.Now.AddDays(refreshTokenValidityInDays));
				var DeviceName = await GetDeviceName();
				var remoteIpAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress;
				var cachKey = "User_" + user.Id + "_" + DeviceName;
				var user_Session = new AccountSession
				{
					Id = user.Id,
					Token = new JwtSecurityTokenHandler().WriteToken(newToken),
					DeviceName = DeviceName,
					IP = remoteIpAddress.ToString(),
					CreateTime = DateTime.Now,
					DeleteStatus = false
				};
				await _authenticationRepository.CreateEntity(user_Session);
				var dataCachingJson = JsonConvert.SerializeObject(user_Session);
				var dataToCache = Encoding.UTF8.GetBytes(dataCachingJson);
				DistributedCacheEntryOptions options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddMinutes(5));
				_cache.Set(cachKey, dataToCache, options);
				return responseData;

			}
			catch (Exception ex)
			{
				_logger.LogError($"Login Exception Error: '{ex.Message}'. StackTrace: {ex.StackTrace}");
				return responseData;
			}
		}

		

		public async Task<bool> logout(RequestLogout request)
		{
			try
			{
				if (request == null || string.IsNullOrEmpty(request.AccessToken))
				{
					return false;
				}
				
				var principal = await GetPrincipalFromExpiredToken(request.AccessToken);
				if (principal == null)
				{
					return false;
				}

				string userName = principal.Identity.Name;
				var user = _authenticationRepository.GetUserByUserName(userName);
				if (user == null)
				{
					return false;
				}
				//Lấy dữ liệu từ Redis => LogOut trên 1 thiết bị
				var DeviceName = await GetDeviceName();
				var cachKey = "User_" + user.Id + "_" + DeviceName;
				_cache.Remove(cachKey);
				await _authenticationRepository.DeleteAccountSession(request.AccessToken, user.Id);
				return true;

			}
			catch (Exception ex)
			{
				_logger.LogError($"Login Exception Error: '{ex.Message}'. StackTrace: {ex.StackTrace}");
				return false;
			}
		}

		public async Task<bool> logoutall(RequestLogout request)
		{
			try
			{
				if (request == null || string.IsNullOrEmpty(request.AccessToken))
				{
					return false;
				}
				var principal = await GetPrincipalFromExpiredToken(request.AccessToken);
				if (principal == null)
				{
					return false;
				}
				string userName = principal.Identity.Name;
				var user = _authenticationRepository.GetUserByUserName(userName);
				if (user == null)
				{
					return false;
				}
				var redisConnectionString = _configuration["RedisCacheUrl"];
				if (string.IsNullOrEmpty(redisConnectionString))
				{
					throw new InvalidOperationException("Redis connection string không được cấu hình.");
				}
				if (!redisConnectionString.Contains("allowAdmin=true", StringComparison.OrdinalIgnoreCase))
				{
					redisConnectionString += ",allowAdmin=true";
				}
				var deviceName = await GetDeviceName();
				var cacheKey = "User_" + user.Id + "_" + deviceName;
				using (ConnectionMultiplexer redis = await ConnectionMultiplexer.ConnectAsync(redisConnectionString))
				{
					StackExchange.Redis.IDatabase db = redis.GetDatabase();
					var server = redis.GetServer(redis.GetEndPoints().First());
					var keys = server.Keys(database: db.Database, pattern: "User_*");
					foreach (var key in keys)
					{
						if (key.ToString().Contains(user.Id.ToString()))
						{
							_logger.LogInformation($"Delete Redis key: {key}");
							await db.KeyDeleteAsync(key);
						}
					}
				}
				await _authenticationRepository.DeleteAccountSessionAll(user.Id);

				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError($"LogOutAll_Account Error: '{ex.Message}'. StackTrace: {ex.StackTrace}");
				return false;
			}
		}

		private async Task<ClaimsPrincipal?> GetPrincipalFromExpiredToken(string? token)
		{
			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateAudience = false,
				ValidateIssuer = false,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"])),
				ValidateLifetime = false
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
			if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
				throw new SecurityTokenException("Invalid token");

			return principal;
		}

		private async Task<string> GetDeviceName()
		{
			return Environment.MachineName;
		}

		private async Task<string> GenerateRefreshToken()
		{
			var randomNumber = new byte[64];
			using var rng = RandomNumberGenerator.Create();
			rng.GetBytes(randomNumber);
			return Convert.ToBase64String(randomNumber);
		}

		private async Task<JwtSecurityToken> CreateToken(List<Claim> authClaims)
		{
			if (authClaims == null)
			{
				throw new ArgumentNullException(nameof(authClaims), "authClaims cannot be null");
			}
			var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
			_ = int.TryParse(_configuration["JWT:TokenValidityInMinutes"], out int tokenValidityInMinutes);

			var token = new JwtSecurityToken(
				issuer: _configuration["JWT:ValidIssuer"],
				audience: _configuration["JWT:ValidAudience"],
				expires: DateTime.Now.AddMinutes(tokenValidityInMinutes),
				claims: authClaims,
				signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
			);

			return token;
		}
	}
}
