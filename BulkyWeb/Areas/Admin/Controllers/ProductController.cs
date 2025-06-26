using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {

        public readonly IUnitOfWork _productRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _productRepo = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> ProductList = _productRepo.Product.GetAll(includeProperties:"Category").ToList();
            return View(ProductList);
        }
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                CategoryList = _productRepo.Category.GetAll().//Projections in EF
                Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };
            if ((id == null || id == 0))
            {
                //create
                return View(productVM);
            }
            else
            {
                //update
                productVM.Product = _productRepo.Product.Get(u => u.Id == id);
                return View(productVM);
            }

        }
        [HttpPost] //for submit button
        public IActionResult Upsert(ProductVM productVM, IFormFile? file) //for submit button
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\products");
                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        //del old img
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"images\products\" + fileName;
                }
                if (productVM.Product.Id == 0) {
                    _productRepo.Product.Add(productVM.Product);
                }
                else
                {
                    _productRepo.Product.Update(productVM.Product);
                }
               
                _productRepo.Save();
                TempData["success"] = "Product Added Succuesfully";
                return RedirectToAction("Index", "Product");
            }
            else
            {
                productVM.CategoryList = _productRepo.Category.GetAll().//Projections in EF
                 Select(u => new SelectListItem
                 {
                     Text = u.Name,
                     Value = u.Id.ToString()
                 });
                return View(productVM);
            }

        }

        #region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _productRepo.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _productRepo.Product.Get(u=>u.Id == id);
            if(productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _productRepo.Product.Remove(productToBeDeleted);
            _productRepo.Save();
            return Json(new { success = true, message = "Delete SuccessFull" });
        }
        #endregion
    }
}
