
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_Product.Controllers;
[Route("admin")]
public class ErrorController:Controller
{
[Route("Error/404")]
public IActionResult NotFound()
{  
    return View();
}

[Route("Miantainance")]
public IActionResult Maintainance()
{
    return View();
}

}