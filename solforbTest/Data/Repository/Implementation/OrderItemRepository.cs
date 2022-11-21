using System;
using Microsoft.EntityFrameworkCore;
using solforbTest.Data.DataContext;
using solforbTest.Data.Entity;

namespace solforbTest.Data.Repository.Implementation
{
    public class OrderItemRepository : BaseRepository<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(ApplicationDataContext context) : base(context)
        {
        
        }

        public async Task<IEnumerable<OrderItem>> GetByOrderId(int orderId)
            => await db.OrderItem.Where(x => x.OrderId == orderId).ToListAsync();
    }
}

