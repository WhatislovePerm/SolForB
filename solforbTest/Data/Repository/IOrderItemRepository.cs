using System;
using solforbTest.Data.Entity;

namespace solforbTest.Data.Repository
{
    public interface IOrderItemRepository : IBaseRepositpry<OrderItem>
    {
        Task<IEnumerable<OrderItem>> GetByOrderId(int orderId);
    }
}

;