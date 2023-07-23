

using Microsoft.EntityFrameworkCore;
using TiendaApi.Models;

namespace TiendaApi.Data
{
        public class DataContext : DbContext
        {

            public DataContext(DbContextOptions<DataContext> options) : base(options)
            {

            }
        public DbSet<Product> Product { get; set; }
    }
    }

