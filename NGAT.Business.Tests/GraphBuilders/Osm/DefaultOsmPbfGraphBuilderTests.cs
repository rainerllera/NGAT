using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using NGAT.Business.Contracts.IO.Filters;
using NGAT.Business.Implementation.IO.Osm.Inputs;
using NGAT.Business.Implementation.IO.Osm;
using System.IO;
using System.Linq;

namespace NGAT.Business.Tests.GraphBuilders.Osm
{
    public class DefaultOsmPbfGraphBuilderTests
    {
        
        public DefaultOsmPbfGraphBuilderTests()
        {
            var mockNodeFilterCollection = new Mock<IAttributeFilterCollection>();
            mockNodeFilterCollection.Setup(i => i.ApplyAllFilters(It.IsAny<IDictionary<string, string>>())).Returns(true);
            NodeFilterCollection = mockNodeFilterCollection.Object;

            var mockNodeFetchersCollection = new Mock<IAttributesFetcherCollection>();
            mockNodeFetchersCollection.Setup(i => i.FetchWhiteListed(It.IsAny<IDictionary<string, string>>()))
                .Returns<IDictionary<string, string>>(i =>
                {
                    var result = new Dictionary<string, string>();
                    if (i.ContainsKey("name"))
                        result.Add("name", i["name"]);
                    return result;
                });
            NodeFetchersCollection = mockNodeFetchersCollection.Object;

            var mockArcFilterCollection = new Mock<IAttributeFilterCollection>();
            mockArcFilterCollection.Setup(i => i.ApplyAllFilters(It.IsAny<IDictionary<string, string>>()))
                .Returns<IDictionary<string,string>>((i)=> {
                    
                    return i.ContainsKey("highway");
                });
            ArcFilterCollection = mockArcFilterCollection.Object;

            var mockArcFetchersColletciont = new Mock<IAttributesFetcherCollection>();
            mockArcFetchersColletciont.Setup(i => i.FetchWhiteListed(It.IsAny<IDictionary<string, string>>()))
                .Returns<IDictionary<string, string>>(i =>
                {
                    var result = new Dictionary<string, string>();
                    foreach (var KV in i)
                    {
                        if (KV.Key.ToLowerInvariant().StartsWith("highway"))
                            result.Add(KV.Key, KV.Value);
                        else if (KV.Key.ToLowerInvariant().Contains("way"))
                            result.Add(KV.Key, KV.Value);
                    }

                    return result;
                });
            ArcFetchersCollection = mockArcFetchersColletciont.Object;

        }

        IAttributeFilterCollection NodeFilterCollection { get; set; }

        IAttributeFilterCollection ArcFilterCollection { get; set; }

        IAttributesFetcherCollection NodeFetchersCollection { get; set; }

        IAttributesFetcherCollection ArcFetchersCollection { get; set; }

        [Fact]
        public void DefaultOsmPbfGraphBuilder_Build_Tests()
        {
            var defautlInput = new OsmPbfGraphBuilderInput(Path.Combine(AppContext.BaseDirectory, "cuba-latest.osm.pbf"), NodeFilterCollection, NodeFetchersCollection, ArcFilterCollection, ArcFetchersCollection);
            //var defautlInput = new DefaultOsmPbfGraphBuilderInput(Path.Combine(AppContext.BaseDirectory, "api.osm.pbf"), NodeFilterCollection, NodeFetchersCollection, ArcFilterCollection, ArcFetchersCollection);
            var builder = new OsmPbfGraphBuilder();

            var graph = builder.Build(defautlInput);
            
            Assert.NotNull(graph);

            //Checking that arcs are all well-formed
            foreach (var arc in graph.Arcs)
            {
                var fromId = arc.FromNodeId;
                var toId = arc.ToNodeId;

                Assert.True(graph.NodesIndex.ContainsKey(fromId) && graph.NodesIndex.ContainsKey(toId));
                Assert.True(graph.NodesIndex[fromId].OutgoingArcs.Any(a => a.FromNodeId == fromId && a.ToNodeId == toId));
                Assert.True(graph.NodesIndex[toId].IncomingArcs.Any(a => a.FromNodeId == fromId && a.ToNodeId == toId));
                Assert.True(graph.Arcs[arc.Id - 1].Equals(arc));
            }

            //Checking all nodes belong to at least an arc
            Assert.True(graph.Nodes.All(n => n.IncomingArcs.Count > 0 || n.OutgoingArcs.Count > 0));

            

            var nodes = graph.Nodes.Where(a => a.Longitude <= -82.35806465148926 && a.Latitude <= 23.145805714137563 && a.Longitude >= -82.37866401672363 && a.Latitude >= 23.122363841245967);
            Assert.True(nodes.Count() != 0);
            using (var file = File.OpenWrite("featurescuba.geojson"))
            {
                using (var tw = new StreamWriter(file))
                {
                    tw.Write("{\"type\":\"FeatureCollection\",\"features\":");
                    tw.Write("[");
                    bool flag = true;
                    foreach (var node in nodes)
                    {
                        if (!flag)
                            tw.Write(",");
                        var arcs = node.OutgoingArcs;//.Arcs.Where(a => a.FromNodeId == node.Id);//.Where(a => a.FromNode.Latitude <= -82.35806465148926 && a.FromNode.Longitude <= 23.145805714137563 && a.ToNode.Latitude >= -82.37866401672363 && a.ToNode.Longitude >= 23.122363841245967);
                        foreach (var arc in arcs)
                        {
                            tw.Write("{ \"type\":\"Feature\",\"properties\":{ },\"geometry\":{ \"type\":\"LineString\",\"coordinates\":[[" + arc.FromNode.Longitude + "," + arc.FromNode.Latitude + "],[" + arc.ToNode.Longitude + "," + arc.ToNode.Latitude + "]]}},");
                        }
                        tw.Write("{ \"type\":\"Feature\",\"properties\":{ },\"geometry\":{ \"type\":\"Point\",\"coordinates\":[" + node.Longitude + "," + node.Latitude + "]}}");
                        flag = false;
                    }

                    tw.Write("]");

                    tw.Write("}");


                    
                }
            }
        }
    }
}
