using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Daric.Database.SqlServer
{
    public interface IEntityEntryItem
    {
        public EntityEntry EntityEntry { get; }
        public EntityState State { get; }
    }

}
