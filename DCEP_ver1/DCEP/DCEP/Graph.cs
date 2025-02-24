using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCEP
{
    public class Graph
    {
        public List<Vertex> Vertices { get; set; }
        public List<Edge> Edges { get; set; }
        public List<DistanceConstraint> Constraints { get; set; }

        public Graph()
        {
            Vertices = new List<Vertex>();
            Edges = new List<Edge>();
            Constraints = new List<DistanceConstraint>();
        }

        // Metoda dodająca wierzchołki sztuczne 's' (source) i 't' (target) do grafu
        public void AddDummyVertices()
        {
            Vertex source = new Vertex(-1); // 's' jako -1
            Vertex target = new Vertex(-2); // 't' jako -2

            // Dodanie wierzchołków do listy wierzchołków
            Vertices.Add(source);
            Vertices.Add(target);

            // Połączenie wierzchołka 's' z każdym wierzchołkiem w grafie
            foreach (var vertex in Vertices)
            {
                if (vertex.Id != -1 && vertex.Id != -2) // Pomijamy 's' i 't'
                {
                    var edge = new Edge(source, vertex, 0);
                    Edges.Add(edge); // Dodanie krawędzi do listy krawędzi
                    source.OutgoingEdges.Add(edge); // Dodanie krawędzi wychodzącej z 's'
                    vertex.IncomingEdges.Add(edge); // Dodanie krawędzi przychodzącej do wierzchołka
                }
            }

            // Połączenie każdego wierzchołka w grafie z wierzchołkiem 't'
            foreach (var vertex in Vertices)
            {
                if (vertex.Id != -1 && vertex.Id != -2) // Pomijamy 's' i 't'
                {
                    var edge = new Edge(vertex, target, 0);
                    Edges.Add(edge); // Dodanie krawędzi do listy krawędzi
                    vertex.OutgoingEdges.Add(edge); // Dodanie krawędzi wychodzącej z wierzchołka
                    target.IncomingEdges.Add(edge); // Dodanie krawędzi przychodzącej do 't'
                }
            }
        }
    }
}
