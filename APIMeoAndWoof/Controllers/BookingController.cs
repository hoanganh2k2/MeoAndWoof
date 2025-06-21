using BusinessObject.Models;
using BusinessObject.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace APIMeoAndWoof.Controllers
{
	[Route("api/booking")]
	[ApiController]
	public class BookingController : ControllerBase
	{
		private readonly PostgresContext _db;

		public BookingController(PostgresContext db)
		{
			_db = db;
		}

		[HttpGet]
		[EnableQuery]
		public ActionResult<IEnumerable<BookingDTO>> GetBookings()
		{
			List<BookingDTO> bookingDTOs = _db.Bookings
				.Include(b => b.Pet)
					.ThenInclude(p => p.Pettype)
				.Include(b => b.Service)
				.Select(b => new BookingDTO
				{
					Bookingid = b.Bookingid,
					Petid = b.Petid,
					Userid = b.Userid,
					Startbooking = b.Startbooking,
					Endbooking = b.Endbooking,
					Totalprice = b.Totalprice,
					Serviceid = b.Serviceid,
					Bookingdate = b.Bookingdate,
					Note = b.Note,
					Statuspaid = b.Statuspaid,
					Pet = new Pet
					{
						Petid = b.Pet.Petid,
						Petimage = b.Pet.Petimage,
						Pettype = b.Pet.Pettype,
					},
					Service = new Service
					{
						Serviceid = b.Service.Serviceid,
						Servicename = b.Service.Servicename,
						Servicetype = b.Service.Servicetype,
						Userid = b.Service.Userid
					},
					User = new User
					{
						Userimage = b.User.Userimage,
						Fullname = b.User.Fullname
					}
				}).ToList();

			return Ok(bookingDTOs);
		}

		[HttpGet("{id:int}", Name = "GetBookings")]
		public ActionResult<BookingDTO> GetBookings(int id)
		{
			if (id <= 0) return BadRequest();

			BookingDTO? bookingDTO = _db.Bookings.Where(b => b.Bookingid == id)
									.Select(b => new BookingDTO
									{
										Bookingid = b.Bookingid,
										Petid = b.Petid,
										Serviceid = b.Serviceid,
										Userid = b.Userid,
										Startbooking = b.Startbooking,
										Endbooking = b.Endbooking,
										Totalprice = b.Totalprice,
										Bookingdate = b.Bookingdate,
										Note = b.Note,
										Statuspaid = b.Statuspaid,
										Pet = b.Pet,
										Service = b.Service
									}).FirstOrDefault();

			if (bookingDTO == null) return NotFound();

			return Ok(bookingDTO);
		}

		[HttpPost]
		public ActionResult<BookingDTO> Post([FromBody] BookingDTO bookingDTO)
		{
			if (bookingDTO == null) return BadRequest();

			Booking booking = new()
			{
				Bookingid = bookingDTO.Bookingid,
				Petid = bookingDTO.Petid,
				Serviceid = bookingDTO.Serviceid,
				Bookingdate = bookingDTO.Bookingdate,
				Userid = bookingDTO.Userid,
				Startbooking = bookingDTO.Startbooking,
				Endbooking = bookingDTO.Endbooking,
				Totalprice = bookingDTO.Totalprice,
				Note = bookingDTO.Note,
				Statuspaid = bookingDTO.Statuspaid
			};

			_db.Bookings.Add(booking);
			_db.SaveChanges();

			return CreatedAtRoute("GetBookings", new { id = bookingDTO.Bookingid }, bookingDTO);
		}

		[HttpDelete("{id:int}")]
		public IActionResult Delete(int id)
		{
			if (id <= 0) return BadRequest();

			Booking? booking = _db.Bookings.Find(id);

			if (booking == null) return NotFound();

			_db.Bookings.Remove(booking);
			_db.SaveChanges();

			return NoContent();
		}

		[HttpPut("{id:int}")]
		public IActionResult Put(int id, [FromBody] BookingDTO bookingDTO)
		{
			if (bookingDTO == null || id != bookingDTO.Bookingid) return BadRequest();

			Booking? booking = _db.Bookings.Find(id);

			if (booking == null) return NotFound();

			booking.Petid = bookingDTO.Petid;
			booking.Serviceid = bookingDTO.Serviceid;
			booking.Bookingdate = bookingDTO.Bookingdate;
			booking.Note = bookingDTO.Note;
			booking.Statuspaid = bookingDTO.Statuspaid;

			_db.Bookings.Update(booking);
			_db.SaveChanges();

			return NoContent();
		}

		[HttpGet("{petid:int},{serviceid:int}/getprice")]
		public ActionResult<double> GetPriceByPetId(int petid, int serviceid)
		{
			int pettypeid = _db.Pets.Find(petid).Pettypeid;
			double? price = _db.Serviceprices.FirstOrDefault(s => s.Pettypeid == pettypeid && s.Serviceid == serviceid && s.Enddate == null).Numberprice;
			return price != null ? price : 0;
		}
	}
}
