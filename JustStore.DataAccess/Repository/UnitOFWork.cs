using JustStore.DataAccess.Data;
using JustStore.DataAccess.Repository.IRepository;
using JustStore.Models;
using Microsoft.CodeAnalysis.CodeRefactorings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JustStore.DataAccess.Repository
{
	public class UnitOFWork : IUnitOfWork
	{
		private AplicationDBContextcs _db;
		public ICategoryRepository Category { get; private set; }
		public IProductRepository Product { get; private set; }
		public ICompanyRepository Company { get; private set; }
		public IShoppingCartRepository ShoppingCart { get; private set; }
		public IApplicationUserRepository ApplicationUser { get; private set; }
		public IOrderHeaderRepository OrderHeader { get; private set; }
		public IOrderDetailRepository OrderDetail { get; private set; }
		public UnitOFWork(AplicationDBContextcs db)
		{
			_db = db;
			Category = new CategoryRepository(_db);
			Product = new ProductRepository(_db);
			Company = new CompanyRepository(_db);
			ShoppingCart = new ShoppingCartRepository(_db);
			ApplicationUser = new ApplicationUserRepository(_db);
			OrderDetail = new OrderDetailRepository(_db);
			OrderHeader = new OrderHeaderRepository(_db);
		}

		public void save()
		{
			_db.SaveChanges();
		}
	}
}
