using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<CoverType> coverTypes = _unitOfWork.CoverType.GetAll();
            return View(coverTypes);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CoverType coverType)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.CoverType.Add(coverType);
                _unitOfWork.Save();
                TempData["success"] = "CoverType created successfuly";

                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound(id);

            var coverType = _unitOfWork.CoverType.GetFirstOrDefault(c => c.Id == id);

            if (coverType == null)
                return NotFound(coverType);

            return View(coverType);
        }

        [HttpPost]
        public IActionResult Edit(CoverType coverType)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.CoverType.Update(coverType);
                _unitOfWork.Save();
                TempData["success"] = "CoverType edited successfuly";

                return RedirectToAction("Index");
            }
            return View(coverType);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound(id);

            var coverType = _unitOfWork.CoverType.GetFirstOrDefault(c => c.Id == id);
            if (coverType == null)
                return NotFound(coverType);

            return View(coverType);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            var coverType = _unitOfWork.CoverType.GetFirstOrDefault(c => c.Id == id);

            if (coverType == null)
                return NotFound(id);

            _unitOfWork.CoverType.Remove(coverType);
            _unitOfWork.Save();
            TempData["success"] = "CoverType deleted successfuly";

            return RedirectToAction("Index");
        }
    }
}
