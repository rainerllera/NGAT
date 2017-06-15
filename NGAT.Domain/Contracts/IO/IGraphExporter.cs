using System;
using System.Collections.Generic;
using System.Text;
using NGAT.Business.Contracts.Common;
using System.Threading.Tasks;
using NGAT.Business.Domain.Core;
using System.IO;

namespace NGAT.Business.Contracts.IO
{
    /// <summary>
    /// Represents an object that exports (or saves) a graph to a specific format
    /// </summary>
    /// <typeparam name="TOutput"></typeparam>
    public interface IGraphExporter : IDisposable
    {
        /// <summary>
        /// Exports only the subgraph within this range
        /// </summary>
        /// <param name="minLat">Minimum latitude</param>
        /// <param name="MinLong">Minimum longitude</param>
        /// <param name="maxLat">Maximum latitude</param>
        /// <param name="MaxLong">Maximum longitude</param>
        void ExportInRange(double minLat, double MinLong, double maxLat, double MaxLong, Graph graph);

        /// <summary>
        /// Exports only the subgraph within this range
        /// </summary>
        /// <param name="minLat"></param>
        /// <param name="MinLong"></param>
        /// <param name="maxLat"></param>
        /// <param name="MaxLong"></param>
        Task ExportInRangeAsync(double minLat, double MinLong, double maxLat, double MaxLong, Graph graph);
       
        /// <summary>
        /// Exports a graph synchronously
        /// </summary>
        /// <param name="input">The input for this export, including the graph to export</param>
        void Export(Graph graph);

        /// <summary>
        /// Exports a Graph asynchronously
        /// </summary>
        /// <param name="input">The input for this export, including the graph to export</param>
        /// <returns>A task that exports the graph</returns>
        Task ExportAsync(Graph graph);
    }
}
