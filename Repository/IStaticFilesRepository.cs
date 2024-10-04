using System;
using Ecommerce_Product.Data;
using Ecommerce_Product.Models;
using Org.BouncyCastle.Crypto.Utilities;
namespace Ecommerce_Product.Repository;

public interface IStaticFilesRepository
{

  public Task<IEnumerable<StaticFile>> getAllStaticFile();

  public Task<StaticFile> findStaticFileById(int id);

  public Task<PageList<StaticFile>> pagingStaticFiles(int page_size,int page);

  public Task saveChanges();


}