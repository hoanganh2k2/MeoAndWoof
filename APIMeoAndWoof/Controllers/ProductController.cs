using BusinessObject.Models;
using BusinessObject.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace APIMeoAndWoof.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly PostgresContext _db;
        public ProductController(PostgresContext db)
        {
            _db = db;
        }

        [HttpGet]
        [EnableQuery]
        public ActionResult<IEnumerable<ServicestoreDTO>> GetProducts()
        {
            List<ServicestoreDTO> servicestoreDTOs = _db.Servicestores.Select(s => new ServicestoreDTO
            {
                Productid = s.Productid,
                Productname = s.Productname,
                Serviceid = s.Serviceid,
                Categoryid = s.Categoryid,
                Description = s.Description,
                Productdiscount = s.Productdiscount,
                Productimage = s.Productimage,
                Statusid = s.Statusid,
                currentPrice = s.Productprices != null ? s.Productprices.FirstOrDefault(pp => pp.Enddate == null).Numberprice : 0
            }).ToList();

            return Ok(servicestoreDTOs);
        }

        [HttpGet("{id:int}", Name = "GetProduct")]
        public ActionResult<ServicestoreDTO> GetProducts(int id)
        {
            if (id <= 0) return BadRequest();

            ServicestoreDTO? servicestoreDTO = _db.Servicestores.Where(s => s.Productid == id)
                                                .Select(s => new ServicestoreDTO
                                                {
                                                    Productid = s.Productid,
                                                    Productname = s.Productname,
                                                    Serviceid = s.Serviceid,
                                                    Categoryid = s.Categoryid,
                                                    Description = s.Description,
                                                    Productdiscount = s.Productdiscount,
                                                    Productimage = s.Productimage,
                                                    Statusid = s.Statusid,
                                                    currentPrice = s.Productprices != null ? s.Productprices.FirstOrDefault(pp => pp.Enddate == null).Numberprice : 0
                                                }).FirstOrDefault();
            if (servicestoreDTO == null) return NotFound();

            return Ok(servicestoreDTO);
        }

        [HttpPost]
        public async Task<ActionResult<ServicestoreDTO>> CreateAsync(ServicestoreDTO servicestoreDTO)
        {
            if (servicestoreDTO == null)
            {
                return BadRequest("Invalid data received");
            }


            Servicestore servicestore = new()
            {
                Productid = servicestoreDTO.Productid,
                Productname = servicestoreDTO.Productname,
                Serviceid = servicestoreDTO.Serviceid,
                Categoryid = servicestoreDTO.Categoryid,
                Description = servicestoreDTO.Description,
                Productdiscount = servicestoreDTO.Productdiscount,
                Productimage = servicestoreDTO.Productimage,
                Statusid = 1
            };

            _db.Servicestores.Add(servicestore);
            _db.SaveChanges();

            return CreatedAtRoute("GetProduct", new { id = servicestoreDTO.Productid }, servicestoreDTO);
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Servicestore? servicestore = _db.Servicestores.Find(id);
            if (servicestore == null) return NotFound();

            servicestore.Statusid = 3;

            _db.Servicestores.Update(servicestore);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPut("{id:int}")]
        public IActionResult Put(int id, [FromBody] ServicestoreDTO servicestoreDTO)
        {
            if (servicestoreDTO == null || id != servicestoreDTO.Productid) return BadRequest();

            Servicestore servicestore = _db.Servicestores.Find(id);
            if (servicestore == null) return NotFound();

            servicestore.Productname = servicestoreDTO.Productname;
            servicestore.Categoryid = servicestoreDTO.Categoryid;
            servicestore.Description = servicestoreDTO.Description;
            servicestore.Productdiscount = servicestoreDTO.Productdiscount;
            servicestore.Productimage = servicestoreDTO.Productimage;

            _db.Servicestores.Update(servicestore);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpGet("filter")]
        public ActionResult<IEnumerable<ServicestoreDTO>> GetProductsByService(int serviceid)
        {
            if (serviceid <= 0)
            {
                return BadRequest("Invalid service id.");
            }

            List<ServicestoreDTO> servicestoreDTOs = _db.Servicestores
                .Where(s => s.Serviceid == serviceid && s.Statusid != 0)
                .Select(s => new ServicestoreDTO
                {
                    Productid = s.Productid,
                    Productname = s.Productname,
                    Serviceid = s.Serviceid,
                    Categoryid = s.Categoryid,
                    Description = s.Description,
                    Productdiscount = s.Productdiscount,
                    Productimage = s.Productimage,
                    Statusid = s.Statusid
                })
                .ToList();

            return Ok(servicestoreDTOs);
        }

        [HttpGet("categories")]
        public ActionResult<IEnumerable<Productcategory>> GetProductCategories()
        {
            List<Productcategory> categories = _db.Productcategories.ToList();
            return Ok(categories);
        }
    }
}
