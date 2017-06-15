using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NGAT.Business.Domain.Core;

namespace NGAT.Business.Contracts.IO
{
    /// <summary>
    /// Represents an object that imports a graph
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    public interface IGraphImporter<TInput>
    {
        /// <summary>
        /// Imports a graph synchronously
        /// </summary>
        /// <param name="input">The input for this import</param>
        /// <returns>The imported Graph</returns>
        Graph Import(TInput input);

        /// <summary>
        /// Imports a Graph asynchronously
        /// </summary>
        /// <param name="input">The input for this import</param>
        /// <returns>A task that when completed returns the imported graph</returns>
        Task<Graph> ImportAsync(TInput input);
    }
}
