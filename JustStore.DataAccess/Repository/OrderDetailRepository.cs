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
	public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository 
	{
		private AplicationDBContextcs _db;

		public OrderDetailRepository(AplicationDBContextcs? db) : base(db)
        {
			_db = db;
        }

		public void Update(OrderDetail obj)
		{
			_db.OrderDetails.Update(obj);
		}
	}
}
