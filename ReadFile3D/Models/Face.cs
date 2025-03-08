using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadFile3D.Models
{
    class Face
    {
        public int[] Indices { get; }

        // Constructor to initialize the face with the index list
        public Face(int[] indieces) {
            Indices = indieces;
        }

    }
}
