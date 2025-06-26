using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    [BindProperties]
    public class CreateModel : PageModel
    {
       
        private readonly ApplicationDbContext _db;
        public Category Category { get; set; }
        public CreateModel(ApplicationDbContext db) //Dependency Injection
        {
            _db = db;
        }
        public void OnGet()
        {

        }
        public IActionResult OnPost() {
            _db.Add(Category);
            _db.SaveChanges();
            TempData["success"] = "Category Created Succuesfully";
            return RedirectToPage("Index");
        
        }
    }
}
