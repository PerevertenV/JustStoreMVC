using JustStore.DataAccess.Data;
using JustStore.DataAccess.Repository.IRepository;
using JustStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace JustStore.DataAccess.Repository
{
	public class ProductImagesRepository : Repository<ProductImage>, IProductImagesRepository 
	{
		private AplicationDBContextcs _db;

		public ProductImagesRepository(AplicationDBContextcs? db) : base(db)
        {
			_db = db;
        }

		public void Update(ProductImage obj)
		{
			_db.ProductImages.Update(obj);
		}
	}
}
