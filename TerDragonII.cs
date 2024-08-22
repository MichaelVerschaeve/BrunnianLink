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

            while (rotations.Count > 1)
            {
                bool closed = rotations.Count > 2 && rotations[0] + rotations[1] == -rotations[2]; //closed triangle
                bool counterclockwise = rotations[0].Rotate(120) == rotations[1];
                int rotation = Enumerable.Range(0, 6).Select(t => 60 * t).First(a=>X.Rotate(a)==rotations[0]);
                for (int i = 0; i < level; i++)
                {
                    if (counterclockwise)
                        DrawTriangle(sb, currentPos.Rotate(i * 60), (rotation + i * 60) % 360, closed ? 3 : 1, colorIds[i], colorIds[(i + 1) % 6]);
                    else
                        DrawTriangle(sb, currentPos.Rotate(i * 60), (rotation + i * 60 + 300) % 360, closed ? 3 : 2, colorIds[(i + 1) % 6], colorIds[i]);
                }

                if (closed)
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

        private static void DrawTriangle(StringBuilder sb, Rot30Coords c, int rotation, int cornerflags, int colorOutId, int colorInId)
        {
            switch (rotation)
            {
                case 0:
                    DrawUp(sb, c.Cx + 3, c.Cy + 6, (cornerflags & 2) == 2, false, (cornerflags & 1) == 1, colorOutId, colorInId);
                    break;
                case 60:
                    DrawDown(sb, c.Cx, c.Cy, (cornerflags & 2) == 2, (cornerflags & 1) == 1, false, colorOutId, colorInId);
                    break;
                case 120:
                    DrawUp(sb, c.Cx - 3, c.Cy + 6, (cornerflags & 1) == 1, (cornerflags & 2) == 2, false, colorOutId, colorInId);
                    break;
                case 180:
                    DrawDown(sb, c.Cx - 3, c.Cy - 6, (cornerflags & 1) == 1, false, (cornerflags & 2) == 2, colorOutId, colorInId);
                    break;
                case 240:
                    DrawUp(sb, c.Cx, c.Cy, false, (cornerflags & 1) == 1, (cornerflags & 2) == 2, colorOutId, colorInId);
                    break;
                case 300:
                    DrawDown(sb, c.Cx + 3, c.Cy - 6, false, (cornerflags & 1) == 1, (cornerflags & 2) == 2,  colorOutId, colorInId);
                    break;

            }
        }

        private static void DrawUp(StringBuilder sb, int x, int y, bool top, bool left, bool right, int colorOutId, int colorInId)
        {
            Shape wedgeLeft = new() { PartID = "65426" };  //   \|
            Shape wedgeRight = new() { PartID = "65429" }; //   |/
            Shape swLeft = new() { PartID = "24307" };  //   \|
            Shape swRight = new() { PartID = "24299" }; //   |/
            Plate p = new(2, 2);
            sb.AppendLine(wedgeLeft.Rotate(180).Print(x+1,y-2,0,top?colorOutId:colorInId) );
            sb.AppendLine(wedgeRight.Rotate(180).Print(x-1,y-2,0,top?colorOutId:colorInId) );
            sb.AppendLine(p.Print(x , y - 5, 0, colorInId));
            sb.AppendLine(swLeft.Rotate(180).Print(x + 2, y - 5, 0, right ? colorOutId : colorInId));
            sb.AppendLine(swRight.Rotate(180).Print(x - 2, y - 5, 0, left ? colorOutId : colorInId));
            DrawHex(sb, x, y - 4, colorInId);
        }

        private static void DrawDown(StringBuilder sb, int x, int y, bool left, bool right, bool bottom, int colorOutId, int colorInId)
        {
            Shape wedgeLeft = new() { PartID = "65426" };  //   \|
            Shape wedgeRight = new() { PartID = "65429" }; //   |/
            Shape swLeft = new() { PartID = "24307" };  //   \|
            Shape swRight = new() { PartID = "24299" }; //   |/
            Plate p = new(2, 2);
            sb.AppendLine(wedgeLeft.Print(x - 1, y + 2, 0, bottom ? colorOutId : colorInId));
            sb.AppendLine(wedgeRight.Print(x + 1, y + 2, 0, bottom ? colorOutId : colorInId));
            sb.AppendLine(p.Print(x, y + 5, 0, colorInId));
            sb.AppendLine(swLeft.Print(x - 2, y + 5, 0, left ? colorOutId : colorInId));
            sb.AppendLine(swRight.Print(x + 2, y + 5, 0, right ? colorOutId : colorInId));
            DrawHex(sb, x, y - 4, colorInId);
        }

        private static void DrawHex(StringBuilder sb, int x, int y, int colorInId)
        {
            Shape swLeft = new() { PartID = "24307" };  //   \|
            Shape swRight = new() { PartID = "24299" }; //   |/
            sb.AppendLine(swLeft.Rotate(180).Print(x + 1, y +1, 1,  colorInId));
            sb.AppendLine(swRight.Rotate(180).Print(x - 1, y +1, 1, colorInId));
            sb.AppendLine(swLeft.Print(x - 1, y - 1, 1, colorInId));
            sb.AppendLine(swRight.Print(x + 1, y - 1, 1, colorInId));
        }



        private static Rot30Coords X = new() { x = 1 };
        private static Rot30Coords U = new() { u = 1 };
        //private static Rot30Coords V = new() { v = 1 };
        //private static Rot30Coords Y = new() { y = 1 };


        public struct Rot30Coords
        {
            public int x;
            public int u;
            public int v;
            public int y;
            public readonly int Cx => 6 * (x + u) + 3 * v;
            public readonly int Cy => 6 * (v + y) + 3 * u;

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

            public static bool operator ==(Rot30Coords a, Rot30Coords b)
            => (a.x, a.u, a.v, a.y) == (b.x, b.u, b.v, b.y);
            public static bool operator !=(Rot30Coords a, Rot30Coords b)
            => (a.x, a.u, a.v, a.y) != (b.x, b.u, b.v, b.y);

            public Rot30Coords(int x, int u, int v, int y)
            {
                this.x = x; this.u = u; this.v = v; this.y = y;
            }

            public override bool Equals(object? obj)
            {
                throw new NotImplementedException();
            }

            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }
        }


    }
}
