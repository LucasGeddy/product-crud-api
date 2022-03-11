using Microsoft.AspNetCore.Mvc;
using product_crud_api.API.DTO;
using product_crud_api.Infra;
using product_crud_api.Infra.Models;

namespace product_crud_api.API.Controllers
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

        [HttpGet]
        public IActionResult GetAll(int pageNumber = 1, int pageSize = 5) // Create request DTO to remove default page rules from controller
        {
            var PagedData = new PagedResponse<IEnumerable<Product>>(
                data: _context.Products
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList(),
                pageNumber: pageNumber,
                pageSize: pageSize);

            return Ok(PagedData);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            return Ok(await _context.Products.FindAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (product.Id != 0)
                return StatusCode(400, new { Error = "Please leave ID blank in order to create a product", product });

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return Ok(product);
        }

        [HttpPut]
        public async Task<IActionResult> Edit(Product product)
        {
            if (product.Id == 0)
                return StatusCode(400, new { Error = "Please specify ID", Product = product });

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return Ok(product);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Product product)
        {
            if (product.Id == 0)
                return StatusCode(400, new { Error = "Please specify ID", Product = product });

            var productToDelete = await _context.Products.FindAsync(product.Id);

            if (productToDelete == null)
                return StatusCode(400, new { Error = "Product not found", Product = product });

            _context.Remove(productToDelete);
            await _context.SaveChangesAsync();
            return Ok(productToDelete);
        }

    }
}