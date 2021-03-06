﻿using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using NGAT.Business.Contracts.Filters;
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
            //var defautlInput = new DefaultOsmPbfGraphBuilderInput(Path.Combine(AppContext.BaseDirectory, "cuba-latest.osm.pbf"), NodeFilterCollection, NodeFetchersCollection, ArcFilterCollection, ArcFetchersCollection);
            var defautlInput = new DefaultOsmPbfGraphBuilderInput(Path.Combine(AppContext.BaseDirectory, "api.osm.pbf"), NodeFilterCollection, NodeFetchersCollection, ArcFilterCollection, ArcFetchersCollection);
            var builder = new DefaultOsmPbfGraphBuilder();

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
            
        }
    }
}
