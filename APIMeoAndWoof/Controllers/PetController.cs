using BusinessObject.Models;
using BusinessObject.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace APIMeoAndWoof.Controllers
{
    [Route("api/pet")]
    [ApiController]
    public class PetController : ControllerBase
    {
        private readonly PostgresContext _db;

        public PetController(PostgresContext db)
        {
            _db = db;
        }

        [HttpGet]
        [EnableQuery]
        public ActionResult<IEnumerable<PetDTO>> GetPets()
        {
            List<PetDTO> petDTOs = _db.Pets.Select(pet => new PetDTO
            {
                Petid = pet.Petid,
                Petname = pet.Petname,
                Gender = pet.Gender,
                Pettype = pet.Pettype,
                Pettypeid = pet.Pettypeid,
                Petimage = pet.Petimage,
                Userid = pet.Userid,
                Status = pet.Status
            }).ToList();

            return Ok(petDTOs);
        }

        [HttpGet("{id:int}", Name = "GetPet")]
        public ActionResult<PetDTO> GetPets(int id)
        {
            if (id <= 0) return BadRequest();

            PetDTO? petDTO = _db.Pets.Where(p => p.Petid == id)
                            .Select(p => new PetDTO
                            {
                                Petid = p.Petid,
                                Petname = p.Petname,
                                Gender = p.Gender,
                                Pettypeid = p.Pettypeid,
                                Petimage = p.Petimage,
                                Userid = p.Userid,
                                Status = p.Status
                            }).FirstOrDefault();

            if (petDTO == null) return NotFound();

            return Ok(petDTO);
        }

        [HttpPost]
        public ActionResult<PetDTO> Post([FromBody] PetDTO petDTO)
        {
            if (petDTO == null) return BadRequest();

            Pet pet = new()
            {
                Petid = petDTO.Petid,
                Petname = petDTO.Petname,
                Gender = petDTO.Gender,
                Petimage = petDTO.Petimage,
                Pettypeid = petDTO.Pettypeid,
                Userid = petDTO.Userid,
                Status = 1
            };

            _db.Pets.Add(pet);
            _db.SaveChanges();

            return CreatedAtRoute("GetPet", new { id = petDTO.Petid }, petDTO);
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Pet? pet = _db.Pets.Find(id);

            if (pet == null) return NotFound();

            pet.Status = 0;
            _db.Pets.Update(pet);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPut("{id:int}")]
        public IActionResult Put(int id, [FromBody] PetDTO petDTO)
        {
            if (petDTO == null || id != petDTO.Petid) return BadRequest();

            Pet? pet = _db.Pets.Find(id);

            if (pet == null) return NotFound();

            pet.Petname = petDTO.Petname;
            pet.Gender = petDTO.Gender;
            pet.Pettypeid = petDTO.Pettypeid;
            pet.Petimage = petDTO.Petimage;

            _db.Pets.Update(pet);
            _db.SaveChanges();

            return NoContent();
        }
    }
}
