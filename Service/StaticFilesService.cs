using Ecommerce_Product.Repository;
using Ecommerce_Product.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Npgsql.Replication;
using System.Drawing;

namespace Ecommerce_Product.Service;

public class StaticFilesService:IStaticFilesRepository
{
    private readonly EcommerceShopContext _context;

    private readonly Support_Serive.Service _sp_services;
  public StaticFilesService(EcommerceShopContext context,Support_Serive.Service sp_services)
  {
    this._context=context;
    this._sp_services=sp_services;
  }

  public async Task<IEnumerable<StaticFile>> getAllStaticFile()
  {
    var static_files=this._context.StaticFiles.ToList();
    return static_files;
  }

  public async Task<StaticFile> findStaticFileById(int id)
  {
    var static_file=await this._context.StaticFiles.FirstOrDefaultAsync(s=>s.Id==id);
    return static_file;
  }

  public async Task<PageList<StaticFile>> pagingStaticFiles(int page_size,int page)
  {
    IEnumerable<StaticFile> all_files= await this.getAllStaticFile();

   List<StaticFile> list_file=all_files.OrderByDescending(u=>u.Id).ToList(); 

   //var users=this._userManager.Users;   
   var paging_list_file=PageList<StaticFile>.CreateItem(list_file.AsQueryable(),page,page_size);
   
   return paging_list_file;
  }

  public async Task saveChanges()
  {
    await this._context.SaveChangesAsync();
  }

}