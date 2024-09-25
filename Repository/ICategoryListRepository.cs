using System;
using Ecommerce_Product.Data;
using Ecommerce_Product.Models;
namespace Ecommerce_Product.Repository;

public interface ICategoryListRepository
{


public Task<IEnumerable<Category>> getAllCategory();
public Task<IEnumerable<Category>> filterCategoryList(FilterCategory category);

public Task<PageList<Category>> pagingCategory(int page_size,int page);

public Task<int> createCategory(Category user);

public Task<bool> checkCategoryExist(string categoryname);

public Task<int> deleteCategory(int id);

public Task<int> updateCategory(Category category);

public Task<Category> findCategoryById(int id);

public Task<Category> findCategoryByName(string categoryname);

public Task<IEnumerable<SubCategory>> findSubCategoryByCat(string category);

public Task<PageList<SubCategory>> pagingSubCategory(int category,int page_size,int page);

public Task<int> createSubCategory(string subcategoryname,int brandid,int categoryid);

public Task<bool> checkSubCatExist(string sub_cat);

public Task<IEnumerable<Brand>> getAllBrandList();

public Task saveChange();
// public Task<IEnumerable<ApplicationUser>> getUserListByRole(string role);

// public Task<bool> checkUserRole(string email,string role);
// public Task<bool> checkUserExist(string email);
// public Task<bool> addUser(ApplicationUser user);
// public Task<bool> updateUser(ApplicationUser user);
// public Task<ApplicationUser> getUser(string email);
// public Task<bool> deleteUser(string email);

// public Task<bool> sendEmail(string email,string receiver,string subject);

}