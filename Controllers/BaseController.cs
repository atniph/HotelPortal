using System.Linq;
using HotelPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HotelPortal.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IPortalService _portalService;
        protected readonly long _fileSizeLimit = 2097152;
        protected readonly string[] _permittedExtensions = { ".jpg", ".png", ".jpeg" };
        public BaseController(IPortalService portalService)
        {
            _portalService = portalService;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);

            // a minden oldalról elérhető információkat össze gyűjtjük
            ViewBag.Cities = _portalService.Cities.ToArray();

        }
    }
}