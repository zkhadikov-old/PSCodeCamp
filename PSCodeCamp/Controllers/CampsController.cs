using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyCodeCamp.Data;
using MyCodeCamp.Data.Entities;
using PSCodeCamp.Models;
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
        private ILogger<CampsController> _logger;
        private IMapper _mapper;

        public CampsController(ICampRepository repository, ILogger<CampsController> logger, IMapper mapper)
        {
            _reposotory = repository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("")]
        public IActionResult Get()
        {
            var camps = _reposotory.GetAllCamps();


            return Ok(_mapper.Map<IEnumerable<CampModel>>(camps));
        }

        [HttpGet("{id}", Name = "CampGet")]
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

                return Ok(_mapper.Map<CampModel>(camp, opt => opt.Items["UrlHelper"] = this.Url));
            }
            catch
            {
            }

            return BadRequest();
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Camp model)
        {
            try
            {
                _logger.LogInformation("Create a new Code Camp");

                _reposotory.Add(model);
                if (await _reposotory.SaveAllAsync())
                {
                    var newUri = Url.Link("CampGet", new { id = model.Id });
                    return Created(newUri, model);
                }
                else
                {
                    _logger.LogWarning("Could not save Camp to the database");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Threw exception thile saving Camp: {ex}");
            }

            return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Camp model)
        {
            try
            {
                var oldCamp = _reposotory.GetCamp(id);
                if (oldCamp == null)
                {
                    return NotFound($"Could not found a camp with an ID of {id}");
                }

                // map model to the oldcamp
                oldCamp.Name = model.Name ?? oldCamp.Name;
                oldCamp.Description = model.Description ?? oldCamp.Description;
                oldCamp.Location = model.Location ?? oldCamp.Location;
                oldCamp.Length = model.Length > 0 ? model.Length : oldCamp.Length;

                if (await _reposotory.SaveAllAsync())
                {
                    return Ok(oldCamp);
                }
                else
                {
                    _logger.LogWarning("Could not save Camp to the database");
                }

            }
            catch (Exception)
            {

                throw;
            }

            return BadRequest("Couldn't update Camp");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var oldCamp = _reposotory.GetCamp(id);
                if (oldCamp == null)
                {
                    return NotFound($"Could not found a camp with an ID of {id}");
                }

                _reposotory.Delete(oldCamp);

                if (await _reposotory.SaveAllAsync())
                {
                    return Ok();
                }
            }
            catch (System.Exception)
            {

                throw;
            }

            return BadRequest("Couldn't delete Camp");
        }
    }
}
