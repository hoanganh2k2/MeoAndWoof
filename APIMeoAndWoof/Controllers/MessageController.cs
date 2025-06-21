using BusinessObject.Models;
using BusinessObject.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace APIMeoAndWoof.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly PostgresContext _db;

        public MessageController(PostgresContext db)
        {
            _db = db;
        }

        [HttpGet]
        public ActionResult<IEnumerable<MessageDTO>> GetMessages()
        {
            List<MessageDTO> messageDTOs = _db.Messages.Select(message => new MessageDTO
            {
                Messageid = message.Messageid,
                Messagetext = message.Messagetext,
                Senderid = message.Senderid,
                Receiverid = message.Receiverid,
                Sendate = message.Sendate
            }).ToList();

            return Ok(messageDTOs);
        }

        [HttpGet("{senderId:int},{receiverId:int}")]
        public ActionResult<IEnumerable<MessageDTO>> GetMessagesOf2People(int senderId, int receiverId)
        {
            List<MessageDTO> messageDTOs = _db.Messages.Where(m => m.Senderid == senderId && m.Receiverid == receiverId)
                .Select(message => new MessageDTO
                {
                    Messageid = message.Messageid,
                    Messagetext = message.Messagetext,
                    Senderid = message.Senderid,
                    Receiverid = message.Receiverid,
                    Sendate = message.Sendate
                }).ToList();

            return Ok(messageDTOs);
        }

        [HttpPost]
        public ActionResult<MessageDTO> Post([FromBody] MessageDTO messageDTO)
        {
            if (messageDTO == null) return BadRequest();

            BusinessObject.Models.Message message = new()
            {
                Messageid = messageDTO.Messageid,
                Messagetext = messageDTO.Messagetext,
                Senderid = messageDTO.Senderid,
                Receiverid = messageDTO.Receiverid,
                Sendate = messageDTO.Sendate
            };

            _db.Messages.Add(message);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0) return BadRequest();

            BusinessObject.Models.Message? message = _db.Messages.Find(id);

            if (message == null) return NotFound();

            _db.Messages.Remove(message);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPut("{iid:int}")]
        public IActionResult Put(int id, [FromBody] MessageDTO messageDTO)
        {
            if (messageDTO == null || id != messageDTO.Messageid) return BadRequest();

            BusinessObject.Models.Message? message = _db.Messages.Find(id);

            if (message == null) return NotFound();

            message.Messagetext = messageDTO.Messagetext;
            message.Sendate = messageDTO.Sendate;

            _db.Messages.Update(message);
            _db.SaveChanges();

            return NoContent();
        }
    }
}
