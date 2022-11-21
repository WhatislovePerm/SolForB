using System;
using Microsoft.EntityFrameworkCore;
using solforbTest.Data.DataContext;
using solforbTest.Data.Entity;

namespace solforbTest.Data.Repository.Implementation
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDataContext context) : base(context)
        {

        }

        public async Task<Order?> GetByNumberAndProvider(string Number, int providerId)
            => await db.Order.Where(x => x.Number == Number && x.ProviderId == providerId).FirstOrDefaultAsync();

        public override async Task<IEnumerable<Order>> GetAll()
            => await db.Set<Order>().Include(x => x.Provider).Include(x => x.OrderItems).ToListAsync();

        public override async Task<Order> GetById(int? id)
            => await db.Set<Order>().Where(x => x.Id == id).Include(x => x.Provider).Include( x => x.OrderItems).FirstOrDefaultAsync();
    }
}

