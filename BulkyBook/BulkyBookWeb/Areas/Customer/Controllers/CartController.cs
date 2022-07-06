using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private ShoppingCartVM ShoppingCart;

        public int OrderTotal { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //[AllowAnonymous]
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCart = new ShoppingCartVM()
            {
                Carts = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == claim.Value, includeProperties: "Product"),
                OrderHeader = new()
            };

            foreach (var cart in ShoppingCart.Carts)
            {
                cart.Price = GetPrice(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);

                ShoppingCart.OrderHeader.OrderTotal += cart.Price * cart.Count;
            }

            return View(ShoppingCart);
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCart = new ShoppingCartVM()
            {
                Carts = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == claim.Value, includeProperties: "Product"),
                OrderHeader = new()
            };

            ShoppingCart.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);

            ShoppingCart.OrderHeader.Name = ShoppingCart.OrderHeader.ApplicationUser.Name;
            ShoppingCart.OrderHeader.PhoneNumber = ShoppingCart.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCart.OrderHeader.StreetAddress = ShoppingCart.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCart.OrderHeader.City = ShoppingCart.OrderHeader.ApplicationUser.City;
            ShoppingCart.OrderHeader.State = ShoppingCart.OrderHeader.ApplicationUser.State;
            ShoppingCart.OrderHeader.PostalCode = ShoppingCart.OrderHeader.ApplicationUser.PostalCode;

            foreach (var cart in ShoppingCart.Carts)
            {
                cart.Price = GetPrice(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);

                ShoppingCart.OrderHeader.OrderTotal += cart.Price * cart.Count;
            }

            return View(ShoppingCart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Summary(ShoppingCartVM shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            shoppingCart.Carts = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == claim.Value, includeProperties: "Product");

            shoppingCart.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            shoppingCart.OrderHeader.OrderStatus = SD.OrderStatusPending;
            shoppingCart.OrderHeader.OrderDate = DateTime.Now;
            shoppingCart.OrderHeader.ApplicationUserId = claim.Value;

            foreach (var cart in shoppingCart.Carts)
            {
                cart.Price = GetPrice(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);

                shoppingCart.OrderHeader.OrderTotal += cart.Price * cart.Count;
            }

            _unitOfWork.OrderHeader.Add(shoppingCart.OrderHeader);
            _unitOfWork.Save();

            foreach (var cart in shoppingCart.Carts)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = shoppingCart.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }

            _unitOfWork.ShoppingCart.Remove(shoppingCart.Carts);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Plus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.Id == cartId);
            _unitOfWork.ShoppingCart.IncrementCount(cart, 1);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.Id == cartId);
            if (cart.Count == 1)
                _unitOfWork.ShoppingCart.Remove(cart);
            else
                _unitOfWork.ShoppingCart.DecrementCount(cart, 1);

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.Id == cartId);
            _unitOfWork.ShoppingCart.Remove(cart);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        private static double GetPrice(double quantity, double price, double price50, double price100)
        {
            if (quantity <= 50)
                return price;

            if (quantity <= 100)
                return price50;

            return price100;
        }
    }
}
