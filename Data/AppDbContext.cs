using Microsoft.EntityFrameworkCore;
using Distribuidora.Models;

namespace Distribuidora.Data{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<ClientePedido> ClientePedidos { get; set; }

    }
}