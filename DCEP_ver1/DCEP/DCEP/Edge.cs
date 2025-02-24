using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCEP
{
    public class Edge
    {
        public Vertex Start { get; set; }
        public Vertex End { get; set; }
        public double Weight { get; set; }

        public Edge(Vertex start, Vertex end, double weight)
        {
            Start = start;
            End = end;
            Weight = weight;
        }
    }
}