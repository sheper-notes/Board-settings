using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IBoardDBService {
        public Task DeleteBoard(long id, string jwt);

    }
}
