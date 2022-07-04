using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var products = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");

            return View(products);
        }

        public IActionResult Details(int productId)
        {
            ShoppingCart cart = new()
            {
                Product = _unitOfWork.Product.GetFirstOrDefault(p => p.Id == productId, includeProperties: "Category,CoverType"),
                ProductId = productId,
                Count = 1
            };

            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart cart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            cart.ApplicationUserId = claim.Value;

            ShoppingCart dbCart = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.ApplicationUserId == claim.Value && c.ProductId == cart.ProductId);

            if (dbCart is null)
                _unitOfWork.ShoppingCart.Add(cart);
            else
                _unitOfWork.ShoppingCart.IncrementCount(dbCart, cart.Count);
            
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}