using DataAccessLayer.Data;
using DataAccessLayer.DataModels;
using DataAccessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly CollectorsKeepDbContext _context;

        public ProductRepository(CollectorsKeepDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Product> GetAll()
            => _context.Products.ToList();

        public Product? GetById(int id)
            => _context.Products.Find(id);

        public void Add(Product product)
        {
            _context.Products.Add(product);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
