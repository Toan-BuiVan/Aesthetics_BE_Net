using Aesthetics.Data.RepositoryInterfaces.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aesthetics.Data.AestheticsDbContext;
using Microsoft.EntityFrameworkCore;
using Aesthetics.Entities.Entities;
using System.Linq.Expressions;

namespace Aesthetics.Data.RepositoryServices.Common
{
	public class CommonRepository<T> : ICommonRepository<T> where T : BaseEntity
	{
		protected readonly ILogger<CommonRepository<T>> _logger;
		protected readonly AestheticsDbContext.AestheticsDbContext _dbContext;
		protected readonly string nameEntity = typeof(T).Name;

		public CommonRepository(ILogger<CommonRepository<T>> logger, AestheticsDbContext.AestheticsDbContext dbContext)
		{
			_logger = logger;
			_dbContext = dbContext;
		}

		public async Task<bool> CreateEntity(T entity)
		{
			try
			{
				_dbContext.Set<T>().Add(entity);
				await _dbContext.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Create Entity {Na} - Exception: {E}", nameEntity, ex);
				return false;
			}
		}

		public async Task<bool> CreateRangeEntities(IEnumerable<T> entities)
		{
			try
			{
				_dbContext.Set<T>().AddRange(entities);
				await _dbContext.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Create Range Entities {Na} - Exception: {E}", nameEntity, ex);
				return false;
			}
		}

		public async Task<bool> DeleteEntitiesStatus(T entity)
		{
			try
			{
				var trackedEntity = _dbContext.Set<T>().Find(entity.Id);

				if (trackedEntity == null)
					return false;

				trackedEntity.DeleteStatus = true;
				await _dbContext.SaveChangesAsync();
				_logger.LogInformation("Soft Delete {Na} - Id: {Id}", nameEntity, entity.Id);
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Soft Delete {Na} - Id: {Id}", nameEntity, entity.Id);
				return false;
			}
		}

		public async Task<bool> DeleteEntity(T entity)
		{
			try
			{
				_dbContext.Set<T>().Remove(entity);
				var result = await _dbContext.SaveChangesAsync();
				return result > 0;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "DeleteEntity {Na} - Exception: {E}", nameEntity, ex);
				return false;
			}

		}

		public async Task<bool> DeleteRangeEntities(IEnumerable<T> entities)
		{
			try
			{
				_dbContext.Set<T>().RemoveRange(entities);
				var result = await _dbContext.SaveChangesAsync();
				return result > 0;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "DeleteRangeEntity {Na} - Exception: {E}", nameEntity, ex);
				return false;
			}

		}

		public async Task<bool> DeleteRangeEntitiesStatus(T entity)
		{
			try
			{
				var trackedEntity = _dbContext.Set<T>().Find(entity.Id);
				if (trackedEntity == null)
					return false;
				trackedEntity.DeleteStatus = true;

				var entry = _dbContext.Entry(trackedEntity);
				foreach (var navigationEntry in entry.Navigations)
				{
					if (navigationEntry.CurrentValue is IEnumerable<object> collection)
					{
						foreach (var child in collection)
						{
							((dynamic)child).DeleteStatus = true;
						}
					}
					else if (navigationEntry.CurrentValue != null)
					{
						((dynamic)navigationEntry.CurrentValue).DeleteStatus = true;
					}
				}

				await _dbContext.SaveChangesAsync();  
				_logger.LogInformation("Soft Delete {Entity} - Id: {Id}", nameEntity, entity.Id);
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Soft Delete {Entity} - Id: {Id}", nameEntity, entity.Id);
				return false;
			}
		}

		public async Task<ICollection<T>> FindByPredicate(Expression<Func<T, bool>> predicate)
		{
			try
			{
				var query = _dbContext.Set<T>().AsNoTracking().Where(predicate);
				var res = await query.ToListAsync();
				return res;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Entity {Na} - FindByPredicate - Exception: {E}", nameEntity, ex);
				return [];
			}
		}

		public async Task<T?> GetById(int id)
		{
			try
			{
				return await _dbContext.Set<T>()
				.AsNoTracking()
				.FirstOrDefaultAsync(x => x.Id == id);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Entity {Na} - GetById: {Id} - Exception: {E}", nameEntity, id, ex);
				return null;
			}
		}

		public async Task<bool> UpdateEntity(T entity)
		{
			try
			{
				_logger.LogInformation("Updating Entity {Na} with ID: {Id}", nameEntity, entity.Id);

				var trackedEntity = _dbContext.ChangeTracker
					  .Entries<T>()
					  .FirstOrDefault(e => e.Entity.Id == entity.Id);

				if (trackedEntity != null)
				{
					_dbContext.Entry(trackedEntity.Entity).State = EntityState.Detached;
				}
				_dbContext.Set<T>().Update(entity);
				var count = await _dbContext.SaveChangesAsync();
				_logger.LogInformation("Entity {Na} updated successfully. Rows affected: {Count}", nameEntity, count);
				return count > 0;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Entity {Na} {Id} updated fail: {Ex}", nameEntity, entity.Id, ex);
				_dbContext.Entry(entity).State = EntityState.Detached;
				return false;
			}
		}

		public async Task<bool> UpdateRangeEntities(IEnumerable<T> entities)
		{
			_logger.LogInformation("Start UpdateEntityRange {Na} - Count: {C}", nameEntity, entities.Count());
			if (!entities.Any()) return false;
			try
			{
				_dbContext.UpdateRange(entities);
				var count = await _dbContext.SaveChangesAsync();
				var check = count == entities.Count();
				_logger.LogInformation("Result UpdateEntityRange {Na} - {Co}", nameEntity, check);
				return check;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error UpdateEntityRange {Na} - Error: {Ex}", nameEntity, ex);
			}
			finally
			{
				_logger.LogInformation("End UpdateEntityRange {Na} - Count: {C}", nameEntity, entities.Count());
			}
			return false;
		}
	}
}
