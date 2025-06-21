using BusinessObject.Models;
using BusinessObject.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace APIMeoAndWoof.Controllers
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly PostgresContext _db;

        public CommentController(PostgresContext db)
        {
            _db = db;
        }

        [HttpGet]
        [EnableQuery]
        public ActionResult<IEnumerable<CommentDTO>> GetComments()
        {
            List<CommentDTO> commentDTOs = _db.Comments
                                              .Select(c => new CommentDTO
                                              {
                                                  Commentid = c.Commentid,
                                                  Serviceid = c.Serviceid,
                                                  Userid = c.Userid,
                                                  Content = c.Content,
                                                  CreateAt = c.CreateAt,
                                                  UpdateAt = c.UpdateAt
                                              })
                                              .ToList();

            return Ok(commentDTOs);
        }

        [HttpGet("{id:int}", Name = "GetComment")]
        public ActionResult<CommentDTO> GetComment(int id)
        {
            if (id <= 0) return BadRequest();

            CommentDTO? commentDTO = _db.Comments.Where(c => c.Commentid == id)
                            .Select(c => new CommentDTO
                            {
                                Commentid = c.Commentid,
                                Serviceid = c.Serviceid,
                                Userid = c.Userid,
                                Content = c.Content,
                                CreateAt = c.CreateAt,
                                UpdateAt = c.UpdateAt
                            }).FirstOrDefault();

            if (commentDTO == null) return NotFound();

            return Ok(commentDTO);
        }

        [HttpGet("filter")]
        public ActionResult<IEnumerable<CommentDTO>> GetCommentsByServiceAndUser(int serviceId)
        {
            List<CommentDTO> comments = _db.Comments
                              .Where(c => c.Serviceid == serviceId)
                              .Select(c => new CommentDTO
                              {
                                  Commentid = c.Commentid,
                                  Serviceid = c.Serviceid,
                                  Userid = c.Userid,
                                  Content = c.Content,
                                  CreateAt = c.CreateAt,
                                  UpdateAt = c.UpdateAt
                              })
                              .ToList();

            return Ok(comments);
        }


        [HttpPost]
        public ActionResult<CommentDTO> Post123([FromBody] CommentDTO commentDTO)
        {
            if (commentDTO == null) return BadRequest();

            Comment comment = new()
            {
                Serviceid = commentDTO.Serviceid,
                Userid = commentDTO.Userid,
                Content = commentDTO.Content,
                CreateAt = commentDTO.GetUnspecifiedCreateAt(),
                UpdateAt = commentDTO.GetUnspecifiedUpdateAt()
            };

            _db.Comments.Add(comment);
            _db.SaveChanges();

            return CreatedAtRoute("GetComment", new { id = comment.Commentid }, commentDTO);
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(long id)
        {
            if (id <= 0) return BadRequest();

            Comment? comment = _db.Comments.Find(id);

            if (comment == null) return NotFound();

            _db.Comments.Remove(comment);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPut("{id:int}")]
        public IActionResult Put(int id, [FromBody] CommentDTO commentDTO)
        {
            if (commentDTO == null || id != commentDTO.Commentid) return BadRequest();

            Comment? comment = _db.Comments.Find(id);

            if (comment == null) return NotFound();

            comment.Serviceid = commentDTO.Serviceid;
            comment.Userid = commentDTO.Userid;
            comment.Content = commentDTO.Content;
            comment.CreateAt = commentDTO.CreateAt;
            comment.UpdateAt = commentDTO.UpdateAt;

            _db.Comments.Update(comment);
            _db.SaveChanges();

            return NoContent();
        }
    }
}