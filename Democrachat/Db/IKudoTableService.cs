using System.Collections.Generic;
using Democrachat.Db.Models;

namespace Democrachat.Db
{
    public interface IKudoTableService
    {
        IEnumerable<KudoListing> GetPossibleKudoItems();
    }
}