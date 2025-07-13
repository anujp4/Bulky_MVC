using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Utility;
using BulkyWeb.DataAccess.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.Common;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _categoryRepository;
        public CategoryController(IUnitOfWork db)
        {
            _categoryRepository = db;
        }
        public IActionResult Index()
        {
            List<Category> objCategories = _categoryRepository.Category.GetAll().ToList();
            return View(objCategories);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost] //for submit button
        public IActionResult Create(Category obj) //for submit button
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "Category name and Display Order cannot be same");
            }
            if (ModelState.IsValid)
            {
                _categoryRepository.Category.Add(obj);
                _categoryRepository.Save();
                TempData["success"] = "Category Created Succuesfully";
                return RedirectToAction("Index", "Category");
            }
            else
            {
                return View();
            }

        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? CategoryFromDb = _categoryRepository.Category.Get(u => u.Id == id);
            //Category? CategoryFromDb1 = _db.Categories.FirstOrDefault(u=>u.Id ==Id);
            //Category? CategoryFromDb2 = _db.Categories.Where(u=>u.Id==Id).FirstOrDefault();
            if (CategoryFromDb == null)
            {
                return NotFound();
            }
            return View(CategoryFromDb);
        }
        [HttpPost] //for submit button
        public IActionResult Edit(Category obj) //for submit button
        {
            if (ModelState.IsValid)
            {
                _categoryRepository.Category.Update(obj);
                _categoryRepository.Save();
                TempData["success"] = " Category Updated Succuesfully";
                return RedirectToAction("Index", "Category");
            }
            else
            {
                return View();
            }

        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category? CategoryFromDb = _categoryRepository.Category.Get(u => u.Id == id);
            if (CategoryFromDb == null)
            {
                return NotFound();
            }
            return View(CategoryFromDb);
        }
        [HttpPost, ActionName("Delete")] //for submit button
        public IActionResult DeletePOST(int? id) //for submit button
        {
            Category? CategoryFromDb = _categoryRepository.Category.Get(u => u.Id == id);
            if (CategoryFromDb == null)
            {
                return NotFound();
            }
            _categoryRepository.Category.Remove(CategoryFromDb);
            _categoryRepository.Save();
            TempData["success"] = "Category Deleted Succuesfully";
            return RedirectToAction("Index", "Category");

        }
    }
}
