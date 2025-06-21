using BusinessObject.Models;
using BusinessObject.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace APIMeoAndWoof.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriceController : ControllerBase
    {
        private readonly PostgresContext _db;

        public PriceController(PostgresContext db)
        {
            _db = db;
        }

        [HttpGet]
        [EnableQuery]
        public ActionResult<IEnumerable<ServicePriceDTO>> GetPrices()
        {
            List<ServicePriceDTO> priceDTOs = _db.Serviceprices.Select(p => new ServicePriceDTO
            {
                Servicepriceid = p.Servicepriceid,
                Serviceid = p.Serviceid,
                Startdate = p.Startdate,
                Enddate = p.Enddate,
                Pettypeid = p.Pettypeid,
                Numberprice = p.Numberprice,
                Pettype = p.Pettype
            }).ToList();

            return Ok(priceDTOs);
        }

        [HttpGet("{id:int}", Name = "GetPrice")]
        public ActionResult<ServicePriceDTO> GetPrices(int id)
        {
            if (id <= 0) return BadRequest();

            ServicePriceDTO? priceDTO = _db.Serviceprices.Where(p => p.Servicepriceid == id)
                                .Select(p => new ServicePriceDTO
                                {
                                    Servicepriceid = p.Servicepriceid,
                                    Serviceid = p.Serviceid,
                                    Startdate = p.Startdate,
                                    Enddate = p.Enddate,
                                    Pettypeid = p.Pettypeid,
                                    Numberprice = p.Numberprice
                                }).FirstOrDefault();

            if (priceDTO == null) return NotFound();

            return Ok(priceDTO);
        }

        [HttpPost]
        public ActionResult Post([FromBody] ServicePriceDTO priceDTO)
        {
            if (priceDTO == null) return BadRequest();

            Serviceprice price = new()
            {
                Servicepriceid = priceDTO.Servicepriceid,
                Serviceid = priceDTO.Serviceid,
                Startdate = priceDTO.Startdate,
                Enddate = priceDTO.Enddate,
                Pettypeid = priceDTO.Pettypeid,
                Numberprice = priceDTO.Numberprice
            };

            _db.Serviceprices.Add(price);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Serviceprice? price = _db.Serviceprices.Find(id);

            if (price == null) return NotFound();

            _db.Serviceprices.Remove(price);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPut("{id:int}")]
        public IActionResult Put(int id, [FromBody] ServicePriceDTO priceDTO)
        {
            if (priceDTO == null || id != priceDTO.Servicepriceid) return BadRequest();

            Serviceprice? price = _db.Serviceprices.Find(id);

            if (price == null) return NotFound();

            price.Serviceid = priceDTO.Serviceid;
            price.Startdate = priceDTO.Startdate;
            price.Enddate = priceDTO.Enddate;
            price.Pettypeid = priceDTO.Pettypeid;
            price.Numberprice = priceDTO.Numberprice;

            _db.Serviceprices.Update(price);
            _db.SaveChanges();

            return NoContent();
        }
    }
}
