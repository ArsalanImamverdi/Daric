using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Daric.Database.SqlServer.Internals
{
    internal class EntityEntryItem(EntityEntry entityEntry, EntityState state) : IEntityEntryItem
    {
        public EntityEntry EntityEntry { get; } = entityEntry;
        public EntityState State { get; } = state;
    }

    internal static class EntityEntryExtensions
    {
        public static List<IEntityEntryItem> ToEntityEntryItems(this IEnumerable<EntityEntry> entities)
        {
            return entities.Select(entity => new EntityEntryItem(entity, entity.State) as IEntityEntryItem).ToList();
        }

    }

}
