using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BulkyBookWeb.Controllers
{
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var coverTypes = _unitOfWork.CoverType.GetAll();
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

            var CoverType = _unitOfWork.CoverType.GetFirstOrDefault(c => c.Id == id);

            if (CoverType == null)
                return NotFound(CoverType);

            return View(CoverType);
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

            return View();
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound(id);

            var CoverType = _unitOfWork.CoverType.GetFirstOrDefault(c => c.Id == id);

            if (CoverType == null)
                return NotFound(CoverType);

            return View(CoverType);
        }

        [HttpPost]
        public IActionResult Delete(CoverType coverType)
        {
            _unitOfWork.CoverType.Remove(coverType);
            _unitOfWork.Save();
            TempData["success"] = "CoverType deleted successfuly";
            return RedirectToAction("Index");
        }
    }
}
