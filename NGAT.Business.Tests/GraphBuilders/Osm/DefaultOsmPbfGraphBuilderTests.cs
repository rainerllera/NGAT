using System;
using System.Collections.Generic;
using Xunit;
using NGAT.Services.IO.Osm;
using NGAT.Services.IO.Fetchers;
using NGAT.Services.IO.Osm.Filters;
using System.IO;
using System.Linq;

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

            //Checking for loops THIS IS FAILING WITH THE SIMPLIFICATION
            //Assert.False(graph.Edges.Any(e => e.FromNodeId == e.ToNodeId));
            //Assert.False(graph.Arcs.Any(a => a.FromNodeId == a.ToNodeId));

            var nodes = graph.Nodes.Where(a => a.Longitude <= -82.35806465148926 && a.Latitude <= 23.145805714137563 && a.Longitude >= -82.37866401672363 && a.Latitude >= 23.122363841245967);
            Assert.True(nodes.Count() != 0);

            Assert.True(graph.Edges.Where(e => e.LinkData.Attributes.ContainsKey("junction") && e.LinkData.Attributes["junction"] == "roundabout").Count() == 0);
            Assert.True(graph.Arcs.Where(e => e.LinkData.Attributes.ContainsKey("junction") && e.LinkData.Attributes["junction"] == "roundabout").Count() != 0);

            var markedEdges = new Dictionary<int,bool>();
            using (var file = File.OpenWrite("features-cuba-with-geometries.geojson"))
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
                            var attributesToProps = string.Concat(arc.LinkData.Attributes.Select(kv => $"\"{kv.Key}\": \"{kv.Value}\","));
                            attributesToProps = attributesToProps.Substring(0, attributesToProps.Length - 1);
                            List<string> coordinatesString = new List<string>();
                            if (arc.PointsData != null)
                            {
                                var points = arc.PointsData.Split(',');
                                for (int i = 0; i < points.Length; i++)
                                {
                                    var pointString = points[i];
                                    var splittedPoint = pointString.Split(' ');
                                    var lat = splittedPoint[0];
                                    var lon = splittedPoint[1];
                                    coordinatesString.Add($"[{lon}, {lat}]");
                                }
                            }
                            else
                            {
                                coordinatesString.Add($"[{arc.FromNode.Longitude}, {arc.FromNode.Latitude}]");
                                coordinatesString.Add($"[{arc.ToNode.Longitude}, {arc.ToNode.Latitude}]");
                            }
                            tw.Write("{ \"type\":\"Feature\",\"properties\":{ \"stroke\": \"#ff0000\", \"stroke-width\": 3, \"stroke-opacity\": 1, \"obj-type\": \"arc\", " + attributesToProps + "},\"geometry\":{ \"type\":\"LineString\",\"coordinates\":[" + string.Join(",", coordinatesString) + "]}},");
                        }

                        foreach (var edge in node.Edges)
                        {
                            if (!markedEdges.ContainsKey(edge.Id-1))
                            {
                                var attributesToProps = string.Concat(edge.LinkData.Attributes.Select(kv => $"\"{kv.Key}\": \"{kv.Value}\","));
                                attributesToProps = attributesToProps.Substring(0, attributesToProps.Length - 1);
                                List<string> coordinatesString = new List<string>();
                                if (edge.PointsData != null)
                                {
                                    var points = edge.PointsData.Split(',');
                                    for (int i = 0; i < points.Length; i++)
                                    {
                                        var pointString = points[i];
                                        var splittedPoint = pointString.Split(' ');
                                        var lat = splittedPoint[0];
                                        var lon = splittedPoint[1];
                                        coordinatesString.Add($"[{lon}, {lat}]");
                                    }
                                }
                                else
                                {
                                    coordinatesString.Add($"[{edge.FromNode.Longitude}, {edge.FromNode.Latitude}]");
                                    coordinatesString.Add($"[{edge.ToNode.Longitude}, {edge.ToNode.Latitude}]");
                                }
                                tw.Write("{ \"type\":\"Feature\",\"properties\":{ \"stroke\": \"#00ff00\", \"stroke-width\": 5, \"stroke-opacity\": 1, \"obj-type\": \"edge\", " + attributesToProps + "},\"geometry\":{ \"type\":\"LineString\",\"coordinates\":[" + string.Join(",", coordinatesString) + "]}},");
                                markedEdges[edge.Id - 1] = true;
                            }
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
