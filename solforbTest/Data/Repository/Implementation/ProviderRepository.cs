using System;
using Microsoft.EntityFrameworkCore;
using solforbTest.Data.DataContext;
using solforbTest.Data.Entity;

namespace solforbTest.Data.Repository.Implementation
{
    public class ProviderRepository : BaseRepository<Provider>, IProviderRepository
    {
        public ProviderRepository(ApplicationDataContext context) : base(context) 
        {
            
        }

        public async Task<Provider?> GetByName(string name)
            => await db.Provider.Where(x => x.Name == name).FirstOrDefaultAsync();
    }
}

