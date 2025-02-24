using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCEP
{
    public class Vertex
    {
        public int Id { get; set; }
        public List<Edge> IncomingEdges { get; set; }
        public List<Edge> OutgoingEdges { get; set; }

        public Vertex(int id)
        {
            Id = id;
            IncomingEdges = new List<Edge>();
            OutgoingEdges = new List<Edge>();
        }
    }
}
