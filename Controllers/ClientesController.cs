using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Distribuidora.Data;
using Distribuidora.Models;
using System.Security.Claims;

namespace Distribuidora.Controllers{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClientesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/clientes
        [HttpGet]
        public async Task<IActionResult> GetClientes()
        {
            var clientes = await _context.Clientes
                .Include(c => c.UserElimina)
                .Include(c => c.Pedidos)
                .Select(c => new
                    {
                        c.Id,
                        c.Nombre,
                        c.Telefono,
                        c.Direccion,
                        c.Estado,
                        UserEliminaNombre = c.UserElimina != null ? c.UserElimina.Username : null,
                        c.FechaDesactivacion,
                        c.Pedidos
                    }
                )
                .ToListAsync();

            return Ok(clientes);
        }

        // GET: api/clientes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCliente(int id)
        {
            var cliente = await _context.Clientes
                .Include(c => c.UserElimina)
                .Include(c => c.Pedidos)
                .Where(c => c.Id == id)
                .Select(c => new
                    {
                        c.Id,
                        c.Nombre,
                        c.Telefono,
                        c.Direccion,
                        c.Estado,
                        UserEliminaNombre = c.UserElimina != null ? c.UserElimina.Username : null,
                        c.FechaDesactivacion,
                        c.Pedidos
                    }
                )
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cliente == null)
                return NotFound();

            return Ok(cliente);
        }

        // POST: api/clientes
        [HttpPost]
        public async Task<IActionResult> CreateCliente([FromBody] Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCliente), new { id = cliente.Id }, cliente);
        }

        // PUT: api/clientes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCliente(int id, [FromBody] Cliente updatedCliente)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null || !cliente.Estado)
                return NotFound();

            // Actualizar campos
            cliente.Nombre = updatedCliente.Nombre;
            cliente.Telefono = updatedCliente.Telefono;
            cliente.Direccion = updatedCliente.Direccion;

            await _context.SaveChangesAsync();

            return Ok(cliente);
        }

        // DELETE: api/clientes/5 (eliminación lógica)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null || !cliente.Estado)
                return NotFound();

            // Eliminación lógica
            cliente.Estado = false;
            cliente.FechaDesactivacion = DateTime.UtcNow;

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId)){
                cliente.UserEliminaId = userId;
            }

            await _context.SaveChangesAsync();

            return Ok(cliente);
        }
        
    }
}