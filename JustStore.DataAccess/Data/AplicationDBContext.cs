using JustStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JustStore.DataAccess.Data
{
	public class AplicationDBContextcs : IdentityDbContext<IdentityUser>
	{
		public AplicationDBContextcs(DbContextOptions<AplicationDBContextcs> options) :
			base(options)
		{

		}
		public DbSet<Category> Categories { get; set; }
		public DbSet<Product> Products { get; set; }
        public DbSet<ApplicationUser> applicationUser {  get; set; } 
        public DbSet<Company> CompanyUsers {  get; set; } 
        public DbSet<ShoppingCart> ShoppingCarts {  get; set; } 
        public DbSet<OrderHeader> OrderHeaders {  get; set; } 
        public DbSet<OrderDetail> OrderDetails {  get; set; } 


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{

            base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Category>().HasData(
				new Category { ID = 1, Name = "Action", DisplayOrder = 1 },
				new Category { ID = 2, Name = "SciFi", DisplayOrder = 2 },
				new Category { ID = 3, Name = "History", DisplayOrder = 3 },
				new Category { ID = 4, Name = "Fairy Tail", DisplayOrder = 4 }
				);

			modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    ID = 1,
                    Title = "Fortune of Time",
                    Author = "Billy Spark",
                    Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "SWD9999001",
                    ListPrice = 99,
                    Price = 90,
                    Price50 = 85,
                    Price100 = 80,
					CategoryId = 1,
                    ImageUrl = "\\images\\product\\049e5cc8-2882-44a0-8e3b-6304887d569d.jpg"
				},
                new Product
                {
                    ID = 2,
                    Title = "Dark Skies",
                    Author = "Nancy Hoover",
                    Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "CAW777777701",
                    ListPrice = 40,
                    Price = 30,
                    Price50 = 25,
                    Price100 = 20,
					CategoryId = 1,
					ImageUrl = "\\images\\product\\049e5cc8-2882-44a0-8e3b-6304887d569d.jpg"

				},
                new Product
                {
                    ID = 3,
                    Title = "Vanish in the Sunset",
                    Author = "Julian Button",
                    Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "RITO5555501",
                    ListPrice = 55,
                    Price = 50,
                    Price50 = 40,
                    Price100 = 35,
					CategoryId = 1,
					ImageUrl = "\\images\\product\\049e5cc8-2882-44a0-8e3b-6304887d569d.jpg"
				},
                new Product
                {
                    ID = 4,
                    Title = "Cotton Candy",
                    Author = "Abby Muscles",
                    Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "WS3333333301",
                    ListPrice = 70,
                    Price = 65,
                    Price50 = 60,
                    Price100 = 55,
					CategoryId = 3,
					ImageUrl = "\\images\\product\\049e5cc8-2882-44a0-8e3b-6304887d569d.jpg"
				},
                new Product
                {
                    ID = 5,
                    Title = "Rock in the Ocean",
                    Author = "Ron Parker",
                    Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "SOTJ1111111101",
                    ListPrice = 30,
                    Price = 27,
                    Price50 = 25,
                    Price100 = 20,
					CategoryId = 4,
					ImageUrl = "\\images\\product\\049e5cc8-2882-44a0-8e3b-6304887d569d.jpg"
				},
                new Product
                {
                    ID = 6,
                    Title = "Leaves and Wonders",
                    Author = "Laura Phantom",
                    Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                    ISBN = "FOT000000001",
                    ListPrice = 25,
                    Price = 23,
                    Price50 = 22,
                    Price100 = 20,
					CategoryId = 1,
					ImageUrl = "\\images\\product\\049e5cc8-2882-44a0-8e3b-6304887d569d.jpg"
				}
            );

            modelBuilder.Entity<Company>().HasData(
            new Company
            {
                Id = 1,
                Name = "SomeCompany",
                State = "Uk",
                City = "Zhashkiv",
                PhoneNumber = "+380954712339",
                PostalCode = "19201",
                StreetAdress = "Gidnosti 28"
            }
            );

        }
    }
}