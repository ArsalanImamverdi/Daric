using Daric.Domain.Shared;

namespace Daric.Database.SqlServer
{
    /// <summary>
    /// when the table row has soft deleted structure
    /// </summary>
    public interface IRemovableEntity : IEntity
    {
        /// <summary>
        /// if is soft deleted in database row or not
        /// </summary>
        bool IsDeleted { get; set; }
    }
}
