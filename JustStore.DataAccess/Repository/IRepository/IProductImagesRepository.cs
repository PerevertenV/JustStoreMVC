﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JustStore.Models;

namespace JustStore.DataAccess.Repository.IRepository
{
	public interface IProductImagesRepository : IRepository<ProductImage>
	{
		void Update(ProductImage obj);
	}
}
