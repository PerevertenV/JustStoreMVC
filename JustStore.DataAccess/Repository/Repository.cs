using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using JustStore.DataAccess.Data;
using JustStore.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace JustStore.DataAccess.Repository
{
	public class Repository<T> : IRepository<T> where T : class
	{
		private readonly AplicationDBContextcs _db;
		internal DbSet<T> dbSet;
		public Repository(AplicationDBContextcs? db) 
		{
			_db = db;
			this.dbSet = _db.Set<T>();
			_db.Products.Include(u => u.Category).Include(u=>u.CategoryId);
		}
		public void Add(T entity)
		{
			dbSet.Add(entity);
		}

		public void Delete(T entity)
		{
			dbSet.Remove(entity);
		}

		public void DeleteRange(IEnumerable<T> entities)
		{
			dbSet.RemoveRange(entities);
		}

		public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter, string? includeProperties = null)
		{
			IQueryable<T> query = dbSet;
			if(filter != null) { query = query.Where(filter); }
            if (!string.IsNullOrEmpty(includeProperties)) 
			{
				foreach(var includeProp in includeProperties
					.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries)) 
				{
					query = query.Include(includeProp);
				}
			}
			return query.ToList();
		}

		public T GetFirstOrDefault(Expression<Func<T, bool>> filter, 
			string? includeProperties = null, bool tracked = false)
		{
			IQueryable<T> query = tracked? dbSet: dbSet.AsNoTracking();
			query = query.Where(filter);
			if (!string.IsNullOrEmpty(includeProperties))
			{
				foreach (var includeProp in includeProperties
					.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(includeProp);
				}
			}
			return query.FirstOrDefault();
		}
	}
}
