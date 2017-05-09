using System;
using AutoMapper;
using MyCodeCamp.Data.Entities;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PSCodeCamp.Controllers;

namespace PSCodeCamp.Models
{
    internal class TalkLinksResolver : IValueResolver<Talk, TalkModel, ICollection<LinkModel>>
    {
        private IHttpContextAccessor _httpContextAccessor;

        public TalkLinksResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ICollection<LinkModel> Resolve(Talk source, TalkModel destination, ICollection<LinkModel> destMember, ResolutionContext context)
        {
            var url = (IUrlHelper)_httpContextAccessor.HttpContext.Items[BaseController.UrlHelper];

            return new List<LinkModel>()
            {
                new LinkModel()
                {
                    Rel = "Self",
                    Href = url.Link("GetTalk", new { moniker = source.Speaker.Camp.Moniker,
                        speakerId = source.Speaker.Id, id = source.Id})
                },
                new LinkModel()
                {
                    Rel = "Update",
                    Href = url.Link("UpdateTalk", new { moniker = source.Speaker.Camp.Moniker,
                        speakerId = source.Speaker.Id, id = source.Id}),
                    Verb = "PUT"
                },
                new LinkModel()
                {
                    Rel = "Create",
                    Href = url.Link("CreateTalk", new { moniker = source.Speaker.Camp.Moniker,
                        speakerId = source.Speaker.Id}),
                    Verb = "POST"
                },
                new LinkModel()
                {
                    Rel = "Speaker",
                    Href = url.Link("SpeakerGet", new { moniker = source.Speaker.Camp.Moniker,
                        id = source.Speaker.Id})
                }
            };
        }
    }
}