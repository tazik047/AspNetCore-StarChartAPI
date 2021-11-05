using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var celestial = _context.CelestialObjects.SingleOrDefault(p => p.Id == id);

            if (celestial == null)
            {
                return NotFound();
            }

            PopulateSatellites(celestial);

            return Ok(celestial);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestial = _context
                .CelestialObjects
                .Where(p => p.Name == name)
                .ToList();

            if (celestial.Count == 0)
            {
                return NotFound();
            }

            celestial.ForEach(PopulateSatellites);

            return Ok(celestial);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var celestial = _context
                .CelestialObjects
                .ToList();

            celestial.ForEach(PopulateSatellites);

            return Ok(celestial);
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject model)
        {
            _context.CelestialObjects.Add(model);

            _context.SaveChanges();

            return CreatedAtRoute("GetById", new {id = model.Id}, model);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] CelestialObject model)
        {
            var celestial = _context.CelestialObjects.SingleOrDefault(p => p.Id == id);

            if (celestial == null)
            {
                return NotFound();
            }

            celestial.Name = model.Name;
            celestial.OrbitedObjectId = model.OrbitedObjectId;
            celestial.OrbitalPeriod = model.OrbitalPeriod;

            _context.CelestialObjects.Update(celestial);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var celestial = _context.CelestialObjects.SingleOrDefault(p => p.Id == id);

            if (celestial == null)
            {
                return NotFound();
            }

            celestial.Name = name;

            _context.CelestialObjects.Update(celestial);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestial = _context
                .CelestialObjects
                .Where(p => p.Id == id || p.OrbitedObjectId == id)
                .ToList();

            if (celestial.Count == 0)
            {
                return NotFound();
            }

            _context.CelestialObjects.RemoveRange(celestial);
            _context.SaveChanges();

            return NoContent();
        }

        private void PopulateSatellites(CelestialObject celestial)
        {
            celestial.Satellites = _context
                .CelestialObjects
                .Where(p => p.OrbitedObjectId == celestial.Id)
                .ToList();
        }
    }
}
