using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadFile3D.Models
{
    class Vertex
    {
        public float X { get; }
        public float Y { get; }
        public float Z { get; }

        // Construtor to initialize the coordinates of 3D vertex
        public Vertex(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
