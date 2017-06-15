using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NGAT.Business.Domain.Core;
using System.Linq;
using System;

namespace NGAT.Services.IO.Exporters.GeoJSON
{
    public class GeoJSONGraphExporter : StreamGraphExporter
    {
        public override string FormatID => "GeoJSON";

        public GeoJSONGraphExporter(Stream stream, bool exportPoints = true) : base(stream)
        {
            ExportPoints = exportPoints;
        }

        public override void Export(Graph graph)
        {
            InternalExport(graph.NodesIndex);
        }

        public bool ExportPoints { get; set; }

        private void InternalExport(IDictionary<int, Node> nodesIndex)
        {
            var nodes = nodesIndex.Values;
            var markedEdges = new Dictionary<int, bool>();
            using (var tw = new StreamWriter(Stream))
            {
                tw.Write("{\"type\":\"FeatureCollection\",\"features\":");
                tw.Write("[");
                List<string> featuresStrings = new List<string>();
                foreach (var node in nodes)
                {
                    
                    var arcs = node.OutgoingArcs;//.Arcs.Where(a => a.FromNodeId == node.Id);//.Where(a => a.FromNode.Latitude <= -82.35806465148926 && a.FromNode.Longitude <= 23.145805714137563 && a.ToNode.Latitude >= -82.37866401672363 && a.ToNode.Longitude >= 23.122363841245967);
                    foreach (var arc in arcs.Where(a=>nodesIndex.ContainsKey(a.ToNodeId)))
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
                        featuresStrings.Add("{ \"type\":\"Feature\",\"properties\":{ \"stroke\": \"#ff0000\", \"stroke-width\": 3, \"stroke-opacity\": 1, \"obj-type\": \"arc\", " + attributesToProps + "},\"geometry\":{ \"type\":\"LineString\",\"coordinates\":[" + string.Join(",", coordinatesString) + "]}}");
                    }
                    foreach (var edge in node.Edges.Where(e=>nodesIndex.ContainsKey(e.FromNodeId)&&nodesIndex.ContainsKey(e.ToNodeId)))
                    {
                        if (!markedEdges.ContainsKey(edge.Id - 1))
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
                            featuresStrings.Add("{ \"type\":\"Feature\",\"properties\":{ \"stroke\": \"#00ff00\", \"stroke-width\": 5, \"stroke-opacity\": 1, \"obj-type\": \"edge\", " + attributesToProps + "},\"geometry\":{ \"type\":\"LineString\",\"coordinates\":[" + string.Join(",", coordinatesString) + "]}}");
                            markedEdges[edge.Id - 1] = true;
                        }
                    }
                    if(ExportPoints)
                        featuresStrings.Add("{ \"type\":\"Feature\",\"properties\":{ },\"geometry\":{ \"type\":\"Point\",\"coordinates\":[" + node.Longitude + "," + node.Latitude + "]}}");
                    

                }
                tw.Write(string.Join(",", featuresStrings));
                tw.Write("]");

                tw.Write("}");



            }
        }

        public override Task ExportAsync(Graph graph)
        {
            return new Task(() => InternalExport(graph.NodesIndex));
        }

        public override void ExportInRange(double minLat, double MinLong, double maxLat, double MaxLong, Graph graph)
        {
            InternalExport(new SortedDictionary<int, Node>(graph.Nodes.Where(n => n.Latitude >= minLat
                                              && n.Longitude >= MinLong
                                              && n.Latitude <= maxLat
                                              && n.Longitude <= MaxLong).ToDictionary(n => n.Id)));
        }

        public override Task ExportInRangeAsync(double minLat, double MinLong, double maxLat, double MaxLong, Graph graph)
        {
            return new Task(() => InternalExport(new SortedDictionary<int, Node>(graph.Nodes.Where(n => n.Latitude >= minLat
                                             && n.Longitude >= MinLong
                                             && n.Latitude <= maxLat
                                             && n.Longitude <= MaxLong).ToDictionary(n => n.Id))));
        }
    }
}
