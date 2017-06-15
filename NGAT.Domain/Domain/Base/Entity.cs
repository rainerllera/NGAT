using NGAT.Business.Domain.Core;

namespace NGAT.Business.Domain.Base
{
    /// <summary>
    /// An Entity
    /// </summary>
    public class Entity
    {
        /// <summary>
        /// Id of the Entity
        /// </summary>
        public int Id { get; set; }
    }

    /// <summary>
    /// Default GraphDependantEntity
    /// </summary>
    public class GraphDependantEntity : Entity
    {
        public int GraphId { get; set; }

        public Graph Graph;
    }
}
