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
            graph.AddNode(newNode, fetchedAttributes);

            //Assert.True(graph.VertexToNodesIndex.ContainsKey(originalId));
            //Assert.True(graph.VertexToNodesIndex[originalId] == id);
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
                graph.AddNode(newNode, fetchedAttributes);
            }

            Assert.True(graph.Nodes.Count == 10);
            //Assert.True(graph.VertexToNodesIndex.Count == 10);
            Dictionary<string, string> fetchedattrs = new Dictionary<string, string>();
            var arcData = new LinkData()
            {
                RawData = Newtonsoft.Json.JsonConvert.SerializeObject(fetchedattrs)
            };
            graph.AddLinkData(arcData);

            graph.AddLink(1, 3, arcData, true);
            graph.AddLink(1, 4, arcData, true);
            graph.AddLink(4, 3, arcData, true);
            graph.AddLink(4, 1, arcData, true);
            graph.AddLink(3, 6, arcData, true);

            //Verifying relations were added correctly
            Assert.True(graph.NodesIndex[1].OutgoingArcs.Count == 2);
            Assert.True(graph.NodesIndex[1].OutgoingArcs[0].ToNodeId == 3);
            Assert.True(graph.NodesIndex[1].OutgoingArcs[1].ToNodeId == 4);
            Assert.True(graph.NodesIndex[1].IncomingArcs.Count == 1);
            Assert.True(graph.NodesIndex[1].IncomingArcs[0].FromNodeId == 4);

            //testing distances
            graph.AddLink(1, 6, 5000, arcData, true);

            Assert.True(graph.Arcs[6 - 1].Distance == 5000);
            Assert.True(graph.Arcs[2 - 1].Distance == graph.Arcs[4 - 1].Distance);

        }
    }
}
