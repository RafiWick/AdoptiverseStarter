using AdoptiverseAPI.DataAccess;
using AdoptiverseAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace AdoptiverseAPI.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class SheltersController : ControllerBase
    {
        private readonly AdoptiverseApiContext _context;
        public SheltersController(AdoptiverseApiContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult GetShelters()
        {
            var shelters = _context.Shelters;
            return new JsonResult(shelters);
        }

        [HttpGet("{id}")]
        public ActionResult GetShelterById(int id)
        {
            var shelter = _context.Shelters.FirstOrDefault(s => s.Id == id);
            if (shelter == null)
            {
                return NotFound();
            }
            return new JsonResult(shelter);
        }

        [HttpPost]
        public ActionResult CreateShelter(Shelter shelter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            shelter.CreatedAt = DateTime.Now.ToUniversalTime();
            shelter.UpdatedAt = null;
            _context.Shelters.Add(shelter);
            _context.SaveChanges();
            Response.StatusCode = 201;
            return new JsonResult(shelter);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteShelter(int id)
        {
            var shelter = _context.Shelters.FirstOrDefault(s => s.Id == id);
            if (shelter == null)
            {
                return NotFound();
            }
            _context.Shelters.Remove(shelter);
            _context.SaveChanges();
            Response.StatusCode = 204;
            return new JsonResult(_context.Shelters);
        }

        [HttpPut("{id}")]
        public ActionResult UpdateShelter(int id, Shelter update)
        {
            var shelter = _context.Shelters.FirstOrDefault(s => s.Id == id);
            if (shelter == null)
            {
                shelter = update;
                shelter.CreatedAt = DateTime.Now.ToUniversalTime();
                shelter.UpdatedAt = null;
                _context.Shelters.Add(shelter);
                _context.SaveChanges();
                Response.StatusCode = 201;
            }
            else
            {
                if(update.FosterProgram != null)
                {
                    shelter.FosterProgram = update.FosterProgram;
                }
                if(update.Rank != null)
                {
                    shelter.Rank = update.Rank;
                }
                if(update.City != null)
                {
                    shelter.City = update.City;
                }
                if(update.Name != null)
                {
                    shelter.Name = update.Name;
                }
                shelter.UpdatedAt = DateTime.Now.ToUniversalTime();
                _context.Shelters.Update(shelter);
                _context.SaveChanges();
                Response.StatusCode = 204;
            }
            return new JsonResult(shelter);
        }
    }
}