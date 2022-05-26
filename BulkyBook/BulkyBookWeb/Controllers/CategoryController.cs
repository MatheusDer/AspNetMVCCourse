using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _db;

        public CategoryController(ICategoryRepository db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> categories = _db.GetAll();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]       
        public IActionResult Create(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
                ModelState.AddModelError("Name", "The DisplayOrder cannot exactly match the Name.");

            if (ModelState.IsValid)
            {
                _db.Add(category);
                _db.Save();
                TempData["success"] = "Category created successfuly";

                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound(id);

            var category = _db.GetFirstOrDefault(c => c.Id == id);
            if (category == null)
                return NotFound(category);

            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
                ModelState.AddModelError("Name", "The DisplayOrder cannot exactly match the Name.");

            if (ModelState.IsValid)
            {
                _db.Update(category);
                _db.Save();
                TempData["success"] = "Category edited successfuly";

                return RedirectToAction("Index");
            }
            return View(category);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound(id);

            var category = _db.GetFirstOrDefault(c => c.Id == id);
            if (category == null)
                return NotFound(category);

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            var category = _db.GetFirstOrDefault(c => c.Id == id);

            if (category == null)
                return NotFound(id);

            _db.Remove(category);
            _db.Save();
            TempData["success"] = "Category deleted successfuly";

            return RedirectToAction("Index");
        }
    }
}
