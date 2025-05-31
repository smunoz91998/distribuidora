using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Distribuidora.Data;
using Distribuidora.Models;

namespace Distribuidora.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ClientePedidosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClientePedidosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/clientespedidos/cliente/5
        [HttpGet("cliente/{clienteId}")]
        public async Task<IActionResult> GetPedidosByCliente(int clienteId)
        {
            var pedidos = await _context.ClientePedidos
                .Where(p => p.ClienteId == clienteId)
                .ToListAsync();

            return Ok(pedidos);
        }

        // POST: api/clientespedidos
        [HttpPost]
        public async Task<IActionResult> CreatePedido([FromBody] ClientePedido pedido)
        {
            // Verificar que el cliente existe y está activo
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == pedido.ClienteId && c.Estado);
            if (cliente == null)
                return BadRequest("Cliente no encontrado o inactivo.");

            // Calcular totales si lo deseas
            pedido.Subtotal = pedido.Cantidad * pedido.PrecioUnitario;
            pedido.Total = pedido.Subtotal + pedido.Iva;

            _context.ClientePedidos.Add(pedido);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPedidosByCliente), new { clienteId = pedido.ClienteId }, pedido);
        }
    }
}