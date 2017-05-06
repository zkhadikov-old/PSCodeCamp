﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MyCodeCamp.Data;
using MyCodeCamp.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using PSCodeCamp.Models;

namespace PSCodeCamp.Controllers
{
    [Route("api/camps/{moniker}/speakers")]
    [ApiVersion("2.0")]
    public class Speakers2Controller : SpeakersController
    {
        public Speakers2Controller(
            ICampRepository repository,
            ILogger<SpeakersController> logger,
            IMapper mapper, UserManager<CampUser> userManager) 
            : base(repository, logger, mapper, userManager)
        {
        }
        public override IActionResult GetWithCount(string moniker, bool includeTalks = false)
        {
            var speakers = includeTalks ? _repository.GetSpeakersByMonikerWithTalks(moniker) : _repository.GetSpeakersByMoniker(moniker);

            return Ok(new {
                currentTime = DateTime.Now,
                count = speakers.Count(),
                results = _mapper.Map<IEnumerable<Speaker2Model>>(speakers)
            });
        }
    }
}
