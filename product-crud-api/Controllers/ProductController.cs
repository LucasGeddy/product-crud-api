using Microsoft.AspNetCore.Mvc;
using product_crud_api.Models;

namespace product_crud_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {

        private readonly ApiDbContext _context;

        public ProductController(ApiDbContext dbContext)
        {
            _context = dbContext;
        }

        public IEnumerable<Product> Get()
        {
            return _context.Products.ToList();
        }

        public async Task<Product> Get(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        [HttpPost]
        public int Post(Product product)
        {
            if (product.Id != 0)
                return 0;

            var addedProductId = _context.Products.Add(product).Entity.Id;
            _context.SaveChanges();

            return addedProductId;
        }

        [HttpPut]
        public int Put(Product product)
        {
            if (product.Id == 0)
                return 0;

            var updatedProductId = _context.Products.Update(product).Entity.Id;
            _context.SaveChanges();

            return updatedProductId;
        }

        [HttpDelete]
        public async Task<int> Delete(int id)
        {
            if (id == 0)
                return 0;

            var productToDelete = await _context.Products.FindAsync(id);

            if (productToDelete == null)
                return 0;

            _context.Remove(productToDelete);
            _context.SaveChanges();
            return productToDelete.Id;
        }

    }
}