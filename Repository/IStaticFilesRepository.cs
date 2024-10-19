using System;
using Ecommerce_Product.Data;
using Ecommerce_Product.Models;
using Org.BouncyCastle.Crypto.Utilities;
namespace Ecommerce_Product.Repository;

public interface IStaticFilesRepository
{

  public Task<IEnumerable<StaticFiles>> getAllStaticFile();

  public Task<StaticFiles> findStaticFileById(int id);

  public Task<PageList<StaticFiles>> pagingStaticFiles(int page_size,int page);

  public Task<int> addPage(StaticFiles file);

  public Task<int> deletePage(int id);

  public Task<int> updatePage(int id,StaticFiles file);

  public Task saveChanges();


}