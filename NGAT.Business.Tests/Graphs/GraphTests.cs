using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using NGAT.Business.Domain.Core;
using GeoCoordinatePortable;

namespace NGAT.Business.Tests.Graphs
{
 
    public class GraphTests
    {
        [Fact]
        public void Graph_AddNode_Tests()
        {
            var graph = new Graph();

            var coordinate = new GeoCoordinate(2, 2);
            var id = 1;

            var newNode = new Node()
            {
                Latitude = 2,
                Longitude = 2
            };
            long originalId = 289912;

            var fetchedAttributes = new Dictionary<string, string>
            {
                { "name", "NodeName" },
                { "age", "64" }
            };
            graph.AddNode(newNode, originalId, fetchedAttributes);

            Assert.True(graph.VertexToNodesIndex.ContainsKey(originalId));
            Assert.True(graph.VertexToNodesIndex[originalId] == id);
            Assert.True(newNode.Id == id);
            Assert.True(graph.NodesIndex.ContainsKey(id) && graph.NodesIndex[id] == newNode);

            Assert.True(newNode.NodeData != null);

            var attrsDeserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(newNode.NodeData);

            Assert.True(attrsDeserialized["name"] == "NodeName");
            Assert.True(attrsDeserialized["age"] == "64");
        }

        [Fact]
        public void Graph_AddArc_Tests()
        {
            var graph = new Graph();

            //Adding 10 nodes
            for (int i = 1; i < 11; i++)
            {
                var coordinate = new GeoCoordinate(i, i);
                var id = coordinate.GetHashCode();

                var newNode = new Node()
                {
                    Latitude = i,
                    Longitude = i
                };
                long originalId = (long)i;

                var fetchedAttributes = new Dictionary<string, string>
                {
                    { "name", "NodeName" + i.ToString() },
                    { "age", i.ToString() }
                };
                graph.AddNode(newNode, originalId, fetchedAttributes);
            }

            Assert.True(graph.Nodes.Count == 10);
            Assert.True(graph.VertexToNodesIndex.Count == 10);
            Dictionary<string, string> fetchedattrs = new Dictionary<string, string>();
            graph.AddArc(1, 3, fetchedattrs);
            graph.AddArc(1, 4, fetchedattrs);
            graph.AddArc(4, 3, fetchedattrs);
            graph.AddArc(4, 1, fetchedattrs);
            graph.AddArc(3, 6, fetchedattrs);

            //Verifying arcs were added correctly
            Assert.True(graph.ArcsIndex[1].FromNodeId == graph.VertexToNodesIndex[1] && graph.ArcsIndex[1].ToNodeId == graph.VertexToNodesIndex[3]);
            Assert.True(graph.ArcsIndex[2].FromNodeId == graph.VertexToNodesIndex[1] && graph.ArcsIndex[2].ToNodeId == graph.VertexToNodesIndex[4]);
            Assert.True(graph.ArcsIndex[3].FromNodeId == graph.VertexToNodesIndex[4] && graph.ArcsIndex[3].ToNodeId == graph.VertexToNodesIndex[3]);
            Assert.True(graph.ArcsIndex[4].FromNodeId == graph.VertexToNodesIndex[4] && graph.ArcsIndex[4].ToNodeId == graph.VertexToNodesIndex[1]);
            Assert.True(graph.ArcsIndex[5].FromNodeId == graph.VertexToNodesIndex[3] && graph.ArcsIndex[5].ToNodeId == graph.VertexToNodesIndex[6]);

            //Verifying relations were added correctly
            Assert.True(graph.NodesIndex[1].OutgoingArcs.Count == 2);
            Assert.True(graph.NodesIndex[1].OutgoingArcs[0].ToNodeId == 3);
            Assert.True(graph.NodesIndex[1].OutgoingArcs[1].ToNodeId == 4);
            Assert.True(graph.NodesIndex[1].IncomingArcs.Count == 1);
            Assert.True(graph.NodesIndex[1].IncomingArcs[0].FromNodeId == 4);

            //testing distances
            graph.AddArc(1, 6, 5000, fetchedattrs);

            Assert.True(graph.ArcsIndex[6].Distance == 5000);
            Assert.True(graph.ArcsIndex[2].Distance == graph.ArcsIndex[4].Distance);

        }
    }
}
