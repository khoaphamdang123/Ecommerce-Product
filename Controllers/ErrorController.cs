
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_Product.Controllers;
[Route("admin")]
public class ErrorController:Controller
{
[Route("Error/404")]
public IActionResult NotFound()
{  Console.WriteLine("did stay here");
    return View();
}


public IActionResult Maintainance()
{
    return View();
}

}