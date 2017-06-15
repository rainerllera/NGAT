using System.Threading.Tasks;
using NGAT.Business.Contracts.IO;
using NGAT.Business.Domain.Core;
using System.IO;

namespace NGAT.Services.IO.Exporters
{
    public abstract class StreamGraphExporter : IGraphExporter
    {
        public StreamGraphExporter(Stream underlyingStrem)
        {
            Stream = underlyingStrem;
        }

        public Stream Stream { get; set; }

        public abstract string FormatID { get; }

        public void Dispose()
        {
            Stream.Dispose();
        }

        public abstract void Export(Graph graph);

        public abstract Task ExportAsync(Graph graph);

        public abstract void ExportInRange(double minLat, double MinLong, double maxLat, double MaxLong, Graph graph);

        public abstract Task ExportInRangeAsync(double minLat, double MinLong, double maxLat, double MaxLong, Graph graph);
    }
}
