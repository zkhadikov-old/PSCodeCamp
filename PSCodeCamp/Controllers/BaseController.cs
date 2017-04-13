using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PSCodeCamp.Controllers
{
    public abstract class BaseController : Controller
    {
        public const string UrlHelper = "URLHELPER";
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            context.HttpContext.Items[UrlHelper] = this.Url;
        }

    }
}