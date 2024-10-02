using Ecommerce_Product.Repository;
using Ecommerce_Product.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Npgsql.Replication;

namespace Ecommerce_Product.Service;

public class ProductService:IProductRepository
{
    private readonly EcommerceShopContext _context;

    private readonly IWebHostEnvironment _webHostEnv;
  public ProductService(EcommerceShopContext context)
  {
    this._context=context;
  }

  public async Task<IEnumerable<Product>> getAllProduct()
  {
    try
    {
       var products=this._context.Products.Include(p=>p.Brand).Include(p=>p.Category).Include(p=>p.Variants).ToList();
       return products;
    }
    catch(Exception er)
    {
        Console.WriteLine("Get all product:"+er.Message);
    }
    return null;
  }

 public async Task<Product> findProductById(int id)
 {
    var product=await this._context.Products.FirstOrDefaultAsync(p=>p.Id==id);
    return product;
 }

 public async Task<Product> findProductByName(string name)
 {

  var product = await this._context.Products.FirstOrDefaultAsync(p=>p.ProductName==name);
  return product;
 }


public async Task<PageList<Product>> pagingProduct(int page_size,int page)
{
     IEnumerable<Product> all_prod= await this.getAllProduct();

   List<Product> prods=all_prod.OrderByDescending(u=>u.Id).ToList(); 

   //var users=this._userManager.Users;   
   var prod_list=PageList<Product>.CreateItem(prods.AsQueryable(),page,page_size);
   
   return prod_list;
}

public async Task<IEnumerable<Product>> filterProduct(FilterProduct products)
{
   var prod_list=await this.getAllProduct(); 
try
{      
      string start_date=products.StartDate;
    string end_date = products.EndDate;
    string prod_name=products.ProductName;
    string brand = products.Brand;
    string category = products.Category;
    string status = products.Status;
   if(!string.IsNullOrEmpty(prod_name))
   {
    prod_name=prod_name.Trim();
    prod_list= prod_list.Where(c=>c.ProductName.ToLower()==prod_name.ToLower()).ToList();
   }
   if(!string.IsNullOrEmpty(start_date) && string.IsNullOrEmpty(end_date))
   {
    start_date=start_date.Trim();
    prod_list= prod_list.Where(c=>DateTime.TryParse(c.CreatedDate,out var startDate) && DateTime.TryParse(start_date,out var lowerDate) && startDate>=lowerDate).ToList();
   }
   else if(string.IsNullOrEmpty(start_date) && !string.IsNullOrEmpty(end_date))
   { 
    
    end_date=end_date.Trim();
   
    prod_list=prod_list.Where(c=>DateTime.TryParse(c.CreatedDate,out var startDate) && DateTime.TryParse(end_date,out var upperDate) && startDate<=upperDate).ToList();
   }
   else if(!string.IsNullOrEmpty(start_date) && !string.IsNullOrEmpty(end_date))
   {
    start_date=start_date.Trim();
    end_date=end_date.Trim();
    prod_list=prod_list.Where(c=>DateTime.TryParse(c.CreatedDate,out var createdDate)&& DateTime.TryParse(start_date,out var startDate) && DateTime.TryParse(end_date,out var endDate) && createdDate>=startDate && createdDate<=endDate).ToList();
   }
   
   if(!string.IsNullOrEmpty(brand))
   {
   prod_list= prod_list.Where(c=>c.Brand.BrandName==brand).ToList();
   }

   if(!string.IsNullOrEmpty(category))
   {
     prod_list= prod_list.Where(c=>c.Category.CategoryName==category).ToList();
   }
}
catch(Exception er)
{
    Console.WriteLine("Filter Product Exception:"+er.Message);
}
return prod_list;
}

public async Task<int> deleteProduct(int id)
{
 int res_del=0;
 try
 {
   var product = await this.findProductById(id);
   if(product!=null)
   {
  this._context.Products.Remove(product);
  await this.saveChanges();
  res_del=1;
   }
 }
 catch(Exception er)
 {
    Console.WriteLine("Delete Product Exception:"+er.Message);
 }
 return res_del;
}

  public async Task<MemoryStream> exportToExcelProduct()
  {
    using(ExcelPackage excel = new ExcelPackage())
  {
    var worksheet=excel.Workbook.Worksheets.Add("Products");
    worksheet.Cells[1,1].Value="STT";
    worksheet.Cells[1,2].Value="Tên sản phẩm";
    worksheet.Cells[1,3].Value = "Loại sản phẩm";
    worksheet.Cells[1,4].Value="Nhãn hàng";
     worksheet.Cells[1,5].Value="Gía";
    worksheet.Cells[1,6].Value="Trạng thái";
    worksheet.Cells[1,7].Value="Ngày tạo";
    worksheet.Cells[1,8].Value="Ngày cập nhật";


    var products=await this.getAllProduct();
    if(products!=null)
    {
    List<Product> list_product=products.ToList();
    for(int i=0;i<list_product.Count;i++)
    {
    worksheet.Cells[i+2,1].Value=(i+1).ToString();
    
    worksheet.Cells[i+2,2].Value=list_product[i].ProductName;
    
    worksheet.Cells[i+2,3].Value=list_product[i].Category;
    
    worksheet.Cells[i+2,4].Value=list_product[i].Brand;
    
    worksheet.Cells[i+2,5].Value=list_product[i].Price;
    
    worksheet.Cells[i+2,6].Value=list_product[i].Status;
    
    worksheet.Cells[i+2,7].Value=list_product[i].CreatedDate;
    
    worksheet.Cells[i+2,8].Value=list_product[i].UpdatedDate;

    }    
   }
  var stream = new MemoryStream();
  excel.SaveAs(stream);
  stream.Position=0;
  return stream;
  }
  }


public async Task saveChanges()
{
    await this._context.SaveChangesAsync();
}


}