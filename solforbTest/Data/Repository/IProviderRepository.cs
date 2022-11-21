using System;
using solforbTest.Data.Entity;

namespace solforbTest.Data.Repository
{
    public interface IProviderRepository : IBaseRepositpry<Provider>
    {
        Task<Provider?> GetByName(string name);
    }
}

