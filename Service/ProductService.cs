using Ecommerce_Product.Repository;
using Ecommerce_Product.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Npgsql.Replication;
using System.Drawing;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Ecommerce_Product.Service;

public class ProductService:IProductRepository
{
    private readonly EcommerceshopContext _context;

    private readonly IWebHostEnvironment _webHostEnv;

    private readonly Support_Serive.Service _sp_services;
  public ProductService(EcommerceshopContext context,IWebHostEnvironment webHostEnv,Support_Serive.Service sp_services)
  {
    this._context=context;
    this._webHostEnv=webHostEnv;
    this._sp_services=sp_services;
  }

  public async Task<IEnumerable<Product>> getAllProduct()
  {
    try
    {
       var products=this._context.Products.Include(p=>p.Brand).Include(p=>p.Category).Include(c=>c.SubCat).Include(p=>p.Variants).ToList();
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
    var product=await this._context.Products.Include(c=>c.Category).Include(c=>c.Brand).Include(i=>i.ProductImages).Include(c=>c.Variants).ThenInclude(v=>v.Color).Include(c=>c.Variants).ThenInclude(v=>v.Size).Include(c=>c.Variants).ThenInclude(c=>c.Version).Include(c=>c.Variants).ThenInclude(c=>c.Mirror).FirstOrDefaultAsync(p=>p.Id==id);
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

public async Task<IEnumerable<Product>>getProductBySubCategory(int sub_cat)
{
  var products=await this._context.Products.Include(c=>c.Category).Include(c=>c.Brand).Include(c=>c.SubCat).Include(c=>c.Variants).Where(c=>c.SubCatId==sub_cat).ToListAsync();
  return products;
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
    Console.WriteLine(start_date);
    Console.WriteLine("Brand name here:"+brand);
    Console.WriteLine("Category name here:"+category);
    Console.WriteLine("Status here:"+status);
   if(!string.IsNullOrEmpty(prod_name))
   {
    prod_name=prod_name.Trim();
    prod_list= prod_list.Where(c=>c.ProductName.ToLower()==prod_name.ToLower()).ToList();
   }
   if(!string.IsNullOrEmpty(start_date) && string.IsNullOrEmpty(end_date))
   {
    start_date=start_date.Trim();
    prod_list= prod_list.Where(c=>DateTime.TryParse(c.CreatedDate,out var startDate) && DateTime.TryParse(start_date,out var lowerDate) && startDate.Date==lowerDate.Date).ToList();
   }
   else if(string.IsNullOrEmpty(start_date) && !string.IsNullOrEmpty(end_date))
   { 
    
    end_date=end_date.Trim();
   
    prod_list=prod_list.Where(c=>DateTime.TryParse(c.CreatedDate,out var startDate) && DateTime.TryParse(end_date,out var upperDate) && startDate.Date==upperDate.Date).ToList();
   }
   else if(!string.IsNullOrEmpty(start_date) && !string.IsNullOrEmpty(end_date))
   {
    start_date=start_date.Trim();
    end_date=end_date.Trim();
    prod_list=prod_list.Where(c=>DateTime.TryParse(c.CreatedDate,out var createdDate)&& DateTime.TryParse(start_date,out var startDate) && DateTime.TryParse(end_date,out var endDate) && createdDate>=startDate && createdDate<=endDate).ToList();
   }
   
   if(!string.IsNullOrEmpty(brand))
   {Console.WriteLine("filter brand here");
   prod_list= prod_list.Where(c=>c.Brand.Id==Convert.ToInt32(brand)).ToList();
   }

   if(!string.IsNullOrEmpty(category))
   {Console.WriteLine("filter cat here");
     prod_list= prod_list.Where(c=>c.Category.Id==Convert.ToUInt32(category)).ToList();
   }
   if(!string.IsNullOrEmpty(status))
   {Console.WriteLine("filter status here");
       prod_list= prod_list.Where(c=>c.Status==status).ToList();
   }
}
catch(Exception er)
{
    Console.WriteLine("Filter Product Exception:"+er.Message);
}
return prod_list;
}

public IEnumerable<ProductImage> findProductImageByProductId(int id)
{
 var product_img=this._context.ProductImages.Where(p=>p.Productid==id).ToList();
 return product_img;
}

public async Task<int> deleteProduct(int id)
{
 int res_del=0;
 try
 {
   var product = await this.findProductById(id);
   
   var product_img_ob=this.findProductImageByProductId(id);

   string front_file=product.Frontavatar;
   
   string back_file = product.Backavatar;
  
   List<string> avatar=new List<string>{front_file,back_file};

   if(product!=null)
   {
  this._context.Products.Remove(product);
  await this.saveChanges();  
  res_del=1;
   }

  foreach(string file in avatar)
  {
    if(!string.IsNullOrEmpty(file))
    {
      int val= await this._sp_services.removeFiles(file);
    }
  }

   foreach(var ob in product_img_ob)
   {
    int val =await this._sp_services.removeFiles(ob.Avatar);
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
     worksheet.Cells[1,5].Value="Giá";
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


public async Task<SubCategory> findCatIdBySubId(int id)
{
  var sub_cat=await this._context.SubCategory.Include(c=>c.Category).FirstOrDefaultAsync(c=>c.Id==id);
  return sub_cat;
}


public async Task<bool> checkProductExist(string product_name)
{
  var product=await this._context.Products.FirstOrDefaultAsync(c=>c.ProductName==product_name);
  if(product!=null)
  {
    return true;
  }
  return false;
}

public async Task<int> addNewProduct(AddProductModel model)
{ int created_res=0;
  try
  {


   string product_name=model.ProductName;


   Console.WriteLine("Product name:"+product_name);
   
   int price=model.Price;

      Console.WriteLine("Price:"+price);

   
   int quantity = model.Quantity;

      Console.WriteLine("QUANTITY:"+quantity);

   
   int sub_cat=model.SubCategory;

      Console.WriteLine("Subcat:"+sub_cat);

   
   int brand=model.Brand;
      Console.WriteLine("brand:"+brand);


   int category = model.Category;

         Console.WriteLine("category:"+category);


   string description=model.Description;

         Console.WriteLine("description:"+description);

   
   string inbox_description=model.InboxDescription;
   
      Console.WriteLine("inbox:"+inbox_description);


   string discount_description = model.DiscountDescription;

    Console.WriteLine("discount:"+discount_description);


   string folder_name="UploadImages";

   string upload_path=Path.Combine(this._webHostEnv.WebRootPath,folder_name);

  string front_avatar="";

  string back_avatar="";

  Console.WriteLine("CHECK POINT 0");

   List<string> colors=model.Color;

        Console.WriteLine("colors:"+colors.Count);

   
   List<int> weights=model.Weight;
  
          Console.WriteLine("weight:"+weights.Count);

   
   List<string> sizes=model.Size;

          //Console.WriteLine("sizes:"+sizes.Count);

   
   List<string> mirrors=model.Mirror;

          //Console.WriteLine("mirror:"+mirrors.Count);

   
   List<string> versions=model.Version;

         // Console.WriteLine("version:"+versions.Count);


   List<IFormFile> img_files=model.ImageFiles;

      
          //  Console.WriteLine("img_file:"+img_files.Count);

   
   List<IFormFile> variant_files = model.VariantFiles;

          // Console.WriteLine("variant file:"+variant_files.Count);

   
   if(await checkProductExist(product_name))
   {
    created_res=-1;
    return created_res;
   }

   if(sub_cat!=-1)
   {
      var sub_cat_ob=await this.findCatIdBySubId(sub_cat);
      if(sub_cat_ob!=null)
      {
        category=sub_cat_ob.Category.Id;
      } 
   }
  Console.WriteLine("check point 1");

  if(!Directory.Exists(upload_path))
  {
    Directory.CreateDirectory(upload_path);
  }

 Console.WriteLine("check point 2");

 string created_date=DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss");
 
 string updated_date = DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss");
 
 List<Variant> variant=new List<Variant>();
 
 for(int i=0;i<colors.Count;i++)
 {
  string color=colors[i];
  int weight=weights[i];
  string size=sizes[i];
  string version=versions[i];
  string mirror=mirrors[i];

  var check_color_exist = await this._context.Colors.FirstOrDefaultAsync(c=>c.Colorname==color);
  var check_size_exist = await this._context.Sizes.FirstOrDefaultAsync(c=>c.Sizename==size);
  var check_version_exist = await this._context.Versions.FirstOrDefaultAsync(c=>c.Versionname==version);
  var check_mirror_exist = await this._context.Mirrors.FirstOrDefaultAsync(c=>c.Mirrorname==mirror);

  if(check_color_exist==null)
  {if(!string.IsNullOrEmpty(color))
  {
    var new_color = new Models.Color{Colorname=color};

    await this._context.Colors.AddAsync(new_color);
  }
  }
   if(check_version_exist==null)
  {
   if(!string.IsNullOrEmpty(version))
   {
    var new_version = new Ecommerce_Product.Models.Version{Versionname=version};
    await this._context.Versions.AddAsync(new_version);
   }
  }
   if(check_size_exist==null)
  { if(!string.IsNullOrEmpty(size))
  {
    var new_size = new Ecommerce_Product.Models.Size{Sizename=size};
    await this._context.Sizes.AddAsync(new_size);
  }
  }
   if(check_mirror_exist==null)
  { if(!string.IsNullOrEmpty(mirror))
  {
    var new_mirror = new Ecommerce_Product.Models.Mirror{Mirrorname=mirror};
    await this._context.Mirrors.AddAsync(new_mirror);
  }
  }

  await this.saveChanges();
 
 Console.WriteLine("out of here");
 
  var new_color_ob=await this._context.Colors.FirstOrDefaultAsync(c=>c.Colorname==color);

  var new_size_ob = await this._context.Sizes.FirstOrDefaultAsync(c=>c.Sizename==size);

  var new_version_ob = await this._context.Versions.FirstOrDefaultAsync(c=>c.Versionname==version);

  var new_mirror_ob = await this._context.Mirrors.FirstOrDefaultAsync(c=>c.Mirrorname==mirror);

  var new_varian_ob=new Variant{Colorid=new_color_ob!=null?new_color_ob.Id:null,Sizeid=new_size_ob!=null?new_size_ob.Id:null,Weight=weight,Versionid=new_version_ob!=null?new_version_ob.Id:null,Mirrorid=new_mirror_ob!=null?new_mirror_ob.Id:null};

  variant.Add(new_varian_ob); 
 }

 Console.WriteLine("check point 3");

if(img_files!=null)
{
 for(int i=0;i<img_files.Count;i++)
 { 
   var img=img_files[i];
   
   string file_name=Guid.NewGuid()+"_"+Path.GetFileName(img.FileName);
   
   string file_path=Path.Combine(upload_path,file_name);
   if(i==0)
   {
   front_avatar=file_path;
   }
   else
   {
    back_avatar=file_path;
   }
    
   using(var fileStream=new FileStream(file_path,FileMode.Create))
   {
    await img.CopyToAsync(fileStream);
   }
 }
}

List<ProductImage> list_img=new List<ProductImage>();

if(variant_files!=null)
{
for(int i=0;i<variant_files.Count;i++)
{
  var img = variant_files[i];

  string file_name=Guid.NewGuid()+"_"+Path.GetFileName(img.FileName);

  string file_path = Path.Combine(upload_path,file_name);

  var img_obj=new ProductImage{Avatar=file_path};

  list_img.Add(img_obj);

  using(var fileStream=new FileStream(file_path,FileMode.Create))
  {
    await img.CopyToAsync(fileStream);
  }
}
}

 var product= new Product{ProductName=product_name,CategoryId=category,SubCatId=sub_cat==-1?null:sub_cat,BrandId=brand,Price=price.ToString(),Quantity=quantity,Status="Còn hàng",Description=description,InboxDescription=inbox_description,DiscountDescription=discount_description,Frontavatar=front_avatar,Backavatar=back_avatar,CreatedDate=created_date,UpdatedDate=updated_date,ProductImages=list_img,Variants=variant};

  await this._context.Products.AddAsync(product);

  await this.saveChanges();

  created_res=1;
  Console.WriteLine("Created res here is:"+created_res);
  }
  catch(Exception er)
  { created_res=0;
    Console.WriteLine("Add New Product Exception:"+er.InnerException??er.Message);
  }
  return created_res;
}


public async Task<int> updateProduct(int id,AddProductModel model)
{
 int updated_res=0; 
try
{
     string temp_front_avatar="";
   string temp_back_avatar="";
   List<string> temp_list_img=new List<string>(); 
  var product_ob=await this.findProductById(id);

  
string product_name=model.ProductName;

   Console.WriteLine("Product name:"+product_name);
   
   int price=model.Price;

      Console.WriteLine("Price:"+price);

   
   int quantity = model.Quantity;

      Console.WriteLine("QUANTITY:"+quantity);

   
   int sub_cat=model.SubCategory;

      Console.WriteLine("Subcat:"+sub_cat);

   
   int brand=model.Brand;
      Console.WriteLine("brand:"+brand);


   int category =model.Category;

         Console.WriteLine("category:"+category);


   string description=model.Description;

         Console.WriteLine("description:"+description);

   
   string inbox_description=model.InboxDescription;
   
      Console.WriteLine("inbox:"+inbox_description);


   string discount_description = model.DiscountDescription;

    Console.WriteLine("discount:"+discount_description);

   string status=model.Status;

   Console.WriteLine("Status:"+status);


   string folder_name="UploadImages";

   string upload_path=Path.Combine(this._webHostEnv.WebRootPath,folder_name);

  string front_avatar="";

  string back_avatar="";

  Console.WriteLine("CHECK POINT 0");

   List<string> colors=model.Color;

        Console.WriteLine("colors:"+colors.Count);

   
   List<int> weights=model.Weight;
  
          Console.WriteLine("weight:"+weights.Count);

   
   List<string> sizes=model.Size;

          //Console.WriteLine("sizes:"+sizes.Count);

   
   List<string> mirrors=model.Mirror;

          //Console.WriteLine("mirror:"+mirrors.Count);

   
   List<string> versions=model.Version;

         // Console.WriteLine("version:"+versions.Count);


   List<IFormFile> img_files=model.ImageFiles;

      
          //  Console.WriteLine("img_file:"+img_files.Count);

   
   List<IFormFile> variant_files = model.VariantFiles;

          // Console.WriteLine("variant file:"+variant_files.Count);

   


   if(sub_cat!=-1)
   {
      var sub_cat_ob=await this.findCatIdBySubId(sub_cat);
      if(sub_cat_ob!=null)
      {
        category=sub_cat_ob.Category.Id;
      } 
   }
  Console.WriteLine("check point 1");

  if(!Directory.Exists(upload_path))
  {
    Directory.CreateDirectory(upload_path);
  }

 Console.WriteLine("check point 2");

 
 string updated_date = DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss");
 
 List<Variant> variant=new List<Variant>();
 
 for(int i=0;i<colors.Count;i++)
 {
  string color=colors[i];
  int weight=weights[i];
  string size=sizes[i];
  string version=versions[i];
  string mirror=mirrors[i];

  var check_color_exist = await this._context.Colors.FirstOrDefaultAsync(c=>c.Colorname==color);
  var check_size_exist = await this._context.Sizes.FirstOrDefaultAsync(c=>c.Sizename==size);
  var check_version_exist = await this._context.Versions.FirstOrDefaultAsync(c=>c.Versionname==version);
  var check_mirror_exist = await this._context.Mirrors.FirstOrDefaultAsync(c=>c.Mirrorname==mirror);

  if(check_color_exist==null)
  {if(!string.IsNullOrEmpty(color))
  {
    var new_color = new Models.Color{Colorname=color};

    await this._context.Colors.AddAsync(new_color);
  }
  }
   if(check_version_exist==null)
  {
   if(!string.IsNullOrEmpty(version))
   {
    var new_version = new Ecommerce_Product.Models.Version{Versionname=version};
    await this._context.Versions.AddAsync(new_version);
   }
  }
   if(check_size_exist==null)
  { if(!string.IsNullOrEmpty(size))
  {
    var new_size = new Ecommerce_Product.Models.Size{Sizename=size};
    await this._context.Sizes.AddAsync(new_size);
  }
  }
   if(check_mirror_exist==null)
  { if(!string.IsNullOrEmpty(mirror))
  {
    var new_mirror = new Ecommerce_Product.Models.Mirror{Mirrorname=mirror};
    await this._context.Mirrors.AddAsync(new_mirror);
  }
  }

  await this.saveChanges();
 
 
  var new_color_ob=await this._context.Colors.FirstOrDefaultAsync(c=>c.Colorname==color);

  var new_size_ob = await this._context.Sizes.FirstOrDefaultAsync(c=>c.Sizename==size);

  var new_version_ob = await this._context.Versions.FirstOrDefaultAsync(c=>c.Versionname==version);

  var new_mirror_ob = await this._context.Mirrors.FirstOrDefaultAsync(c=>c.Mirrorname==mirror);

  var new_varian_ob=new Variant{Colorid=new_color_ob!=null?new_color_ob.Id:null,Sizeid=new_size_ob!=null?new_size_ob.Id:null,Weight=weight,Versionid=new_version_ob!=null?new_version_ob.Id:null,Mirrorid=new_mirror_ob!=null?new_mirror_ob.Id:null};

  variant.Add(new_varian_ob); 
 }



if(img_files!=null)
{
 for(int i=0;i<img_files.Count;i++)
 { 
   var img=img_files[i];
   
   string file_name=Guid.NewGuid()+"_"+Path.GetFileName(img.FileName);
   
   string file_path=Path.Combine(upload_path,file_name);
   if(i==0)
   {
   front_avatar=file_path;
   if(!string.IsNullOrEmpty(product_ob.Frontavatar))
   {
    temp_front_avatar=product_ob.Frontavatar;
   }
   }
   else
   {
    back_avatar=file_path;
    if(!string.IsNullOrEmpty(product_ob.Backavatar))
    {
      temp_back_avatar=product_ob.Backavatar;
    }
   }
    
   using(var fileStream=new FileStream(file_path,FileMode.Create))
   {
    await img.CopyToAsync(fileStream);
   }
 }
}

List<ProductImage> list_img=new List<ProductImage>();

if(variant_files!=null)
{
for(int i=0;i<variant_files.Count;i++)
{
  var img = variant_files[i];

  string file_name=Guid.NewGuid()+"_"+Path.GetFileName(img.FileName);

  string file_path = Path.Combine(upload_path,file_name);

  var img_obj=new ProductImage{Avatar=file_path};

  list_img.Add(img_obj);

  using(var fileStream=new FileStream(file_path,FileMode.Create))
  {
    await img.CopyToAsync(fileStream);
  }
  
  foreach(var prod_img in product_ob.ProductImages )
  {
    if(!string.IsNullOrEmpty(prod_img.Avatar))
    {
      temp_list_img.Add(prod_img.Avatar);
    }
  }
}
}


 var product= new Product{ProductName=product_name,CategoryId=category,SubCatId=sub_cat==-1?null:sub_cat,BrandId=brand,Price=price.ToString(),Quantity=quantity,Status=status,Description=description,InboxDescription=inbox_description,DiscountDescription=discount_description,Frontavatar=string.IsNullOrEmpty(front_avatar)?product_ob.Frontavatar:front_avatar,Backavatar=string.IsNullOrEmpty(back_avatar)?product_ob.Backavatar:back_avatar,UpdatedDate=updated_date,ProductImages=list_img.Count==0?product_ob.ProductImages:list_img,Variants=variant};

  if(product_ob!=null)
  {
    product_ob.ProductName=product.ProductName;
    product_ob.Price=product.Price;
    product_ob.Quantity=product.Quantity;
    product_ob.CategoryId=product.CategoryId;
    product_ob.BrandId=product.BrandId;
    product_ob.SubCatId=product.SubCatId;
    product_ob.Frontavatar=product.Frontavatar;
    product_ob.Backavatar=product.Backavatar;
    product_ob.ProductImages=product.ProductImages;
    product_ob.Variants=product.Variants;
    product_ob.Status=product.Status;
    product_ob.Description=product.Description;
    product_ob.InboxDescription=product.InboxDescription;
    product_ob.DiscountDescription=product.DiscountDescription;
    this._context.Products.Update(product_ob);
    await this.saveChanges();
    updated_res=1;
    if(!string.IsNullOrEmpty(temp_front_avatar))
    {
      await this._sp_services.removeFiles(temp_front_avatar);
    }
    if(!string.IsNullOrEmpty(temp_back_avatar))
    {
      await this._sp_services.removeFiles(temp_back_avatar);
    }
    if(temp_list_img.Count>0)
    {
      foreach(var img in temp_list_img)
      {
        await this._sp_services.removeFiles(img);
      }
    }
  }
}
catch(Exception er)
{ 
  Console.WriteLine("Update Product Exception:"+er.Message);
}
return updated_res;
}

public async Task saveChanges()
{
    await this._context.SaveChangesAsync();
}




  public async Task<PageList<Variant>> pagingVariant(int id,int page_size,int page)
  {
   try
   {

    var products=await this.findProductById(id);

    IEnumerable<Variant> all_variant= products.Variants;

   List<Variant> variants=all_variant.OrderByDescending(u=>u.Id).ToList(); 

   var variant_list =PageList<Variant>.CreateItem(variants.AsQueryable(),page,page_size);
   
   return variant_list;
   }
   catch(Exception er)
   {
    Console.WriteLine("Paging Variant Exception:"+er.InnerException??er.Message);
   }
   return null;
  }
 


}