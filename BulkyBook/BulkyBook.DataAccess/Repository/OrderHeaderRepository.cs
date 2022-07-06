using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderHeaderRepository(ApplicationDbContext context) 
            : base(context)
        {
            _context = context;
        }

        public void Update(OrderHeader orderHeader)
        {
            _context.OrderHeaders.Update(orderHeader);
        }

        public void UpdateStatus(int id, string status, string? paymentStats = null)
        {
            var orderHeader = _context.OrderHeaders.FirstOrDefault(x => x.Id == id);

            if (orderHeader is not null)
            {
                orderHeader.OrderStatus = status;
                if (paymentStats is not null)
                    orderHeader.PaymentStatus = paymentStats;
            }
        }
    }
}
