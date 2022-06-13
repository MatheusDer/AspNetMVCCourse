﻿using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> categories = _unitOfWork.Category.GetAll();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            ProductViewModel viewModel = new()
            {
                Product = new(),
                CategoryList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }),
                CoverTypeList = _unitOfWork.CoverType.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }),
            };

            if (id == null || id == 0)
            {
                //ViewBag.CategoryList = CategoryList;
                //ViewData["CoverTypeList"] = CoverTypeList;
                return View(viewModel);
            }
            else
            {

            }

            

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Upsert(ProductViewModel viewModel, IFormFile file)
        {
            const string PRODUCTPATH = @"images\products\";

            if (ModelState.IsValid)
            {
                var wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file is not null)
                {
                    var fileName = Guid.NewGuid().ToString(); 
                    var uploadPath = Path.Combine(wwwRootPath, PRODUCTPATH);
                    var extension = Path.GetExtension(file.FileName);

                    using (var stream = new FileStream(Path.Combine(uploadPath, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    viewModel.Product.ImageUrl = PRODUCTPATH + fileName + extension;
                }

                _unitOfWork.Product.Add(viewModel.Product);
                _unitOfWork.Save();
                TempData["success"] = "Product created successfuly";

                return RedirectToAction("Index");
            }
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound(id);

            var category = _unitOfWork.Category.GetFirstOrDefault(c => c.Id == id);
            if (category == null)
                return NotFound(category);

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            var category = _unitOfWork.Category.GetFirstOrDefault(c => c.Id == id);

            if (category == null)
                return NotFound(id);

            _unitOfWork.Category.Remove(category);
            _unitOfWork.Save();
            TempData["success"] = "Category deleted successfuly";

            return RedirectToAction("Index");
        }
    }
}
