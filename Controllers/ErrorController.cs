
using Microsoft.AspNetCore.Mvc;
namespace Ecommerce_Product.Controllers;
[Route("admin")]
public class ErrorController:Controller
{
[Route("Error/404")]
public IActionResult NotFound()
{   ViewBag.Username=HttpContext.Session.GetString("Username");
    return View();
}

[Route("Error/502")]
 
public IActionResult ServerError()
{    
    return View();
}

// [Route("Maintainance")]
// public IActionResult Maintainance()
// {
//     return View();
// }
}