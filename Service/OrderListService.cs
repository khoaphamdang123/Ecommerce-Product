using Ecommerce_Product.Repository;
using Ecommerce_Product.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Runtime.Serialization;

namespace Ecommerce_Product.Service;

public class OrderListService:IOrderRepository
{
    private readonly EcommerceshopContext _context;

    private readonly Support_Serive.Service _sp_services;
  public OrderListService(EcommerceshopContext context,Support_Serive.Service sp_services)
  {
    this._context=context;
    this._sp_services=sp_services;
  }

  public async Task<IEnumerable<Order>> getAllOrderList()
  {
    var orders=this._context.Orders.Include(c=>c.User).Include(c=>c.Payment).ToList();
    return orders;
  }

  public async Task<Order> findOrderById(int id)
  {
    var order=await this._context.Orders.Include(c=>c.User).Include(c=>c.Payment).Include(c=>c.OrderDetails).ThenInclude(c=>c.Product).FirstOrDefaultAsync(s=>s.Id==id);
    return order;
  }

  public async Task<PageList<Order>> pagingOrderList(int page_size,int page)
  {
   IEnumerable<Order> all_order= await this.getAllOrderList();

   List<Order> list_order=all_order.OrderByDescending(u=>u.Id).ToList(); 

   //var users=this._userManager.Users;   
   var paging_list_order=PageList<Order>.CreateItem(list_order.AsQueryable(),page,page_size);
   
   return paging_list_order;
  }

    public async Task<int> createOrder(AspNetUser user,List<CartModel> cart,Payment payment,string note)
    {
     int created_res=0;
     Console.WriteLine("Cart length here is:"+cart.Count);
     Console.WriteLine("Cart Product name here is:"+cart[0].Product.ProductName);
   try
   {
    Console.WriteLine("Did come to order create section");

    var order=new Order
    {
      Status="Processing",
      Total=cart.Sum(s=>Convert.ToInt32(s.Product.Price)*s.Quantity),
      Shippingaddress=string.IsNullOrEmpty(user.Address2)?user.Address1:user.Address2,
      Userid=user.Id,
      Paymentid=payment.Id,
      Note=note,
      Createddate=DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss")
    };

    await this._context.Orders.AddAsync(order);

    await this.saveChanges();

    Console.WriteLine("did come to here");

    foreach(var product in cart)
    {
      var product_ob=product.Product.Variants;

      List<int> variant_id=new List<int>();
      
      if(product_ob!=null)
      {
        foreach(var variant in product_ob)
        {
          if(variant.Color.Colorname==product.Color && variant.Size.Sizename==product.Size && variant.Version.Versionname==product.Version && variant.Mirror.Mirrorname==product.Mirror)
          { if(!variant_id.Contains(variant.Id))
            {
            variant_id.Add(variant.Id);
            }
          }
        }
      foreach(var id in variant_id)
      {
        var order_detail=new OrderDetail
        {
          Quantity=product.Quantity,
          Price=Convert.ToInt32(product.Product.Price),
          Productid=product.Product.Id,
          Orderid=order.Id,
          VariantId=id
        };
        await this._context.OrderDetails.AddAsync(order_detail);        
      }
      }
    else
    {
      var order_detail=new OrderDetail
      {
        Quantity=product.Quantity,
        Price=Convert.ToInt32(product.Product.Price),
        Productid=product.Product.Id,
        Orderid=order.Id
      };
      await this._context.OrderDetails.AddAsync(order_detail);
    }
    }

    await this.saveChanges();

    Console.WriteLine("Create Product Detail for the order");
 



    created_res=1;
   }
   catch(Exception er)
   {
      Console.WriteLine("Create Order Exception:"+er.InnerException??er.Message);
   }
  return created_res;  
  }


private async Task<IEnumerable<Order>> getOrderByPayment()
{
  var orders=await this._context.Orders.Include(c=>c.Payment).Where(c=>c.Payment.Paymentname=="Bank" ||c.Payment.Paymentname=="Paypal").ToListAsync();
  return orders;
}

public async Task checkOrderStatus()
{
  try
  {
  Console.WriteLine("Get In this check order status function");
  var orders=await this.getOrderByPayment();
  if(orders!=null)
  {
    foreach(var order in orders)
    {
      if(order.Status=="Processing")
      { 
        var created_date=DateTime.Parse(order.Createddate);
        var current_date=DateTime.Now;
        var diff_date=current_date.Subtract(created_date).TotalDays;
        Console.WriteLine("Diff Date:"+diff_date);
        if(diff_date>=3)
        {
          order.Status="Cancelled";
        }
        this._context.Orders.Update(order);
      }
     if(order.Status=="Cancelled")
     {
         var created_date=DateTime.Parse(order.Createddate);
        var current_date=DateTime.Now;
        var diff_date=current_date.Subtract(created_date).TotalDays;
        if(diff_date>=7)
        { 
          var user_email=order.User.Email;
          
          var user=await this._context.AspNetUsers.Include(c=>c.Roles).FirstOrDefaultAsync(s=>s.Email==user_email);
          
          if(user!=null)
          {
          if(user.Roles.FirstOrDefault(c=>c.Name=="Anonymous")!=null)
          {
           this._context.AspNetUsers.Remove(user);
          }
        else
        {
          this._context.Orders.Remove(order);
        }
          }
        
        }
      }
    }
    await this.saveChanges();
  }
  }
  catch(Exception er)
  {
    Console.WriteLine("Check Order Status Exception:"+er.Message);
  }
}

