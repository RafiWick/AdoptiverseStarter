using Microsoft.AspNetCore.Mvc;
using AdoptiverseAPI.DataAccess;
using AdoptiverseAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AdoptiverseAPI.Controllers
{
    [Route("/api/shelters/{shelterId:int}/[controller]")]
    [ApiController]
    public class PetsController : ControllerBase
    {
        private readonly AdoptiverseApiContext _context;
        public PetsController(AdoptiverseApiContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult GetPets(int shelterId)
        {
            var shelter = _context.Shelters.FirstOrDefault(s => s.Id == shelterId);
            if (shelter == null)
            {
                return NotFound();
            }
            var pets = _context.Pets.Where(p => p.ShelterId == shelterId);
            return new JsonResult(pets.OrderBy(p => p.Id));
        }

        [HttpGet("{petId}")]
        public ActionResult GetPetById(int shelterId, int petId)
        {
            var shelter = _context.Shelters.FirstOrDefault(s => s.Id == shelterId);
            if (shelter == null)
            {
                return NotFound();
            }
            var pets = _context.Pets.Where(p => p.ShelterId == shelterId);
            var pet = pets.FirstOrDefault(p => p.Id == petId);
            if (pet == null)
            {
                return NotFound();
            }
            return new JsonResult(pet);
        }

        [HttpPost]
        public ActionResult CreatePet(int shelterId, Pet pet)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var shelter = _context.Shelters.FirstOrDefault(s => s.Id == shelterId);
            if (shelter == null)
            {
                return NotFound();
            }
            pet.CreatedAt = DateTime.UtcNow;
            pet.UpdatedAt = null;
            pet.ShelterId = shelterId;
            _context.Pets.Add(pet);
            _context.SaveChanges();
            Response.StatusCode = 201;
            return new JsonResult(pet);
        }

        [HttpDelete("{petId}")]
        public ActionResult DeteletPet(int shelterId, int petId)
        {
            var shelter = _context.Shelters.FirstOrDefault(s => s.Id == shelterId);
            if (shelter == null)
            {
                return NotFound();
            }
            var pets = _context.Pets.Where(p => p.ShelterId == shelterId);

            var pet = pets.FirstOrDefault(p => p.Id == petId);
            if (pet == null)
            {
                return NotFound();
            }
            _context.Pets.Remove(pet);
            _context.SaveChanges();
            Response.StatusCode = 204;
            pets = _context.Pets.Where(p => p.ShelterId == shelterId);
            return new JsonResult(pets);
        }

        [HttpPut("{petId}")]
        public ActionResult UpdatePet(int shelterId, int petId, Pet update)
        {
            var shelter = _context.Shelters.FirstOrDefault(s => s.Id == shelterId);
            if (shelter == null)
            {
                return NotFound();
            }
            var pet = _context.Pets.FirstOrDefault(p => p.Id == petId);
            if (pet == null)
            {
                pet = update;
                pet.CreatedAt = DateTime.UtcNow;
                pet.UpdatedAt = null;
                pet.ShelterId = shelterId;
                _context.Pets.Add(pet);
                _context.SaveChanges();
                Response.StatusCode = 201;
            }
            else
            {
                pet.Adoptable = update.Adoptable;
                pet.Age = update.Age;
                pet.Breed = update.Breed;
                pet.Name = update.Name;
                pet.UpdatedAt = DateTime.UtcNow;
                _context.Pets.Update(pet);
                _context.SaveChanges();
                Response.StatusCode = 204;
            }
            return new JsonResult(pet);
        }

    }
}
