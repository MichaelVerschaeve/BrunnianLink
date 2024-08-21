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
            neededSubPart.Clear();
            List<string> someColorNames = new() { "Red", "Blue", "Green", "Orange", "Purple", "Black" };
            var colorIds = someColorNames.Select(x => ColorMap.Get(x).id).ToList();

            for (int i = 0; i < 6; i++)
                GenerateRec(sb, new Rot30Coords(), i * 60, i, (i + 1) % 6, level);
            Shape wedgeLeft = new() { PartID = "65426" };  //   \|
            Shape wedgeRight = new() { PartID = "65429" }; //   |/
            Shape triangle = new() { PartID = "35787" }; // |\
            foreach (var p in neededSubPart)
            {
                MetaData.StartSubModel(sb, $"Rhomb_{p.r}_{p.id1}_{p.id2}");
                int id1 = colorIds[p.id1];
                int id2 = colorIds[p.id2];
                switch (p.r)
                {
                    case 0:
                        sb.AppendLine(wedgeRight.Rotate(90).Print(2, -1, 1, id1));
                        sb.AppendLine(wedgeLeft.Rotate(270).Print(2, 1, 1, id2));
                        sb.AppendLine(wedgeRight.Rotate(270).Print(6, 1, 1, id2));
                        sb.AppendLine(wedgeLeft.Rotate(90).Print(6, -1, 1, id1));
                        break;
                    case 30:
                        sb.AppendLine(wedgeRight.Rotate(180).Print(1, 2, 0, id2));
                        sb.AppendLine(wedgeRight.Print(5, 2, 0, id1));

                        sb.AppendLine(triangle.Rotate(90).Print(3, 2, 0, id1));
                        sb.AppendLine(triangle.Rotate(270).Print(5, 2, 0, id2));

                        sb.AppendLine(wedgeLeft.Rotate(90).Print(4, 3, 1, id2));
                        sb.AppendLine(wedgeLeft.Rotate(270).Print(2, 1, 1, id1));
                        break;
                    case 60:
                        sb.AppendLine(wedgeLeft.Rotate(90).Print(2,1, 0, id1));
                        sb.AppendLine(wedgeLeft.Rotate(270).Print(2,5, 0, id2));
                        sb.AppendLine(triangle.Rotate(90).Print(2, 3, 0, id1));
                        sb.AppendLine(triangle.Rotate(270).Print(2, 3, 0, id2));
                        sb.AppendLine(wedgeRight.Print(0, 2, 1, id2));
                        sb.AppendLine(wedgeRight.Rotate(180).Print(3, 4, 1, id1));
                        break;
                }
            }
        }

        public static void GenerateRec(StringBuilder sb, Rot30Coords c, int rotation, int id1, int id2, int level)
        {
            if (level == 0)
            {
                int rotMod = rotation % 90;
                int cutrot = (rotation / 90) * 90;
                sb.AppendLine(new Shape() { PartID = $"Rhomb_{rotMod}_{id1}_{id2}", SubModel = true }.Rotate(cutrot).Print(c.Cx, c.Cy, 0, 16));
                neededSubPart.Add((rotMod, id1, id2));
                return;
            }
            c *= 3;
            level--;
            Rot30Coords Xr = X.Rotate(rotation);
            Rot30Coords Vr = V.Rotate(rotation);
            GenerateRec(sb,c, (rotation+30)%360, id1, id2, level);
            GenerateRec(sb, c + Xr + Vr, (rotation + 270) % 360, id1, id2, level);
            GenerateRec(sb, c + 2*Xr - Vr, (rotation + 90) % 360, id1, id2, level);

        }

        static readonly HashSet<(int r, int id1, int id2)> neededSubPart = new();

        private static Rot30Coords X = new() { x = 1 };
        //private static Rot30Coords U = new() { u = 1 };
        private static Rot30Coords V = new() { v = 1 };
        //private static Rot30Coords Y = new() { y = 1 };



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
            public Rot30Coords(int x, int u, int v, int y)
            {
                this.x = x; this.u = u; this.v = v; this.y = y;
            }
        }


    }
}
