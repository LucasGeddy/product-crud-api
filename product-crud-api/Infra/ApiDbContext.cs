using Microsoft.EntityFrameworkCore;
using product_crud_api.Infra.Models;

namespace product_crud_api.Infra
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {

        }
        public DbSet<Product> Products { get; set; }
    }
}
