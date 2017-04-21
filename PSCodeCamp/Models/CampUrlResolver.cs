using MyCodeCamp.Data.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using PSCodeCamp.Controllers;

namespace PSCodeCamp.Models
{
    public class CampUrlResolver : IValueResolver<Camp, CampModel, string>
    {
        private IHttpContextAccessor _httpContextAccessor;

        public CampUrlResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        string IValueResolver<Camp, CampModel, string>.Resolve(Camp source, CampModel destination, string destMember, ResolutionContext context)
        {
                var url = (IUrlHelper)_httpContextAccessor.HttpContext.Items[BaseController.UrlHelper];
                return url.Link("CampGet", new { moniker = source.Moniker });
        }
    }
}