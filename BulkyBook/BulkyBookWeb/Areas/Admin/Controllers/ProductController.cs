using BulkyBook.DataAccess;
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
            return View();
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
                viewModel.Product = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == id);
                return View(viewModel);
            }
        }

        [HttpPost]
        public IActionResult Upsert(ProductViewModel viewModel, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                var wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file is not null)
                {
                    var fileName = Guid.NewGuid().ToString(); 
                    var uploadPath = Path.Combine(wwwRootPath, @"images\products\");
                    var extension = Path.GetExtension(file.FileName);

                    if (viewModel.Product.ImageUrl is not null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, viewModel.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                            System.IO.File.Delete(oldImagePath);
                    }

                    using (var stream = new FileStream(Path.Combine(uploadPath, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    viewModel.Product.ImageUrl = @"\images\products\" + fileName + extension;
                }
                
                if (viewModel.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(viewModel.Product);
                    TempData["success"] = "Product created successfuly";
                }
                else
                {
                    _unitOfWork.Product.Update(viewModel.Product);
                    TempData["success"] = "Product updated successfuly";
                }

                _unitOfWork.Save();

                return RedirectToAction("Index");
            }
            return View(viewModel);
        }        

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitOfWork.Product.GetAll(includeProperties:"Category,CoverType");
            return Json(new { data = productList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var product = _unitOfWork.Product.GetFirstOrDefault(c => c.Id == id);
            var productTitle = product.Title;

            if (product == null)
                return Json(new { success = false, message = "Error while deleting" });

            var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);

            _unitOfWork.Product.Remove(product);
            _unitOfWork.Save();

            return Json(new { success = true, message = $"Product {productTitle} deleted successfuly" });
        }
        #endregion
    }
}
