using BusinessObject.Models;
using BusinessObject.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace APIMeoAndWoof.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceTypeController : ControllerBase
    {
        private readonly PostgresContext _db;

        public ServiceTypeController(PostgresContext db)
        {
            _db = db;
        }

        [HttpGet]
        [EnableQuery]
        public ActionResult<IEnumerable<ServiceTypeDTO>> GetServiceTypes()
        {
            List<ServiceTypeDTO> serviceTypeDTOs = _db.Servicetypes.Select(service => new ServiceTypeDTO
            {
                Servicetypeid = service.Servicetypeid,
                Servicetypename = service.Servicetypename,
                Description = service.Description,
                Serviceimage = service.Serviceimage,
                Status = service.Status
            }).ToList();

            return Ok(serviceTypeDTOs);
        }

        [HttpGet("{id:int}", Name = "GetServiceType")]
        public ActionResult<ServiceDTO> GetServiceTypes(int id)
        {
            if (id <= 0) return BadRequest();

            ServiceTypeDTO? serviceTypeDTO = _db.Servicetypes.Where(s => s.Servicetypeid == id)
                                            .Select(s => new ServiceTypeDTO
                                            {
                                                Servicetypeid = s.Servicetypeid,
                                                Servicetypename = s.Servicetypename,
                                                Description = s.Description,
                                                Serviceimage = s.Serviceimage,
                                                Status = s.Status
                                            }).FirstOrDefault();
            if (serviceTypeDTO == null) return NotFound();

            return Ok(serviceTypeDTO);
        }

        [HttpPost]
        public ActionResult<ServiceTypeDTO> Post([FromBody] ServiceTypeDTO serviceTypeDTO)
        {
            if (serviceTypeDTO == null) return BadRequest();

            if (_db.Servicetypes.FirstOrDefault(s => s.Servicetypename == serviceTypeDTO.Servicetypename) != null)
            {
                ModelState.AddModelError("CustomError", "Service Type Name already Exists!");
                return BadRequest(ModelState);
            }

            Servicetype servicetype = new()
            {
                Servicetypeid = serviceTypeDTO.Servicetypeid,
                Servicetypename = serviceTypeDTO.Servicetypename,
                Description = serviceTypeDTO.Description,
                Serviceimage = serviceTypeDTO.Serviceimage
            };

            _db.Servicetypes.Add(servicetype);
            _db.SaveChanges();

            return CreatedAtRoute("GetServiceType", new { id = serviceTypeDTO.Servicetypeid }, serviceTypeDTO);
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Servicetype? serviceType = _db.Servicetypes.Find(id);

            if (serviceType == null) return NotFound();

            _db.Servicetypes.Remove(serviceType);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPut("{id:int}")]
        public IActionResult Put(int id, [FromBody] ServiceTypeDTO serviceTypeDTO)
        {
            if (serviceTypeDTO == null || id != serviceTypeDTO.Servicetypeid) return BadRequest();

            Servicetype? servicetype = _db.Servicetypes.Find(id);

            if (servicetype == null) return NotFound();

            servicetype.Servicetypename = serviceTypeDTO.Servicetypename;

            _db.Servicetypes.Update(servicetype);
            _db.SaveChanges();

            return NoContent();
        }
    }
}
