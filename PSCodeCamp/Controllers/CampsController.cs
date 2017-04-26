using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyCodeCamp.Data;
using MyCodeCamp.Data.Entities;
using PSCodeCamp.Filters;
using PSCodeCamp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSCodeCamp.Controllers
{
    [Route("api/[controller]")]
    [ValidateModel]
    public class CampsController : BaseController
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

        [HttpGet("{moniker}", Name = "CampGet")]
        public IActionResult Get(string moniker, bool includeSpeakers = false)
        {
            try
            {
                Camp camp = null;

                if (includeSpeakers)
                {
                    camp = _reposotory.GetCampByMonikerWithSpeakers(moniker);
                }
                else
                {
                    camp = _reposotory.GetCampByMoniker(moniker);
                }

                if (camp == null)
                {
                    return NotFound($"Camp {moniker} was not found");
                }

                return Ok(_mapper.Map<CampModel>(camp));
            }
            catch
            {
            }

            return BadRequest();
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody]CampModel model)
        {
            try
            {
                _logger.LogInformation("Create a new Code Camp");

                var camp = _mapper.Map<Camp>(model);

                _reposotory.Add(camp);
                if (await _reposotory.SaveAllAsync())
                {
                    var newUri = Url.Link("CampGet", new { moniker = model.Moniker });
                    return Created(newUri, _mapper.Map<CampModel>(camp));
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

        [HttpPut("{moniker}")]
        public async Task<IActionResult> Put(string moniker, [FromBody] CampModel model)
        {
            try
            {
                var oldCamp = _reposotory.GetCampByMoniker(moniker);
                if (oldCamp == null)
                {
                    return NotFound($"Could not found a camp with an moniker of {moniker}");
                }

                _mapper.Map(model, oldCamp);

                if (await _reposotory.SaveAllAsync())
                {
                    return Ok(_mapper.Map<CampModel>(model));
                }
                else
                {
                    _logger.LogWarning($"Could not save Camp with moniker {moniker} to the database");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Couldn't update Campw with { moniker} to the database");
                throw;
            }

            return BadRequest("Couldn't update Camp");
        }

        [HttpDelete("{moniker}")]
        public async Task<IActionResult> Delete(string moniker)
        {
            try
            {
                var oldCamp = _reposotory.GetCampByMoniker(moniker);
                if (oldCamp == null)
                {
                    return NotFound($"Could not found a camp with an ID of {moniker}");
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
