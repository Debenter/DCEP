using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace DCEP
{
    public class GraphReader
    {
        public Graph ReadGraphFromFile(string filePath)
        {
            var graph = new Graph();
            var vertices = new Dictionary<int, Vertex>();

            using (var reader = new StreamReader(filePath))
            {
                string line;
                bool readingEdges = true; // Flaga, która przełącza pomiędzy krawędziami i ograniczeniami

                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        // Jeśli linia jest pusta, przełącz na czytanie ograniczeń odległościowych
                        readingEdges = false;
                        continue;
                    }

                    var tokens = line.Split(',');

                    if (readingEdges)
                    {
                        // Czytanie krawędzi
                        int startId = int.Parse(tokens[0].Trim());
                        int endId = int.Parse(tokens[1].Trim());
                        double weight = double.Parse(tokens[2].Trim(), CultureInfo.InvariantCulture);

                        // Dodanie wierzchołków, jeśli jeszcze nie istnieją
                        if (!vertices.ContainsKey(startId))
                        {
                            vertices[startId] = new Vertex(startId);
                            graph.Vertices.Add(vertices[startId]); // Make sure to add vertices to graph
                        }
                        if (!vertices.ContainsKey(endId))
                        {
                            vertices[endId] = new Vertex(endId);
                            graph.Vertices.Add(vertices[endId]); // Make sure to add vertices to graph
                        }

                        var edge = new Edge(vertices[startId], vertices[endId], weight);
                        vertices[startId].OutgoingEdges.Add(edge);
                        vertices[endId].IncomingEdges.Add(edge);
                        graph.Edges.Add(edge);
                    }
                    else
                    {
                        // Czytanie ograniczeń odległościowych
                        int startId = int.Parse(tokens[0].Trim());
                        int endId = int.Parse(tokens[1].Trim());
                        double minDistance = double.Parse(tokens[2].Trim(), CultureInfo.InvariantCulture);
                        double maxDistance = double.Parse(tokens[3].Trim(), CultureInfo.InvariantCulture);

                        var constraint = new DistanceConstraint(vertices[startId], vertices[endId], minDistance, maxDistance);
                        graph.Constraints.Add(constraint);
                    }
                }
            }

            return graph;
        }
    }
}
