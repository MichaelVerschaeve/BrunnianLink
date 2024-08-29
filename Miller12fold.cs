using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    public static class Miller12fold
    {
        static readonly int starId = ColorMap.Get("White").id;
        static readonly int triangleId = ColorMap.Get("Red").id;
        static readonly int rhombId = ColorMap.Get("Dark_Blue").id;


        public static void Generate(StringBuilder sb, int level)
        {
            visitedMids.Clear();
            MetaData.StartSubModel(sb, $"Miller12_{level}");
            sb.AppendLine(new Tile(2, 2).Print(0, 0, -1, 4));

            GenerateTriangle(sb, new Rot30Coords(), 0, level);
            //GenerateStar(sb, new Rot30Coords(), (level-1)*30, 1);
            Shape wRight = new() { PartID = "65429" };
            Shape wLeft = new() { PartID = "65426" };
            //triangle subpart
            MetaData.StartSubModel(sb, "Triangle");
            sb.AppendLine(wLeft.Rotate(180).Print(1, -2, 1, triangleId));
            sb.AppendLine(wRight.Rotate(180).Print(-1, -2, 1, triangleId));

            MetaData.StartSubModel(sb, "Star");
            sb.AppendLine(wLeft.Rotate(180).Print(1, -2, 1, starId));
            sb.AppendLine(wRight.Rotate(90).Print(4, -5, 1, starId));
            sb.AppendLine(new Plate(4, 4).Print(0, -6, 0, starId)); //sunken;

            MetaData.StartSubModel(sb, "Rhomb_0");
            sb.AppendLine(wLeft.Rotate(270).Print(2, 1, 1, rhombId));
            sb.AppendLine(wLeft.Rotate(90).Print(6, 1, 1, rhombId));
            MetaData.StartSubModel(sb, "Rhomb_30");
            sb.AppendLine(wRight.Print(5, 4, 0, rhombId));
            sb.AppendLine(wLeft.Rotate(90).Print(2, 1, 0, rhombId));
            sb.AppendLine(new Shape() {PartID="2639" }.Rotate(90).Print(3, 3, 0, rhombId)); //sunken, try to orient bottom outwards
            MetaData.StartSubModel(sb, "Rhomb_60");
            sb.AppendLine(wRight.Print(1, 2, 1, rhombId));
            sb.AppendLine(wRight.Rotate(180).Print(1, 6, 1, rhombId));
        }

        private static Rot30Coords X = new() { x = 1 };
        private static Rot30Coords U = new() { u = 1 };
        private static Rot30Coords V = new() { v = 1 };
        private static Rot30Coords Y = new() { y = 1 };


        private static readonly HashSet<(double x, double y, int level)> visitedMids = new();


        //rotation = 0 -> uppointing, top is origin
        public static void GenerateTriangle(StringBuilder sb, Rot30Coords c, int rotation, int level)
        {
            Rot30Coords otherPoint1 = c - V.Rotate(rotation);
            Rot30Coords otherPoint2 = otherPoint1 + X.Rotate(rotation);
            double midx = (c.Cx + otherPoint1.Cx + otherPoint2.Cx) / 3.0;
            double midy = (c.Cy + otherPoint1.Cy + otherPoint2.Cy) / 3.0;
            if (visitedMids.Contains((midx, midy, level)))
                return;
            visitedMids.Add((midx, midy, level));
            if (level == 0)
            {
                Rot30Coords dc = new();
                int drot = 0;
                switch (rotation % 90)
                {
                    case 0:
                        break;
                    case 30:
                        dc = U - Y;
                        drot = 270;
                        break;
                    case 60:
                        dc = X - V;
                        drot = 180;
                        break;
                }
                int cutrot = (rotation / 90) * 90;
                c += dc.Rotate(cutrot);
                rotation = drot + cutrot;
                if (rotation >= 360) rotation -= 360;
                sb.AppendLine(new Shape() { PartID = "Triangle", SubModel = true }.Rotate(rotation).Print(c.Cx, c.Cy, 0, 16));
                return;

            }
            //enlarge, rotate by 15 degrees...
            c += c.Rotate(30);
            level--;
            Rot30Coords Xr = X.Rotate(rotation);
            Rot30Coords Ur = U.Rotate(rotation);
            Rot30Coords Vr = V.Rotate(rotation);
            Rot30Coords Yr = Y.Rotate(rotation);
            GenerateStar(sb, c, rotation, level);
            GenerateRhomb(sb, c - Vr - Yr, rotation, level);
            GenerateRhomb(sb, c + Ur - Yr - Vr + Xr, (rotation + 120) % 360, level);
            GenerateRhomb(sb, c, (rotation+240)%360, level);
        }


        public static void GenerateRhomb(StringBuilder sb, Rot30Coords c, int rotation, int level)
        {
            Rot30Coords otherPoint = c + X.Rotate(rotation)+U.Rotate(rotation);
            double midx = (c.Cx + otherPoint.Cx)  * 0.5;
            double midy = (c.Cy + otherPoint.Cy)  * 0.5;
            if (visitedMids.Contains((midx, midy, level)))
                return;
            visitedMids.Add((midx, midy, level));
            if (level == 0)
            {
                sb.AppendLine(new Shape() { PartID = $"Rhomb_{rotation%90}", SubModel = true }.Rotate((rotation/90)*90).Print(c.Cx, c.Cy, 0, 16));
                return;
            }
            c += c.Rotate(30);
            level--;
            Rot30Coords Xr = X.Rotate(rotation);
            Rot30Coords Ur = U.Rotate(rotation);
            Rot30Coords Vr = V.Rotate(rotation);
            Rot30Coords Yr = Y.Rotate(rotation);
            GenerateRhomb(sb, c, rotation, level);
            GenerateRhomb(sb, c+Ur+Vr, (rotation+210)%360, level);
            GenerateTriangle(sb, c + Ur,(rotation + 120) % 360, level);
            c += Xr + 2 * Ur + Vr;
            GenerateRhomb(sb, c, (rotation+180)%360, level);
            GenerateRhomb(sb, c - Ur - Vr, (rotation + 30) % 360, level);
            GenerateTriangle(sb, c - Ur, (rotation + 300) % 360, level);
        }

        //rotation = 0 -> top is origin left of uppointing star arm is vertical
        public static void GenerateStar(StringBuilder sb, Rot30Coords c, int rotation, int level)
        {
            Rot30Coords otherPoint1 = c - V.Rotate(rotation) - Y.Rotate(rotation);
            Rot30Coords otherPoint2 = otherPoint1 + X.Rotate(rotation)+U.Rotate(rotation);
            double midx = (c.Cx + otherPoint1.Cx + otherPoint2.Cx) / 3.0;
            double midy = (c.Cy + otherPoint1.Cy + otherPoint2.Cy) / 3.0;
            if (visitedMids.Contains((midx, midy, level)))
                return;
            visitedMids.Add((midx, midy, level));
            if (level == 0)
            {
                Rot30Coords dc = new();
                int drot = 0;
                switch (rotation % 90)
                {
                    case 0:
                        break;
                    case 30:
                        dc = X - Y + U;
                        drot = 270;
                        break;
                    case 60:
                        dc = U - Y - V + X;
                        drot = 180;
                        break;
                }
                int cutrot = (rotation / 90) * 90;
                c += dc.Rotate(cutrot);
                rotation = drot + cutrot;
                if (rotation >= 360) rotation -= 360;
                sb.AppendLine(new Shape() { PartID = "Star", SubModel = true }.Rotate(rotation).Print(c.Cx, c.Cy, 0, 16));
                return;
            }
            c += c.Rotate(30);
            level--;
            Rot30Coords Xr = X.Rotate(rotation);
            Rot30Coords Ur = U.Rotate(rotation);
            Rot30Coords Vr = V.Rotate(rotation);
            Rot30Coords Yr = Y.Rotate(rotation);
            GenerateTriangle(sb, c -(Yr+Vr)+Xr, (rotation + 90) % 360, level); //center triangle

            for (int i = 0; i < 3; i++)
            {
                int subrotation = (rotation + i * 120) % 360;
                Xr = X.Rotate(subrotation);
                Ur = U.Rotate(subrotation);
                Vr = V.Rotate(subrotation);
                Yr = Y.Rotate(subrotation);

                GenerateRhomb(sb, c, (subrotation+270), level);
                GenerateRhomb(sb, c + Ur- Yr - Vr+Xr, (subrotation + 120) % 360, level);
                GenerateTriangle(sb, c -Vr + Xr, (subrotation + 30) % 360, level);
                c += Xr - 2 * (Vr + Yr);
            }
        }

        //rotation = 0 -> to the right 
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

            public Rot30Coords(int x, int u, int v, int y)
            {
                this.x = x; this.u = u; this.v = v; this.y = y;
            }
        }

    }
}
