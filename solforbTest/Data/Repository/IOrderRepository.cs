using System;
using solforbTest.Data.Entity;

namespace solforbTest.Data.Repository.Implementation
{
    public interface IOrderRepository : IBaseRepositpry<Order>
    {
        Task<Order?> GetByNumberAndProvider(string Number, int providerId);
        new Task<IEnumerable<Order>> GetAll();
        new Task<Order> GetById(int? id);
    }
}

