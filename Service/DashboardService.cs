using Ecommerce_Product.Repository;
using Ecommerce_Product.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Npgsql.Replication;
using System.Drawing;

namespace Ecommerce_Product.Service;

public class DashboardService:IDashboardRepository
{
    private readonly EcommerceShopContext _context;

    private readonly Support_Serive.Service _sp_services;
  public DashboardService(EcommerceShopContext context,Support_Serive.Service sp_services)
  {
    this._context=context;
    this._sp_services=sp_services;
  }



  public async Task saveChanges()
  {
    await this._context.SaveChangesAsync();
  }



}