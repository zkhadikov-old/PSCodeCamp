﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyCodeCamp.Data.Entities;
using PSCodeCamp.Controllers;

namespace PSCodeCamp.Models
{
    public class TalkUrlResolver : IValueResolver<Talk, TalkModel, string>
    {
        private IHttpContextAccessor _httpContextAccessor;

        public TalkUrlResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string Resolve(Talk source, TalkModel destination, string destMember, ResolutionContext context)
        {
            var helper = (IUrlHelper)_httpContextAccessor.HttpContext.Items[BaseController.UrlHelper];
            return helper.Link("GetTalk", new { moniker = source.Speaker.Camp.Moniker, speakerId = source.Speaker.Id, id = source.Id });
        }
    }
}
