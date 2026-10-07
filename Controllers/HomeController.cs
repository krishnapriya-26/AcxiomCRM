using Microsoft.AspNetCore.Mvc;
namespace AcxiomCRM.Controllers;
public class HomeController : Controller { [Route("/Home/Error")] public IActionResult Error() => View(); }
