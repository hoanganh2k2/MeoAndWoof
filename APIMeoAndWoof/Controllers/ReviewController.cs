using BusinessObject.Models;
using BusinessObject.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace APIMeoAndWoof.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly PostgresContext _db;

        public ReviewController(PostgresContext db)
        {
            _db = db;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ReviewDTO>> GetReviews()
        {
            List<ReviewDTO> reviewDTOs = _db.Reviews.Select(r => new ReviewDTO
            {
                Reviewid = r.Reviewid,
                Serviceid = r.Serviceid,
                Userid = r.Userid,
                Rating = r.Rating,
                Reviewtext = r.Reviewtext,
                Reviewdate = r.Reviewdate
            }).ToList();

            return Ok(reviewDTOs);
        }

        [HttpGet("{id:int}", Name = "GetReview")]
        public ActionResult<ReviewDTO> GetReview(int id)
        {
            if (id <= 0) return BadRequest();

            ReviewDTO? reviewDTO = _db.Reviews.Where(r => r.Userid == id)
                                    .Select(r => new ReviewDTO
                                    {
                                        Reviewid = r.Reviewid,
                                        Serviceid = r.Serviceid,
                                        Userid = r.Userid,
                                        Rating = r.Rating,
                                        Reviewtext = r.Reviewtext,
                                        Reviewdate = r.Reviewdate
                                    }).FirstOrDefault();

            if (reviewDTO == null) return BadRequest();

            return Ok(reviewDTO);
        }

        [HttpPost]
        public ActionResult<ReviewDTO> Post([FromBody] ReviewDTO reviewDTO)
        {
            if (reviewDTO == null) return BadRequest();

            Review review = new()
            {
                Reviewid = reviewDTO.Reviewid,
                Serviceid = reviewDTO.Serviceid,
                Userid = reviewDTO.Userid,
                Rating = reviewDTO.Rating,
                Reviewtext = reviewDTO.Reviewtext,
                Reviewdate = reviewDTO.Reviewdate
            };

            _db.Reviews.Add(review);
            _db.SaveChanges();

            return CreatedAtRoute("GetReview", new { id = reviewDTO.Reviewid }, reviewDTO);
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Review? review = _db.Reviews.Find(id);

            if (review == null) return NotFound();

            _db.Reviews.Remove(review);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPut("{id:int}")]
        public IActionResult Put(int id, [FromBody] ReviewDTO reviewDTO)
        {
            if (reviewDTO == null || id != reviewDTO.Reviewid) return BadRequest();

            Review? review = _db.Reviews.Find(id);

            if (review == null) return NotFound();

            review.Rating = reviewDTO.Rating;
            review.Reviewtext = reviewDTO.Reviewtext;
            review.Reviewdate = reviewDTO.Reviewdate;

            _db.Reviews.Update(review);
            _db.SaveChanges();

            return NoContent();
        }
    }
}