 public async Task<int> updateOrderStatus(int id,string status)
 {
    int updated_res=0;
    try
    {
      var order=await this.findOrderById(id);

      if(order!=null)
      {
        order.Status=status;
        this._context.Orders.Update(order);
        await this.saveChanges();
        updated_res=1;
      }
    }
    catch(Exception er)
    {
      Console.WriteLine("Update Order Status Exception:"+er.Message);
    }
    return updated_res;
 }


  private string generateOrderId(int id)
  {
    string prefix="#ORD";
    string time_stamp=DateTime.Now.ToString("MMddhhmmss");
    string order_id=prefix+time_stamp+id.ToString();
    return order_id;
  }
  public async Task<Order> getLatestOrderByUsername(string user_id)
  {
    var order=await this._context.Orders.Include(c=>c.User).Include(c=>c.Payment).Include(c=>c.OrderDetails).ThenInclude(c=>c.Product).Where(s=>s.User.Id==user_id).OrderByDescending(s=>s.Id).FirstOrDefaultAsync();
    
    string order_id=this.generateOrderId(order.Id);

    order.OrderId=order_id;

    this._context.Orders.Update(order);

    await this.saveChanges();
    
    return order;
  }



  public async Task<int> deleteOrder(int id)
  {
    int deleted_res=0;
    try
    {
      var order=await this.findOrderById(id);

      if(order!=null)
      {
        this._context.Orders.Remove(order);
        await this.saveChanges();
        deleted_res=1;
        return deleted_res;
      }
    }
    catch(Exception er)
    { 
      Console.WriteLine("Delete Order Exception:"+er.Message);
      return deleted_res;
    }
    return deleted_res;
  }
  
  public async Task<MemoryStream> exportToExcel()
  {
    
    using(ExcelPackage excel = new ExcelPackage())
  {
    var worksheet=excel.Workbook.Worksheets.Add("Order");
    
    worksheet.Cells[1,1].Value="STT";
    
    worksheet.Cells[1,2].Value="Tên khách hàng";
    
    worksheet.Cells[1,3].Value = "Phương thức thanh toán";
    
    worksheet.Cells[1,4].Value="Trạng thái đơn hàng";
    
    worksheet.Cells[1,5].Value="Giá trị đơn hàng";
    
    worksheet.Cells[1,6].Value="Địa chỉ giao hàng";
    
    worksheet.Cells[1,7].Value="Ngày tạo";


    var orders =await this.getAllOrderList();
    if(orders!=null)
    {
    List<Order> list_order=orders.ToList();
    for(int i=0;i<list_order.Count;i++)
    {
    worksheet.Cells[i+2,1].Value=(i+1).ToString();
    
    worksheet.Cells[i+2,2].Value=list_order[i].User.UserName;
    
    worksheet.Cells[i+2,3].Value=list_order[i].Payment.Paymentname;
    
    worksheet.Cells[i+2,4].Value=list_order[i].Status;
    
    worksheet.Cells[i+2,5].Value=list_order[i].Total;
        
     worksheet.Cells[i+2,6].Value=list_order[i].Shippingaddress;
    
    worksheet.Cells[i+2,7].Value=list_order[i].Createddate;
    
    }    
   }
  var stream = new MemoryStream();
  excel.SaveAs(stream);
  stream.Position=0;
  return stream;
  }
  }

  public int countOrderStatus(string status)
  {
    int count=0;
    try
    {
      var orders=this._context.Orders.Where(s=>s.Status==status).ToList();
      count=orders.Count;
    }
    catch(Exception er)
    {
    Console.WriteLine("Count Order Status Exception:"+er.Message);
    }
    return count;
  }

  public int countOrder(string id)
  {
    int count=0;
    try
    {
      var orders=this._context.Orders.Where(s=>s.Userid==id).ToList();
      count=orders.Count;
    }
    catch(Exception er)
    {
    Console.WriteLine("Count Order Exception:"+er.Message);
    }
    return count;
  }


  public async Task<int> deleteProductOrderDetail(int id)
  {
    int deleted_res=0;
    try
    {
      var order_detail=await this._context.OrderDetails.FirstOrDefaultAsync(s=>s.Id==id);
      if(order_detail!=null)
      {
        this._context.OrderDetails.Remove(order_detail);
        await this.saveChanges();
        deleted_res=1;
      }
    }
    catch(Exception er)
    {
      return deleted_res;
    }
    return deleted_res;
  }

public async Task<IEnumerable<Order>> filterOrderList(string status)
{
    try
    {
        var orders=this._context.Orders.Include(c=>c.User).Include(c=>c.Payment).Where(s=>s.Status==status).ToList();
        return orders;        
    }
    catch(Exception er)
    {
      Console.WriteLine(er.Message);
    }
    return null;
}


  public async Task saveChanges()
  {
    await this._context.SaveChangesAsync();
  }

}