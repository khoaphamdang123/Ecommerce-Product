using System;
using Ecommerce_Product.Data;
using Ecommerce_Product.Models;
using Org.BouncyCastle.Crypto.Utilities;
namespace Ecommerce_Product.Repository;

public interface IProductRepository
{

  public Task<IEnumerable<Product>> getAllProduct();

  public Task<Product> findProductById(int id);

  public Task<Product> findProductByName(string name);

  public Task<PageList<Product>> pagingProduct(int page_size,int page);

  public Task<PageList<Variant>> pagingVariant(int id,int page_size,int page);

  public Task<IEnumerable<Product>> filterProduct(FilterProduct product);

  public Task<int> deleteProduct(int id);

  public Task<MemoryStream> exportToExcelProduct();

  public Task<int> addNewProduct(AddProductModel product);

  public Task<int> updateProduct(int id,AddProductModel product);

  public Task saveChanges();


}