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

        private void PopulateSatellites(CelestialObject celestial)
        {
            celestial.Satellites = _context
                .CelestialObjects
                .Where(p => p.OrbitedObjectId == celestial.Id)
                .ToList();
        }
    }
}
