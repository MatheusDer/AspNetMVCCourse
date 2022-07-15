using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int orderId)
        {
            OrderVM = new OrderVM()
            {
                OrderHeader = _unitOfWork.OrderHeader
                    .GetFirstOrDefault(o => o.Id == orderId, includeProperties:"ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail
                    .GetAll(o => o.OrderHeaderId == orderId, includeProperties: "Product"),
            };

            return View(OrderVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderDetails()
        {
            var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(o => o.Id == OrderVM.OrderHeader.Id, tracked: false);

            orderHeader.Name = OrderVM.OrderHeader.Name;
            orderHeader.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeader.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeader.City = OrderVM.OrderHeader.City;
            orderHeader.State = OrderVM.OrderHeader.State;
            orderHeader.PostalCode = OrderVM.OrderHeader.PostalCode;

            if(OrderVM.OrderHeader.Carrier is not null)
                orderHeader.Carrier = OrderVM.OrderHeader.Carrier;

            if (OrderVM.OrderHeader.TrackingNumber is not null)
                orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;

            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();

            TempData["Success"] = "Order Details Updated Successfully";

            return RedirectToAction("Details", "Order", new { orderId = orderHeader.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StartProcessing()
        {
            _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.OrderStatusInProcess);
            _unitOfWork.Save();

            TempData["Success"] = "Order Status Updated Successfully";

            return RedirectToAction("Details", "Order", new { orderId = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ShipOrder()
        {
            var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(o => o.Id == OrderVM.OrderHeader.Id, tracked: false);

            orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
            orderHeader.OrderStatus = OrderVM.OrderHeader.OrderStatus;
            orderHeader.ShippingDate = DateTime.Now;
            orderHeader.OrderStatus = SD.OrderStatusShipped;

            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();

            TempData["Success"] = "Order Shipped Successfully";

            return RedirectToAction("Details", "Order", new { orderId = OrderVM.OrderHeader.Id });
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            var orderHeaders = GetOrderHeaders();

            switch (status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(o => o.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    orderHeaders = orderHeaders.Where(o => o.OrderStatus == SD.OrderStatusInProcess);
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(o => o.OrderStatus == SD.OrderStatusShipped);
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(o => o.OrderStatus == SD.OrderStatusApproved);
                    break;
                default:
                    break;
            }

            return Json(new { data = orderHeaders });
        }

        private IEnumerable<OrderHeader> GetOrderHeaders()
        {
            if (!User.IsInRole(SD.Role_Admin) && !User.IsInRole(SD.Role_Employee))
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                return _unitOfWork.OrderHeader
                    .GetAll(o => o.ApplicationUserId == claim.Value, includeProperties: "ApplicationUser");
            }
            
            return _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
        }
        #endregion
    }
}
