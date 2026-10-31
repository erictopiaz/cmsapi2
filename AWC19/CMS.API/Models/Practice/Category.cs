using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.API.Models.Practice
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }

    public interface ICategoryRepository
    {
        List<Category> GetCategories();
    }


    public class CategoryRepository : ICategoryRepository
    {
        public List<Category> GetCategories()
        {
            List<Category> categories = new List<Category>();

            Category category = new Category() { CategoryId = 1, CategoryName = "Category1" };
            categories.Add(category);

            category = new Category() { CategoryId = 2, CategoryName = "Category2" };
            categories.Add(category);

            return categories;
        }
    }

}
