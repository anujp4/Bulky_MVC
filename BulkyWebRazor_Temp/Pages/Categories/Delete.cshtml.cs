using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    [BindProperties]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public Category? Category { get; set; }
        public DeleteModel(ApplicationDbContext db) //Dependency Injection
        {
            _db = db;
        }
        public void OnGet(int? Id)
        {
            if(Id!=null|| Id != 0)
            {
                Category = _db.Categories.FirstOrDefault(u => u.Id == Id);
            }
        }
        public IActionResult OnPost()
        {
            Category? CategoryFromDb = _db.Categories.Find(Category.Id);
            if (CategoryFromDb == null)
            {
                return NotFound();
            }
            _db.Categories.Remove(CategoryFromDb);
            _db.SaveChanges();
            TempData["success"] = "Category Deleted Succuesfully";
            return RedirectToPage("Index");
        }
    }
}
