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
    }
}