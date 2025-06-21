using BusinessObject.Models;
using BusinessObject.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace APIMeoAndWoof.Controllers
{
    [Route("api/image")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly PostgresContext _db;

        public ImageController(PostgresContext db)
        {
            _db = db;
        }

        [HttpGet]
        [EnableQuery]
        public ActionResult<IEnumerable<ServiceImageDTO>> GetImages()
        {
            List<ServiceImageDTO> imageDTOs = _db.Serviceimages.Select(image => new ServiceImageDTO
            {
                Serviceimageid = image.Serviceimageid,
                Imagetxt = image.Imagetxt,
                Serviceid = image.Serviceid
            }).ToList();

            return Ok(imageDTOs);
        }

        [HttpGet("{id:int}", Name = "GetImage")]
        public ActionResult<ServiceImageDTO> GetImages(int id)
        {
            if (id <= 0) return BadRequest();

            ServiceImageDTO? imageDTO = _db.Serviceimages.Where(i => i.Serviceimageid == id)
                                .Select(i => new ServiceImageDTO
                                {
                                    Serviceimageid = i.Serviceimageid,
                                    Imagetxt = i.Imagetxt,
                                    Serviceid = i.Serviceid
                                }).FirstOrDefault();

            if (imageDTO == null) return NotFound();

            return Ok(imageDTO);
        }

        [HttpPost]
        public ActionResult<ServiceImageDTO> Post([FromBody] ServiceImageDTO imageDTO)
        {
            if (imageDTO == null) return BadRequest();

            Serviceimage image = new()
            {
                Serviceimageid = imageDTO.Serviceimageid,
                Imagetxt = imageDTO.Imagetxt,
                Serviceid = imageDTO.Serviceid
            };

            _db.Serviceimages.Add(image);
            _db.SaveChanges();

            return CreatedAtRoute("GetImage", new { id = imageDTO.Serviceimageid }, imageDTO);
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Serviceimage? image = _db.Serviceimages.Find(id);

            if (image == null) return NotFound();

            _db.Serviceimages.Remove(image);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPut("{id:int}")]
        public IActionResult Put(int id, [FromBody] ServiceImageDTO imageDTO)
        {
            if (imageDTO == null || id != imageDTO.Serviceimageid) return BadRequest();

            Serviceimage? image = _db.Serviceimages.Find(id);

            if (image == null) return NotFound();

            image.Imagetxt = imageDTO.Imagetxt;
            image.Serviceid = imageDTO.Serviceid;

            _db.Serviceimages.Update(image);
            _db.SaveChanges();

            return NoContent();
        }
    }
}
