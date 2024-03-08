using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace STech_Web.Models
{
    public class DatabaseSTech : DbContext
    {
        public DatabaseSTech() : base("DatabaseSTechEntities") { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Specification> Specifications { get; set; }
        public DbSet<ProductDescription> ProductDescriptions { get; set; }
        public DbSet<ProductDetail> ProductDetails { get; set; }
        public DbSet<ProductImgDetail> ProductImgDetails { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<ProductOutStanding> ProductOutStandings { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillDetail> BillDetails { get; set; }
        public DbSet<WareHouse> WareHouses { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Banner1> Banner1s { get; set; }
    }
}