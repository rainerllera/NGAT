using System;
using System.Collections.Generic;
using System.Text;
using NGAT.Business.Contracts.Common;
using System.Threading.Tasks;

namespace NGAT.Business.Contracts.IO
{
    /// <summary>
    /// Represents an object that exports (or saves) a graph to a specific format
    /// </summary>
    /// <typeparam name="TOutput"></typeparam>
    public interface IGraphExporter<TInput> where TInput:IGraphContainer
    {
        /// <summary>
        /// Exports a graph synchronously
        /// </summary>
        /// <param name="input">The input for this export, including the graph to export</param>
        void Export(TInput input);

        /// <summary>
        /// Exports a Graph asynchronously
        /// </summary>
        /// <param name="input">The input for this export, including the graph to export</param>
        /// <returns>A task that exports the graph</returns>
        Task ExportAsync(TInput input);
    }
}
