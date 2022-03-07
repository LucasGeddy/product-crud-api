using Microsoft.EntityFrameworkCore;
using product_crud_api.Models;

namespace product_crud_api
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {

        }
        public DbSet<Product> Products { get; set; }
    }
}
