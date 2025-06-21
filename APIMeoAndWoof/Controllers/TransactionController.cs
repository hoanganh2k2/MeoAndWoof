using BusinessObject.Models;
using BusinessObject.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace APIMeoAndWoof.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly PostgresContext _db;

        public TransactionController(PostgresContext db)
        {
            _db = db;
        }

        [HttpGet]
        public ActionResult<IEnumerable<TransactionDTO>> GetTransactions()
        {
            List<TransactionDTO> transactionDTOs = _db.Transactions.Select(t => new TransactionDTO
            {
                Transactionid = t.Transactionid,
                Bookingid = t.Bookingid,
                Transactiondate = t.Transactiondate,
                Paymentmethod = t.Paymentmethod,
                Amountpaid = t.Amountpaid
            }).ToList();

            return Ok(transactionDTOs);
        }

        [HttpGet("{id:int}", Name = "GetTransaction")]
        public ActionResult<TransactionDTO> GetTransaction(int id)
        {
            if (id <= 0) return BadRequest();

            TransactionDTO? transactionDTO = _db.Transactions.Where(t => t.Transactionid == id)
                                            .Select(t => new TransactionDTO
                                            {
                                                Transactionid = t.Transactionid,
                                                Bookingid = t.Bookingid,
                                                Transactiondate = t.Transactiondate,
                                                Paymentmethod = t.Paymentmethod,
                                                Amountpaid = t.Amountpaid
                                            }).FirstOrDefault();

            if (transactionDTO == null) return NotFound();

            return Ok(transactionDTO);
        }

        [HttpPost]
        public ActionResult<TransactionDTO> Post([FromBody] TransactionDTO transactionDTO)
        {
            if (transactionDTO == null) return BadRequest();

            Transaction transaction = new()
            {
                Transactionid = transactionDTO.Transactionid,
                Bookingid = transactionDTO.Bookingid,
                Transactiondate = transactionDTO.Transactiondate,
                Paymentmethod = transactionDTO.Paymentmethod,
                Amountpaid = transactionDTO.Amountpaid
            };

            _db.Transactions.Add(transaction);
            _db.SaveChanges();

            return CreatedAtRoute("GetTransaction", new { id = transactionDTO.Transactionid }, transactionDTO);
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0) return BadRequest();

            Transaction? transaction = _db.Transactions.Find(id);

            if (transaction == null) return NotFound();

            _db.Transactions.Remove(transaction);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPut("{id:int}")]
        public IActionResult Put(int id, [FromBody] TransactionDTO transactionDTO)
        {
            if (transactionDTO == null || id != transactionDTO.Transactionid) return BadRequest();

            Transaction? transaction = _db.Transactions.Find(id);

            if (transaction == null) return NotFound();

            transaction.Transactiondate = transactionDTO.Transactiondate;
            transaction.Paymentmethod = transactionDTO.Paymentmethod;
            transaction.Amountpaid = transactionDTO.Amountpaid;

            _db.Transactions.Update(transaction);
            _db.SaveChanges();

            return NoContent();
        }
    }
}
