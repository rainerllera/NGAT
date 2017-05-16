using NGAT.Business.Domain.Core;
using System.Threading.Tasks;

namespace NGAT.Business.Contracts.IO
{
    /// <summary>
    /// Represents a graph builder
    /// </summary>
    /// <typeparam name="TInput">Type to wrap the arguments needed by this graph builder</typeparam>
    public interface IGraphBuilder<TInput>
    {
        /// <summary>
        /// Builds a graph synchronously from <paramref name="input"/> source
        /// </summary>
        /// <param name="input">Arguments needed to build the graph</param>
        /// <returns>A Graph</returns>
        Graph Build(TInput input);

        /// <summary>
        /// Builds a graph asynchronously from <paramref name="input"/> source
        /// </summary>
        /// <param name="input">Arguments needed to build the graph</param>
        /// <returns>A Task for building the Graph</returns>
        Task<Graph> BuildAsync(TInput input);
    }
}
