using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext dbContext)
        {
            this._context = dbContext;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = _context.CelestialObjects.Find(id);
            if (celestialObject == null)
            {
                return NotFound();
            }
            else
            {
                celestialObject.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == id).ToList();

                return Ok(celestialObject);
            }
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = _context.CelestialObjects.Where(c => c.Name == name);
            if (celestialObjects.Any() == false)
            {
                return NotFound();
            }
            else
            {
                foreach (var celestialObject in celestialObjects)
                {
                    celestialObject.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == celestialObject.Id).ToList();

                }

                return Ok(celestialObjects.ToList());
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestialObjects = _context.CelestialObjects.ToList();

            foreach (var celestialObject in celestialObjects)
            {
                celestialObject.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == celestialObject.Id).ToList();

            }

            return Ok(celestialObjects);
        }

        [HttpPost]

        public IActionResult Create([FromBody] CelestialObject celestialObjectBody)
        {
            _context.CelestialObjects.Add(celestialObjectBody);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { id = celestialObjectBody.Id }, celestialObjectBody);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var c = _context.CelestialObjects.Find(id);
            if (c == null)
            {
                return NotFound();
            }
            else
            {
                c.Name = celestialObject.Name;
                c.OrbitalPeriod = celestialObject.OrbitalPeriod;
                c.OrbitedObjectId = celestialObject.OrbitedObjectId;
            }
            _context.Update(c);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var c = _context.CelestialObjects.Find(id);
            if (c == null)
            {
                return NotFound();
            }
            else
            {
                c.Name = name;

                _context.Update(c);
                _context.SaveChanges();

                return NoContent();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObjects = _context.CelestialObjects.Where(c => c.Id == id || c.OrbitedObjectId == id);
            if (celestialObjects.Count() == 0)
            {
                return NotFound();
            }
            else
            {
                _context.RemoveRange(celestialObjects);
                _context.SaveChanges();

                return NoContent();
            }
        }
    }
}
