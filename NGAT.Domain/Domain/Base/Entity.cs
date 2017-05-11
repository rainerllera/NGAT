using NGAT.Business.Domain.Core;

namespace NGAT.Business.Domain.Base
{
    /// <summary>
    /// A Generic Entity, with id type <see cref="TKey"/>
    /// </summary>
    /// <typeparam name="TKey">The type of the id</typeparam>
    public class Entity<TKey> where TKey:struct
    {
        /// <summary>
        /// Id of the Entity
        /// </summary>
        public TKey Id { get; set; }
    }

    /// <summary>
    /// Default Entity, with ulong as type for the Id
    /// </summary>
    public class Entity : Entity<ulong>
    {

    }

    /// <summary>
    /// An Entity that depends on an existing graph, i.e: a node
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class GraphDependantEntity<TKey> : Entity<TKey> where TKey : struct
    {
        /// <summary>
        /// The id of the graph this entity belongs to
        /// </summary>
        public int GraphId { get; set; }

        /// <summary>
        /// The graph this entity belongs to
        /// </summary>
        public Graph Graph { get; set; }
    }

    /// <summary>
    /// Default GraphDependantEntity
    /// </summary>
    public class GraphDependantEntity : GraphDependantEntity<ulong>
    {

    }
}
