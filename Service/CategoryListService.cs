using Ecommerce_Product.Repository;
using Ecommerce_Product.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_Product.Service;
public class CategoryListService:ICategoryListRepository
{
  private readonly EcommerceShopContext _context;
 // private readonly Logger<CategoryListService> _logger;
  public CategoryListService(EcommerceShopContext context)
  {
    this._context=context;
  }


public async Task<IEnumerable<Category>> getAllCategory()
{    
    try
    {
     var categories=this._context.Categories.ToList();
     return categories;
    }
    catch(Exception er)
    {
        Console.WriteLine("Get All Category Exception:"+er.Message);
    
    }
    return null;
}
public async Task<IEnumerable<Category>> filterCategoryList(FilterCategory category)
{   var cat_list=this._context.Categories.ToList();
  try
  {
   string category_name=category.Category;
   string start_date=category.StartDate;
   string end_date = category.EndDate;
   if(!string.IsNullOrEmpty(category_name))
   {
    category_name=category_name.Trim();
    cat_list= cat_list.Where(c=>c.CategoryName.ToLower()==category_name.ToLower()).ToList();
   }
   if(!string.IsNullOrEmpty(start_date) && string.IsNullOrEmpty(end_date))
   {
    start_date=start_date.Trim();
    cat_list= cat_list.Where(c=>DateTime.TryParse(c.CreatedDate,out var startDate) && DateTime.TryParse(start_date,out var lowerDate) && startDate>=lowerDate).ToList();
   }
   else if(string.IsNullOrEmpty(start_date) && !string.IsNullOrEmpty(end_date))
   { 
    
    end_date=end_date.Trim();
   
    cat_list= cat_list.Where(c=>DateTime.TryParse(c.CreatedDate,out var startDate) && DateTime.TryParse(end_date,out var upperDate) && startDate<=upperDate).ToList();
   }
   else if(!string.IsNullOrEmpty(start_date) && !string.IsNullOrEmpty(end_date))
   {
    start_date=start_date.Trim();
    end_date=end_date.Trim();
    cat_list=cat_list.Where(c=>DateTime.TryParse(c.CreatedDate,out var createdDate)&& DateTime.TryParse(start_date,out var startDate) && DateTime.TryParse(end_date,out var endDate) && createdDate>=startDate && createdDate<=endDate).ToList();
   }
  }
  catch(Exception er)
  {
    Console.WriteLine("Filter Category Exception:"+er.Message);
  }
  return cat_list;
}

public async Task<PageList<Category>> pagingCategory(int page_size,int page)
{
 
   IEnumerable<Category> all_cat= await this.getAllCategory();

   List<Category> cats=all_cat.OrderByDescending(u=>u.Id).ToList(); 

   //var users=this._userManager.Users;   
   var cat_list=PageList<Category>.CreateItem(cats.AsQueryable(),page,page_size);
   
   return cat_list;
}


public async Task<bool> checkCategoryExist(string categoryname)
{  bool is_existed=false;
    try
    {
     var cat=await this._context.Categories.FirstOrDefaultAsync(c=>c.CategoryName==categoryname);
     if(cat!=null)
     {
        is_existed=true;
     }
    }
    catch(Exception er)
    {
    Console.WriteLine("Check Category Exist Exception:"+er.Message); 
    }
    return is_existed;
}

public async Task<int> createCategory(Category category)
{ int create_res=0;
    try
    {
    
    bool is_existed=await checkCategoryExist(category.CategoryName);

    if(is_existed)
    {   create_res=-1;
        return create_res;
    }
     string avatar="https://cdn-icons-png.flaticon.com/128/16955/16955062.png";
     string created_date=DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");
     
     string updated_date =DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");
     
     var new_cat=new Category{CategoryName= category.CategoryName,CreatedDate=created_date,UpdatedDate=updated_date,Avatar=avatar};
     
    await this._context.Categories.AddAsync(new_cat);

    await saveChange();

     create_res=1;
    //  else{
    //     foreach(var err in res.Errors)
    //     {
    //         Console.WriteLine(err.Description);
    //     }
    //  }
    }
    catch(Exception er)
    {   create_res=0;
        Console.WriteLine("Create Category Exception:"+er.Message);
    }
    return create_res;
}


public async Task<Category> findCategoryById(int id)
{
    try
    {
      var category=await this._context.Categories.FirstOrDefaultAsync(c=>c.Id==id);
      if(category!=null)
      {
        return category;
      }
    }
    catch(Exception er)
    {
    Console.WriteLine("Find Category By Id Exception:"+er.Message);
    }
    return null;
}


public async Task<int> deleteCategory(int id)
{
    int delete_res=0;

    try
    {
      var category=await this._context.Categories.FirstOrDefaultAsync(c=>c.Id==id);
      if(category!=null)
      {
        this._context.Categories.Remove(category);
        await this.saveChange();
        delete_res=1;
      }
    }
    catch(Exception er)
    {   delete_res=0;
        Console.WriteLine("Delete Category Exception:"+er.Message);
    }
    return delete_res;
}

public async Task<int> updateCategory(Category category)
{
    int update_res=0;
    try
    {
        int cat_id=category.Id;
        var cat=await this._context.Categories.FirstOrDefaultAsync(c=>c.Id==cat_id);
        if(cat!=null)
        {
           cat.CategoryName=category.CategoryName;
           cat.UpdatedDate=DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");
           this._context.Categories.Update(cat);
           await this.saveChange();
           update_res=1;
        }
    }
    catch(Exception er)
    {
    update_res=0;
   Console.WriteLine("Update Category Exception:"+er.Message); 
    }
    return update_res;
}

public async Task<Category> findCategoryByName(string categoryname)
{
    try
    {
      var category=await this._context.Categories.FirstOrDefaultAsync(c=>c.CategoryName==categoryname);
      if(category!=null)
      {
        return category;
      }
    }
    catch(Exception er)
    {
       Console.WriteLine("Find Category By Name Exception:"+er.Message); 
    }
    return null;
}

public async Task<IEnumerable<SubCategory>> findSubCategoryByCat(string category)
{
    try
    {
       var cat=await this._context.Categories.FirstOrDefaultAsync(c=>c.CategoryName==category);
       if(cat!=null)
       {
        return cat.SubCategories;
       }
    }
    catch(Exception er)
    {
        Console.WriteLine("Find Sub Cat Exception:"+er.Message);
    }
    return null;
}

public async Task<IEnumerable<SubCategory>> findSubCategoryById(int category)
{
    try
    {
       var cat=await this._context.Categories.FirstOrDefaultAsync(c=>c.Id==category);
       if(cat!=null)
       {
        return cat.SubCategories;
       }
    }
    catch(Exception er)
    {
        Console.WriteLine("Find Sub Cat Exception:"+er.Message);
    }
    return null;
}
public async Task<PageList<SubCategory>> pagingSubCategory(int category,int page_size,int page)
{

   var all_sub_cat= await this.findSubCategoryById(category);

   var cats=all_sub_cat.OrderByDescending(u=>u.Id).ToList(); 

   //var users=this._userManager.Users;   
   var cat_list=PageList<SubCategory>.CreateItem(cats.AsQueryable(),page,page_size);
   
   return cat_list;
}

public async Task<bool> checkSubCatExist(string sub_cat)
{ bool is_exist=false;
  var sub_cat_obj=await this._context.SubCategories.FirstOrDefaultAsync(c=>c.SubCategoryName==sub_cat);
  if(sub_cat_obj!=null)
  {
    is_exist=true;
  }
  return is_exist;
}

public async Task<int> createSubCategory(string subcategoryname,int brandid,int categoryid)
{
int create_res=0;
    try
    {
    
    bool is_existed=await checkSubCatExist(subcategoryname);

    if(is_existed)
    {   create_res=-1;
        return create_res;
    }
     string created_date=DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");
     
     string updated_date =DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss");
     
     var new_cat=new SubCategory{SubCategoryName= subcategoryname,BrandId=brandid,CategoryId=categoryid,CreatedDate=created_date,UpdatedDate=updated_date};
     
    await this._context.SubCategories.AddAsync(new_cat);

    await saveChange();

     create_res=1;
    //  else{
    //     foreach(var err in res.Errors)
    //     {
    //         Console.WriteLine(err.Description);
    //     }
    //  }
    }
    catch(Exception er)
    {   create_res=0;
        Console.WriteLine("Create Category Exception:"+er.Message);
    }
    return create_res;
}

public async Task<IEnumerable<Brand>> getAllBrandList()
{
    var brand_list=this._context.Brands.ToList();
    return brand_list;
}



public async Task saveChange()
{
 await this._context.SaveChangesAsync();
}
}