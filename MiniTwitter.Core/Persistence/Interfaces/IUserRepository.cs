using MiniTwitter.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTwitter.Core.Persistence.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
    }
}
