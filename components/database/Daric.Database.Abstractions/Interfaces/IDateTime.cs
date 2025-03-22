namespace Daric.Database.Abstraction
{
    /// <summary>
    /// default created date time for filters
    /// </summary>
    public interface IDateTime
    {
        /// <summary>
        /// Modification Date Time 
        /// </summary>
        DateTime ModifiedAt { get; set; }
        /// <summary>
        /// Creation Date Time 
        /// </summary>
        DateTime CreatedAt { get; set; }
    }
}
