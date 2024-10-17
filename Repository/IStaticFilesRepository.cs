using System;
using Ecommerce_Product.Data;
using Ecommerce_Product.Models;
using Org.BouncyCastle.Crypto.Utilities;
namespace Ecommerce_Product.Repository;

public interface IStaticFilesRepository
{

  public Task<IEnumerable<Staticfile>> getAllStaticFile();

  public Task<Staticfile> findStaticFileById(int id);

  public Task<PageList<Staticfile>> pagingStaticFiles(int page_size,int page);

  public Task<int> addPage(Staticfile file);

  public Task<int> deletePage(int id);

  public Task<int> updatePage(int id,Staticfile file);

  public Task saveChanges();


}