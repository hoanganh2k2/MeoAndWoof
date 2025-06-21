using BusinessObject.Models;
using BusinessObject.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace APIMeoAndWoof.Controllers
{
    [Route("api/orderdetail")]
    [ApiController]
    public class OrderDetailController : ControllerBase
    {
        private readonly PostgresContext _db;
        public OrderDetailController(PostgresContext db)
        {
            _db = db;
        }

        [HttpPost]
        public IActionResult Post([FromBody] OrderDetailDTO orderDetailDTO)
        {
            if (orderDetailDTO == null) return BadRequest();

            OrderDetail orderDetail = new()
            {
                OrderId = orderDetailDTO.OrderId,
                ProductId = orderDetailDTO.ProductId,
                Quantity = orderDetailDTO.Quantity,
                UnitPrice = orderDetailDTO.UnitPrice,
                Discount = orderDetailDTO.Discount
            };

            _db.OrderDetails.Add(orderDetail);
            _db.SaveChanges();

            return Ok();
        }
    }
}
