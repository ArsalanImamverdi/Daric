using Daric.Database.Abstraction;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Daric.Database.Internals.EntityHelpers;

internal interface IEntityPropertySet
{
    Task Set(EntityPropertySetParameters parameters);
}

internal class DateTimeEntityPropertySet : IEntityPropertySet
{
    public DateTimeEntityPropertySet()
    {
    }

    public Task Set(EntityPropertySetParameters parameters)
    {
        if (parameters.Entity.Entity is IDateTime entityAsIDateTime)
        {
            if (parameters.Entity.State == EntityState.Added)
            {
                entityAsIDateTime.CreatedAt = DateTime.Now;
            }
            else
            {
                parameters.Entity.Property(nameof(IDateTime.CreatedAt)).IsModified = false;
            }
            entityAsIDateTime.ModifiedAt = DateTime.Now;
        }
        return Task.CompletedTask;
    }
}


internal class EntityPropertySetParameters(EntityEntry entity)
{
    public EntityEntry Entity { get; } = entity;
}
