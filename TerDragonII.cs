using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    public static class TerDragonII
    {

        public static void Generate(StringBuilder sb, int level)
        {
            MetaData.StartSubModel(sb, $"TerDragonII_{level}");
            List<string> someColorNames = new() { "Red", "Blue", "Green", "Orange", "Purple", "Black" };
            var colorIds = someColorNames.Select(x => ColorMap.Get(x).id).ToList();

            //rotations as Rot30Coords vectors...
            List<Rot30Coords> rotations = new() { ((level&1)==0)?X:U }; //only up down triangles

            for (int i = 0; i < level; i++)
            {
                List<Rot30Coords> newrotations = new();
                foreach(var rot in rotations)
                {
                    newrotations.Add(rot.Rotate(30));
                    newrotations.Add(rot.Rotate(270));
                    newrotations.Add(rot.Rotate(30));
                }
                rotations = newrotations;
            }

            Rot30Coords currentPos = new();

            while (rotations.Count>1)
            {
                if (rotations.Count > 2 && rotations[0] + rotations[1] == -rotations[2])
                {

                    rotations.RemoveRange(0, 2);
                }
                else
                {

                    currentPos += rotations[0];
                    rotations.RemoveAt(0);
                }
            }

            /*
            Shape wedgeLeft = new() { PartID = "65426" };  //   \|
            Shape wedgeRight = new() { PartID = "65429" }; //   |/
            Shape triangle = new() { PartID = "35787" }; // |\
            */
        }


        private static Rot30Coords X = new() { x = 1 };
        private static Rot30Coords U = new() { u = 1 };
        private static Rot30Coords V = new() { v = 1 };
        private static Rot30Coords Y = new() { y = 1 };


        public struct Rot30Coords
        {
            public int x;
            public int u;
            public int v;
            public int y;
            public readonly int Cx => 4 * x + 4 * u + 2 * v;
            public readonly int Cy => 2 * u + 4 * v + 4 * y;

            public readonly Rot30Coords Rotate(int rot)
            {
                Rot30Coords res = this;
                for (int r = 0; r < rot; r += 30)
                    (res.x, res.u, res.v, res.y) = (-res.y, res.x, res.u + res.y, res.v);

                return res;
            }
            public static Rot30Coords operator +(Rot30Coords a, Rot30Coords b)
            => new() { x = a.x + b.x, u = a.u + b.u, v = a.v + b.v, y = a.y + b.y };
            public static Rot30Coords operator -(Rot30Coords a, Rot30Coords b)
            => new() { x = a.x - b.x, u = a.u - b.u, v = a.v - b.v, y = a.y - b.y };
            public static Rot30Coords operator -(Rot30Coords a)
            => new() { x = -a.x, u = -a.u, v = -a.v, y = -a.y };
            public static Rot30Coords operator *(int s, Rot30Coords a)
            => new() { x = s * a.x, u = s * a.u, v = s * a.v, y = s * a.y };
            public static Rot30Coords operator *(Rot30Coords a, int s)
            => new() { x = s * a.x, u = s * a.u, v = s * a.v, y = s * a.y };

            public static bool operator==(Rot30Coords a, Rot30Coords b)
            => (a.x, a.u, a.v, a.y) == (b.x, b.u, b.v, b.y);
            public static bool operator!=(Rot30Coords a, Rot30Coords b)
            => (a.x, a.u, a.v, a.y)!=(b.x, b.u, b.v, b.y);

            public Rot30Coords(int x, int u, int v, int y)
            {
                this.x = x; this.u = u; this.v = v; this.y = y;
            }
        }


    }
}
