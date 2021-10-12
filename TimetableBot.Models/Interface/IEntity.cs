
namespace TimetableBot.Models.Interface
{

    public interface IEntity<TId>
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public TId Id { get; set; }
    }
}
