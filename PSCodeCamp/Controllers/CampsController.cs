using Microsoft.AspNetCore.Mvc;
using MyCodeCamp.Data;
using MyCodeCamp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSCodeCamp.Controllers
{
    [Route("api/[controller]")]
    public class CampsController : Controller
    {
        private ICampRepository _reposotory;

        public CampsController(ICampRepository repository)
        {
            _reposotory = repository;
        }

        [HttpGet("")]
        public IActionResult Get()
        {
            var camps = _reposotory.GetAllCamps();


            return Ok(camps);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id, bool includeSpeakers = false)
        {
            try
            {
                Camp camp = null;

                if (includeSpeakers)
                {
                    camp = _reposotory.GetCampWithSpeakers(id);
                }
                else
                {
                    camp = _reposotory.GetCamp(id);
                }

                if (camp == null)
                {
                    return NotFound($"Camp {id} was not found");
                }

                return Ok(camp);
            }
            catch
            {
            }

            return BadRequest();
        }

    }
}
