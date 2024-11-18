
using Ecommerce_Product.Repository;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_Product.Models;
using System.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace Ecommerce_Product.Controllers;
public class BlogController:BaseController
{
 

private readonly ICategoryListRepository _category;

private readonly ILogger<BlogController> _logger;

private readonly IBlogRepository _blog;

public BlogController(ICategoryListRepository category,IBlogRepository blog,ILogger<BlogController> logger):base(category)
{
    this._blog=blog;
    this._category=category;
    this._logger=logger;
}

[Route("blog")]

public async Task<IActionResult> Blog()
{
    try
    {
        var blogs=await this._blog.getAllBlog();
        return View("~/Views/ClientSide/Blog/Blog.cshtml",blogs);
    }
    catch(Exception er)
    {
        this._logger.LogTrace("Get Manual File List Exception:"+er.Message);
    }
    return View("~/Views/ClientSide/Blog/Blog.cshtml");

}

[Route("blog/blog_detail/{id}")]

public async Task<IActionResult> BlogDetail(int id)
{  
   var blog=await this._blog.findBlogById(id);

   blog.Content=HttpUtility.HtmlDecode(blog.Content);
    
   return View("~/Views/ClientSide/Blog/BlogDetail.cshtml",blog);
}


}