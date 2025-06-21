using BusinessObject.Models;
using BusinessObject.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace APIMeoAndWoof.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly PostgresContext _db;
        public OrderController(PostgresContext db)
        {
            _db = db;
        }

        [HttpGet]
        [EnableQuery]
        public ActionResult<IEnumerable<OrderDTO>> GetOrders()
        {
            List<OrderDTO> orderDTOs = _db.Orders.Select(o => new OrderDTO
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                ShippedDate = o.ShippedDate,
                AddressShip = o.AddressShip,
                UserId = o.UserId,
                Total = o.Total,
                StoreId = o.StoreId,
                TransactionType = o.TransactionType,
                Status = o.Status
            }).ToList();

            return Ok(orderDTOs);
        }

        [HttpGet("{id:int}", Name = "GetOrder")]
        public ActionResult<OrderDTO> GetOrder(int id)
        {
            if (id <= 0) return BadRequest();

            OrderDTO? orderDTO = _db.Orders.Where(o => o.OrderId == id)
                                    .Select(o => new OrderDTO
                                    {
                                        OrderId = o.OrderId,
                                        OrderDate = o.OrderDate,
                                        ShippedDate = o.ShippedDate,
                                        AddressShip = o.AddressShip,
                                        UserId = o.UserId,
                                        StoreId = o.StoreId,
                                        Total = o.Total,
                                        TransactionType = o.TransactionType,
                                        Status = o.Status
                                    }).FirstOrDefault();

            if (orderDTO == null) return NotFound();

            return Ok(orderDTO);
        }

        [HttpGet("supplier/{id:int}")]
        public ActionResult<List<OrderDetailDTO>> GetOrderBySupplierId(int id)
        {
            if (id <= 0) return BadRequest();

            List<int> serviceIds = _db.Services
                                .Where(s => s.Userid == id)
                                .Select(s => s.Serviceid)
                                .ToList();


            List<OrderDTO> orders = _db.Orders
                    .Where(o => serviceIds.Contains((int)o.StoreId))
                    .Select(o => new OrderDTO
                    {
                        OrderId = o.OrderId,
                        OrderDate = o.OrderDate,
                        ShippedDate = o.ShippedDate,
                        AddressShip = o.AddressShip,
                        UserId = o.UserId,
                        StoreId = o.StoreId,
                        Total = o.Total,
                        TransactionType = o.TransactionType,
                        Status = o.Status,
                        User = new User
                        {
                            Fullname = o.User.Fullname
                        }
                    })
                    .ToList();

            if (orders == null) return NotFound();

            return Ok(orders);
        }

        [HttpGet("orderid")]
        public ActionResult<long> GetOrderId()
        {
            long id = _db.Orders.Max(o => o.OrderId);
            return Ok(id);
        }

        [HttpGet("details/{id:int}")]
        public ActionResult<List<OrderDetailDTO>> GetOrderDetailById(int id)
        {
            List<OrderDetailDTO> orderDetailDTOs = _db.OrderDetails.Where(o => o.OrderId == id)
                                                    .Select(o => new OrderDetailDTO
                                                    {
                                                        OrderId = o.OrderId,
                                                        ProductId = o.ProductId,
                                                        Quantity = o.Quantity,
                                                        UnitPrice = o.UnitPrice,
                                                        Discount = o.Discount,
                                                        Product = new Servicestore
                                                        {
                                                            Productname = o.Product.Productname,
                                                            Productimage = o.Product.Productimage
                                                        }
                                                    }).ToList();
            return Ok(orderDetailDTOs);
        }

        [HttpPost("PostCOD")]
        public ActionResult<OrderDTO> PostCOD([FromBody] OrderDTO orderDTO)
        {
            return Post(orderDTO, 1);
        }

        [HttpPost("PostVnPay")]
        public ActionResult<OrderDTO> PostVnPay([FromBody] OrderDTO orderDTO)
        {
            return Post(orderDTO, 2);
        }

        [HttpPost("PostPayOS")]
        public ActionResult<OrderDTO> PostPayOS([FromBody] OrderDTO orderDTO)
        {
            return Post(orderDTO, 3);
        }

        private ActionResult<OrderDTO> Post(OrderDTO orderDTO, short transactionType)
        {
            if (orderDTO == null) return BadRequest();

            Order order = new()
            {
                OrderId = orderDTO.OrderId,
                OrderDate = orderDTO.OrderDate,
                ShippedDate = orderDTO.ShippedDate,
                AddressShip = orderDTO.AddressShip,
                UserId = orderDTO.UserId,
                Total = orderDTO.Total,
                StoreId = orderDTO.StoreId,
                TransactionType = transactionType,
                Status = 1
            };

            _db.Orders.Add(order);
            _db.SaveChanges();

            return CreatedAtRoute("GetOrder", new { id = orderDTO.OrderId }, orderDTO);
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Order? order = _db.Orders.Find(id);
            if (order == null) return NotFound();

            _db.Remove(order);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPut("{id:int}")]
        public IActionResult Put(int id, [FromBody] OrderDTO orderDTO)
        {
            if (orderDTO == null || id != orderDTO.OrderId) return BadRequest();

            Order? order = _db.Orders.Find(id);
            if (order == null) return NotFound();

            order.ShippedDate = orderDTO.ShippedDate;
            order.AddressShip = orderDTO.AddressShip;
            order.Total = orderDTO.Total;
            order.TransactionType = orderDTO.TransactionType;
            order.Status = orderDTO.Status;

            _db.Orders.Update(order);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpGet("status/{id:long}/{status:int}")]
        public IActionResult PutStatus(long id, int status)
        {
            Order? order = _db.Orders.Find(id);
            if (order == null) return NotFound();

            order.Status = (short?)status;

            _db.Orders.Update(order);
            _db.SaveChanges();

            return NoContent();

        }
    }
}
