using BusinessObject.Models;
using BusinessObject.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace APIMeoAndWoof.Controllers
{
	[Route("api/service")]
	[ApiController]
	public class ServiceController : ControllerBase
	{
		private readonly PostgresContext _db;

		public ServiceController(PostgresContext db)
		{
			_db = db;
		}

		[HttpGet]
		[EnableQuery]
		public ActionResult<IEnumerable<ServiceDTO>> GetServices()
		{
			List<ServiceDTO> serviceDTOs = _db.Services
											.Select(s => new ServiceDTO
											{
												Serviceid = s.Serviceid,
												Servicename = s.Servicename,
												Servicetypeid = s.Servicetypeid,
												Description = s.Description,
												Userid = s.Userid,
												Address = s.Address,
												Status = s.Status,
												Serviceimage = s.Serviceimages.FirstOrDefault().Imagetxt,
												ServiceNumberBooking = s.Bookings.Count(),
												ServiceNumberReview = s.Reviews.Count(),
												Servicestar = s.Reviews.Any() ? s.Reviews.Average(r => r.Rating) : 0
											})
											.ToList();

			return Ok(serviceDTOs);
		}

		[HttpGet("{id:int}", Name = "GetService")]
		public ActionResult<ServiceDTO> GetServices(int id)
		{
			if (id <= 0) return BadRequest();

			ServiceDTO? serviceDTO = _db.Services.Where(s => s.Serviceid == id)
							.Select(s => new ServiceDTO
							{
								Serviceid = s.Serviceid,
								Servicename = s.Servicename,
								Servicetypeid = s.Servicetypeid,
								Description = s.Description,
								Userid = s.Userid,
								Address = s.Address,
								Status = s.Status,
								Serviceimage = s.Serviceimages.FirstOrDefault().Imagetxt,
								ServiceNumberBooking = s.Bookings.Count(),
								ServiceNumberReview = s.Reviews.Count(),
								Servicestar = s.Reviews.Any() ? s.Reviews.Average(r => r.Rating) : 0
							}).FirstOrDefault();

			if (serviceDTO == null) return NotFound();

			return Ok(serviceDTO);
		}

		[HttpGet("filter")]
		public ActionResult<IEnumerable<ServiceDTO>> GetServicesByUserAndType(int userId, int serviceTypeId)
		{
			List<ServiceDTO> services = _db.Services
							  .Where(s => s.Userid == userId && s.Servicetypeid == serviceTypeId)
							  .Select(s => new ServiceDTO
							  {
								  Serviceid = s.Serviceid,
								  Servicename = s.Servicename,
								  Servicetypeid = s.Servicetypeid,
								  Description = s.Description,
								  Userid = s.Userid,
								  Address = s.Address,
								  Status = s.Status,
								  Serviceimage = s.Serviceimages.FirstOrDefault().Imagetxt,
								  ServiceNumberBooking = s.Bookings.Count(),
								  ServiceNumberReview = s.Reviews.Count(),
								  Servicestar = s.Reviews.Any() ? s.Reviews.Average(r => r.Rating) : 0
							  })
							  .ToList();

			return Ok(services);
		}

		[HttpGet("count")]
		public IActionResult GetServiceCountByUserAndType(int userId, int serviceTypeId)
		{
			int serviceCount = _db.Services
								  .Count(s => s.Userid == userId && s.Servicetypeid == serviceTypeId && s.Status == 1);

			return Ok(serviceCount);
		}

		[HttpGet("{id:int}/pettypes", Name = "GetPetTypes")]
		public ActionResult<IEnumerable<PetTypeDTO>> GetPetTypes(int id)
		{
			if (id <= 0) return BadRequest();

			List<PetTypeDTO>? petTypeDTOs = _db.Services.Where(s => s.Servicetypeid == id)
				.SelectMany(s => s.Servicepettypes)
				.Select(s => s.Pettype)
				.Select(p => new PetTypeDTO
				{
					Pettypeid = p.Pettypeid,
					Pettypename = p.Pettypename,
					WeightFrom = p.WeightFrom,
					WeightTo = p.WeightTo
				}).ToList();

			if (petTypeDTOs == null) return NotFound();

			return Ok(petTypeDTOs);
		}

		[HttpPost]
		public ActionResult<ServiceDTO> Post([FromBody] ServiceDTO serviceDTO)
		{
			if (serviceDTO == null) return BadRequest();

			Service service = new()
			{
				Serviceid = serviceDTO.Serviceid,
				Servicename = serviceDTO.Servicename,
				Servicetypeid = serviceDTO.Servicetypeid,
				Description = serviceDTO.Description,
				Userid = serviceDTO.Userid,
				Address = serviceDTO.Address,
				Status = 1
			};

			_db.Services.Add(service);
			_db.SaveChanges();

			return CreatedAtRoute("GetService", new { id = serviceDTO.Serviceid }, serviceDTO);
		}

		[HttpDelete]
		public IActionResult Delete(int id)
		{
			if (id <= 0) return BadRequest();

			Service? service = _db.Services.Find(id);

			if (service == null) return NotFound();

			service.Status = 2;

			_db.Services.Update(service);
			_db.SaveChanges();

			return NoContent();
		}

		[HttpPut("{id:int}")]
		public IActionResult Put(int id, [FromBody] ServiceDTO serviceDTO)
		{
			if (serviceDTO == null || id != serviceDTO.Serviceid) return BadRequest();

			Service? service = _db.Services.Find(id);

			if (service == null) return NotFound();

			serviceDTO.Servicename = serviceDTO.Servicename;
			serviceDTO.Servicetypeid = serviceDTO.Servicetypeid;
			serviceDTO.Description = serviceDTO.Description;
			serviceDTO.Userid = serviceDTO.Userid;
			serviceDTO.Address = serviceDTO.Address;
			serviceDTO.Status = serviceDTO.Status;

			_db.Services.Update(service);
			_db.SaveChanges();

			return NoContent();
		}
	}
}
