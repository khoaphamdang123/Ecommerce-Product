using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Models;
using Microsoft.AspNetCore.Authorization;
using Ecommerce_Product.Repository;
using System.IO;
using System.Text;

namespace Ecommerce_Product.Controllers;
[Route("admin")]
public class CategoryListController : Controller
{
    private readonly ILogger<CategoryListController> _logger;

    private readonly ICategoryRepository _categoryList;

    public CategoryListController(ILogger<CategoryListController> logger,ICategoryRepository categoryList)
    {
        _logger = logger;
       this._categoryList=categoryList; 
    }


 
}
