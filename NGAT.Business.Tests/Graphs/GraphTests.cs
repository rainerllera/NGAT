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
            var id = coordinate.GetHashCode();

            var newNode = new Node()
            {
                Latitude = 2,
                Longitude = 2
            };
            long originalId = 289912;

            var fetchedAttributes = new Dictionary<string, string>();
            fetchedAttributes.Add("name", "NodeName");
            fetchedAttributes.Add("age", "64");

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
    }
}
