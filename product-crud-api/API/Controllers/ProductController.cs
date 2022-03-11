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
            var pagedResponse = new PagedResponse<IEnumerable<Product>>(
                data: _context.Products
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList(),
                pageNumber: pageNumber,
                pageSize: pageSize);

            return Ok(pagedResponse);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var productFromDb = await _context.Products.FindAsync(id);

            if (productFromDb == null)
                return NotFound(new Response<Product>(productFromDb, false, "Product not found in database"));

            return Ok(new Response<Product>(productFromDb));
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (product.Id != 0)
                return BadRequest(new Response<Product>(null, false,  "Please leave ID blank in order to create a product"));

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return Ok(new Response<Product>(product));
        }

        [HttpPut]
        public async Task<IActionResult> Edit(Product product)
        {
            if (product.Id == 0)
                return BadRequest(new Response<Product>(null, false, "Please specify ID"));

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return Ok(new Response<Product>(product));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Product product)
        {
            if (product.Id == 0)
                return BadRequest(new Response<Product>(null, false, "Please specify ID"));

            var productToDelete = await _context.Products.FindAsync(product.Id);

            if (productToDelete == null)
                return BadRequest(new Response<Product>(productToDelete, false, "Product not found"));

            _context.Remove(productToDelete);
            await _context.SaveChangesAsync();
            return Ok(new Response<Product>(productToDelete));
        }

    }
}