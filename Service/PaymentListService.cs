using Ecommerce_Product.Repository;
using Ecommerce_Product.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Npgsql.Replication;
using System.Drawing;

namespace Ecommerce_Product.Service;

public class PaymentListService:IPaymentRepository
{
    private readonly EcommerceShopContext _context;

    private readonly Support_Serive.Service _sp_services;
      
  public PaymentListService(EcommerceShopContext context,Support_Serive.Service sp_services)
  {
    this._context=context;
    this._sp_services=sp_services;
  }

  public async Task<IEnumerable<Payment>> getAllPayment()
  {
    var payments=this._context.Payments.ToList();
    return payments;
  }

  public async Task<Payment> findPaymentById(int id)
  {
    var payment=await this._context.Payments.FirstOrDefaultAsync(s=>s.Id==id);
    return payment;
  }

    public async Task<PageList<Payment>> pagingPayment(int page_size,int page)
    {
        IEnumerable<Payment> all_payment= await this.getAllPayment();

   List<Payment> list_payment=all_payment.OrderByDescending(u=>u.Id).ToList(); 

   //var users=this._userManager.Users;   
   var paging_list_payment=PageList<Payment>.CreateItem(list_payment.AsQueryable(),page,page_size);
   
   return paging_list_payment;
}

  public async Task<int> deletePaymentMethod(int id)
  {
    int deleted_res=0;
    try
    {
      var payment=await this.findPaymentById(id);
      if(payment!=null)
      {
        this._context.Payments.Remove(payment);
        await this.saveChanges();
        deleted_res=1;
      }
      else
      {
        deleted_res=-1;
      }
    }
    catch(Exception er)
    {
      return -1;
    }
    return deleted_res;
  }


  public async Task<int>addPaymentMethod(Payment payment)
  {
    int created_res=0;
    try
    {
      var check_payment_exist=await this.findPaymentById(payment.Id);
      if(check_payment_exist!=null)
      {
        created_res=-1;
        return created_res;
      }
      string created_date=DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss");
      string updated_date = DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss");    
      var payment_method=new Payment{Paymentname=payment.Paymentname,Createddate=created_date,Updateddate=updated_date};
      await this._context.Payments.AddAsync(payment_method);
      await this.saveChanges();
      created_res=1;
    }
    catch(Exception er)
    {
      return -1;
    }
    return created_res;
  }


  public async Task saveChanges()
  {
    await this._context.SaveChangesAsync();
  }

}