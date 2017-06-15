using System;
using Xunit;
using NGAT.Services.IO.Osm;
using NGAT.Services.IO.Fetchers;
using NGAT.Services.IO.Osm.Filters;
using System.IO;
using System.Linq;
using NGAT.Services.IO.Exporters.GeoJSON;

namespace NGAT.Business.Tests.GraphBuilders.Osm
{
    public class DefaultOsmPbfGraphBuilderTests
    {
     
        [Fact]
        public void DefaultOsmPbfGraphBuilder_Build_Tests()
        {
            var builder = new OsmPbfGraphBuilder(new Uri(Path.Combine(AppContext.BaseDirectory, "cuba-latest.osm.pbf"))
                , new OsmRoadLinksFilterCollection()
                , new AttributesFetcherCollection("name")
                , new AttributesFetcherCollection("name","highway","junction","maxspeed"));

            var graph = builder.Build();
            
            Assert.NotNull(graph);

            //Checking that arcs are all well-formed
            foreach (var arc in graph.Arcs)
            {
                var fromId = arc.FromNodeId;
                var toId = arc.ToNodeId;

                Assert.True(graph.NodesIndex.ContainsKey(fromId) && graph.NodesIndex.ContainsKey(toId));
                Assert.True(graph.NodesIndex[fromId].OutgoingArcs.Any(a => a.FromNodeId == fromId && a.ToNodeId == toId));
                Assert.True(graph.NodesIndex[toId].IncomingArcs.Any(a => a.FromNodeId == fromId && a.ToNodeId == toId));
               // Assert.True(graph.Arcs[arc.Id - 1].Equals(arc));
            }

            foreach (var edge in graph.Edges)
            {
                var fromId = edge.FromNodeId;
                var toId = edge.ToNodeId;

                Assert.True(graph.NodesIndex.ContainsKey(fromId) && graph.NodesIndex.ContainsKey(toId));
                Assert.True(graph.NodesIndex[fromId].Edges.Any(a => a.FromNodeId == fromId && a.ToNodeId == toId));
                Assert.True(graph.NodesIndex[toId].Edges.Any(a => a.FromNodeId == fromId && a.ToNodeId == toId));
               // Assert.True(graph.Edges[edge.Id - 1].Equals(edge));
            }
            //Checking all nodes belong to at least an arc
            Assert.True(graph.Nodes.All(n => n.IncomingArcs.Count > 0 || n.OutgoingArcs.Count > 0 || n.Edges.Count > 0));

            //Checking for loops
            Assert.False(graph.Edges.Any(e => e.FromNodeId == e.ToNodeId));
            Assert.False(graph.Arcs.Any(a => a.FromNodeId == a.ToNodeId));

            Assert.True(graph.Edges.Where(e => e.LinkData.Attributes.ContainsKey("junction") && e.LinkData.Attributes["junction"] == "roundabout").Count() == 0);
            Assert.True(graph.Arcs.Where(e => e.LinkData.Attributes.ContainsKey("junction") && e.LinkData.Attributes["junction"] == "roundabout").Count() != 0);

            
            using (var file = File.OpenWrite("Cuba-Network-Full-Optimized.geojson"))
            {
                var geoJsonExporter = new GeoJSONGraphExporter(file, false);
                geoJsonExporter.ExportInRange(23.1277, -82.3961, 23.145805714137563, -82.35806465148926, graph);
            }
        }
    }
}
