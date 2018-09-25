using Microsoft.AspNetCore.Mvc;
using UniversityRegistrar.Models;
namespace UniversityRegistrar.Controllers
{
    public class HomeController : Controller
    {
      [HttpGet("/")]
      public ActionResult Index()
      {
        return View();
      }

      [HttpGet("/Home/Success")]
      public ActionResult Success()
      {
        return View();
      }
    }
}
