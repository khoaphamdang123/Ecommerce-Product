using System;
using Ecommerce_Product.Data;
using Ecommerce_Product.Models;
using Org.BouncyCastle.Crypto.Utilities;
namespace Ecommerce_Product.Repository;

public interface IOrderRepository
{

  public Task<IEnumerable<Order>> getAllOrderList();

  public Task<Order> findOrderById(int id);

  public Task<IEnumerable<Order>> filterOrderList(string status);

  public Task<PageList<Order>> pagingOrderList(int page_size,int page);

  public Task<int> deleteOrder(int id);

  public Task<MemoryStream> exportToExcel();

  public Task saveChanges();


}