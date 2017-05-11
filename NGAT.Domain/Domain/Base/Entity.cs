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
    public class Entity :Entity<ulong>
    {

    }
}
