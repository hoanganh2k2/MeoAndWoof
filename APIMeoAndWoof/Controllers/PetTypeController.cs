using BusinessObject.Models;
using BusinessObject.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace APIMeoAndWoof.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PetTypeController : ControllerBase
    {
        private readonly PostgresContext _db;

        public PetTypeController(PostgresContext db)
        {
            _db = db;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PetTypeDTO>> GetPetTypes()
        {
            List<PetTypeDTO> petTypeDTOs = _db.Pettypes.Select(p => new PetTypeDTO
            {
                Pettypeid = p.Pettypeid,
                Pettypename = p.Pettypename,
                WeightFrom = p.WeightFrom,
                WeightTo = p.WeightTo
            }).ToList();

            return Ok(petTypeDTOs);
        }

        [HttpGet("{id:int}", Name = "GetPetType")]
        public ActionResult<PetTypeDTO> GetPetTypes(int id)
        {
            if (id <= 0) return BadRequest();

            PetTypeDTO? petTypeDTO = _db.Pettypes.Where(p => p.Pettypeid == id)
                                    .Select(p => new PetTypeDTO
                                    {
                                        Pettypeid = p.Pettypeid,
                                        Pettypename = p.Pettypename,
                                        WeightFrom = p.WeightFrom,
                                        WeightTo = p.WeightTo
                                    }).FirstOrDefault();

            if (petTypeDTO == null) return NotFound();

            return Ok(petTypeDTO);
        }

        [HttpPost]
        public ActionResult<PetTypeDTO> Post([FromBody] PetTypeDTO petTypeDTO)
        {
            if (petTypeDTO == null) return BadRequest();

            if (_db.Pettypes.FirstOrDefault(p => p.Pettypename.ToLower() == petTypeDTO.Pettypename.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Pet Type Name already Exists!");
                return BadRequest(ModelState);
            }

            Pettype pettype = new()
            {
                Pettypeid = petTypeDTO.Pettypeid,
                Pettypename = petTypeDTO.Pettypename,
                WeightFrom = petTypeDTO.WeightFrom,
                WeightTo = petTypeDTO.WeightTo
            };

            _db.Pettypes.Add(pettype);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Pettype? pettype = _db.Pettypes.Find(id);

            if (pettype == null) return NotFound();

            _db.Pettypes.Remove(pettype);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPut("{id:int}")]
        public IActionResult Put(int id, [FromBody] PetTypeDTO petTypeDTO)
        {
            if (petTypeDTO == null || id != petTypeDTO.Pettypeid) return BadRequest();

            Pettype? pettype = _db.Pettypes.Find(id);

            if (pettype == null) return NotFound();

            pettype.Pettypename = petTypeDTO.Pettypename;
            pettype.WeightFrom = petTypeDTO.WeightFrom;
            pettype.WeightTo = petTypeDTO.WeightTo;

            _db.Pettypes.Update(pettype);
            _db.SaveChanges();

            return NoContent();
        }
    }
}
