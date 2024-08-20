using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BrunnianLink.ShieldFlatTiling;

namespace BrunnianLink
{
    static public class MillarTiling
    {
        static readonly int starId = ColorMap.Get("Dark_Blue").id;
        static readonly int rhombId = ColorMap.Get("Bright_Light_Orange").id;
        static readonly int squareId = ColorMap.Get("Green").id;



        public static void Generate(StringBuilder sb, int level)
        {
            visitedMids.Clear();
            MetaData.StartSubModel(sb, $"Millar_{level}");

            GenerateSquare(sb, new Rot45Coords(), 0, level);
            sb.AppendLine(new Tile(2, 2).Print(0, 0, -1, 4)); //mark center

            MetaData.StartSubModel(sb, $"Star");
            Shape w = new() { PartID = "35787" };
            Plate square = new(3,3);
            sb.AppendLine(square.Print(0, 0, 0,starId));
            sb.AppendLine(w.Print(2.5, -0.5, 0, starId));
            sb.AppendLine(w.Rotate(90).Print(0.5, 2.5, 0, starId));
            sb.AppendLine(w.Rotate(180).Print(-2.5, 0.5, 0, starId));
            sb.AppendLine(w.Rotate(270).Print(-0.5, -2.5, 0, starId));
            MetaData.StartSubModel(sb, $"Star_Tilt45");
            sb.AppendLine(square.Print(0, 0, 0, starId));
            sb.AppendLine(w.Print(-0.5, 2.5, 0, starId));
            sb.AppendLine(w.Rotate(90).Print(-2.5, -0.5, 0, starId));
            sb.AppendLine(w.Rotate(180).Print(0.5, -2.5, 0, starId));
            sb.AppendLine(w.Rotate(270).Print(2.5, 0.5, 0, starId));
            MetaData.StartSubModel(sb, $"Rhomb");
            Tile rect  = new(1, 2);
            sb.AppendLine(rect.Print(0, 0, 0, rhombId));
            sb.AppendLine(w.Print(1.5, 0, 0, rhombId));
            sb.AppendLine(w.Rotate(180).Print(-1.5, 0, 0, rhombId));
            MetaData.StartSubModel(sb, $"Rhomb_Tilt45");
            sb.AppendLine(rect.Print(0, 0, 0, rhombId));
            sb.AppendLine(w.Rotate(270).Print(1.5, 0, 0, rhombId));
            sb.AppendLine(w.Rotate(90).Print(-1.5, 0, 0, rhombId));
        }

        private static Rot45Coords X = new() { x = 1 };
        private static Rot45Coords U = new() { u = 1 };
        private static Rot45Coords V = new() { v = 1 };
        private static Rot45Coords Y = new() { y = 1 };


        private static readonly HashSet<(double x, double y, int level)> visitedMids = new();


        //square is at zero degrees parallel to x and y axises and origin is in top corner
        public static void GenerateSquare(StringBuilder sb, Rot45Coords c, int rotation, int level)
        {
            Rot45Coords otherPoint = c + (X - Y).Rotate(rotation);
            double midx = (c.Cx + otherPoint.Cx) * 0.5;
            double midy = (c.Cy + otherPoint.Cy) * 0.5;
            if (visitedMids.Contains((midx, midy, level)))
                return;
            visitedMids.Add((midx, midy, level));

            if (level == 0)
            {
                if (rotation % 90 == 0)
                    sb.AppendLine(new Plate(3, 3).Print(midx, midy, 0, squareId));
                else
                {
                    Shape w = new() { PartID = "35787" };
                    sb.AppendLine(w.Print(midx + 1, midy + 1, 0, squareId));
                    sb.AppendLine(w.Rotate(90).Print(midx - 1, midy + 1, 0, squareId));
                    sb.AppendLine(w.Rotate(180).Print(midx - 1, midy - 1, 0, squareId));
                    sb.AppendLine(w.Rotate(270).Print(midx + 1, midy - 1, 0, squareId));
                }
                return;
            }
            c += c.Rotate(45); //rotate 22.5 degrees...
            level--;
            Rot45Coords Xr = X.Rotate(rotation);
            Rot45Coords Ur = U.Rotate(rotation);
            Rot45Coords Vr = V.Rotate(rotation);
            Rot45Coords Yr = Y.Rotate(rotation);
            GenerateStar(sb, c, rotation, level);
            GenerateRhomb(sb, c, (rotation + 45) % 360, level);
            GenerateRhomb(sb, c - Yr + Ur, (rotation + 45) % 360, level);
            GenerateRhomb(sb, c, (rotation + 360 - 45) % 360, level);
            GenerateRhomb(sb, c + Xr + Vr, (rotation + 360 - 45) % 360, level);
        }


        //star is at zero degrees fitting into a standard square tilted 22.5 degrees
        public static void GenerateStar(StringBuilder sb, Rot45Coords c, int rotation, int level)
        {
            Rot45Coords otherPoint = c + (U - Y + V + X).Rotate(rotation);
            double midx = (c.Cx + otherPoint.Cx) * 0.5;
            double midy = (c.Cy + otherPoint.Cy) * 0.5;
            if (visitedMids.Contains((midx, midy, level)))
                return;
            visitedMids.Add((midx, midy, level));
            if (level == 0)
            {
                if (rotation % 90 == 0)
                    sb.AppendLine(new Shape() { PartID = "Star", SubModel = true }.Rotate(rotation).Print(midx, midy, 0, 16));
                else
                    sb.AppendLine(new Shape() { PartID = "Star_Tilt45", SubModel = true }.Rotate(rotation - 45).Print(midx, midy, 0, 16));
                return;
            }
            c += c.Rotate(45); //rotate 22.5 degrees...
            level--;
            Rot45Coords Xr = X.Rotate(rotation);
            Rot45Coords Ur = U.Rotate(rotation);
            Rot45Coords Vr = V.Rotate(rotation);
            //Rot45Coords Yr = Y.Rotate(rotation);

            Rot45Coords origin = c + Xr + Ur + Vr;
            for (int i = 0; i < 4; i++)
            {
                int subrotation = (rotation + i * 90) % 360;
                GenerateSquare(sb, origin, (subrotation + 45) % 360, level);
                Ur = U.Rotate(subrotation);
                Vr = V.Rotate(subrotation);
                GenerateRhomb(sb, origin+Vr, subrotation, level);
                GenerateRhomb(sb, origin + Ur, (subrotation + 45) % 360, level);
            }
        }

        //    _____
        //    \    \
        //     \____\
        public static void GenerateRhomb(StringBuilder sb, Rot45Coords c, int rotation, int level)
        {
            Rot45Coords otherPoint = c + (U + X).Rotate(rotation);
            double midx = (c.Cx + otherPoint.Cx) * 0.5;
            double midy = (c.Cy + otherPoint.Cy) * 0.5;
            if (visitedMids.Contains((midx, midy, level)))
                return;
            visitedMids.Add((midx, midy, level));
            if (level == 0)
            {
                if (rotation % 90 == 0)
                    sb.AppendLine(new Shape() { PartID = "Rhomb", SubModel = true }.Rotate(rotation).Print(midx, midy, 0, 16));
                else
                    sb.AppendLine(new Shape() { PartID = "Rhomb_Tilt45", SubModel = true }.Rotate(rotation-45).Print(midx, midy,0, 16));
                return;
            }
            c += c.Rotate(45); //rotate 22.5 degrees...
            level--;
            Rot45Coords Xr = X.Rotate(rotation);
            Rot45Coords Ur = U.Rotate(rotation);
            Rot45Coords Vr = V.Rotate(rotation);
           // Rot45Coords Yr = Y.Rotate(rotation);
            GenerateRhomb(sb, c, rotation, level);
            GenerateRhomb(sb, c, (rotation+45)%360, level);
            GenerateRhomb(sb, c + Xr + Vr, rotation, level);
            GenerateRhomb(sb, c + Xr + Ur, (rotation+45)%360, level);
            GenerateSquare(sb, c+Xr, (rotation + 45) % 360, level);
        }

        public struct Rot45Coords
        {
            public int x; //0 degrees
            public int u; //-45 degrees
            public int v;//45 degrees
            public int y; //90 degrees
            public readonly int Cx => 2*(u+v)+3*x;
            public readonly int Cy => 2*(v-u) + 3 * y;

            public readonly Rot45Coords Rotate(int rot)
            {
                Rot45Coords res = this;
                for (int r = 0; r < rot; r += 45)
                    (res.x, res.u, res.v, res.y) = (res.u, -res.y, res.x, res.v);
                return res;
            }
            public static Rot45Coords operator +(Rot45Coords a, Rot45Coords b)
            => new() { x = a.x + b.x, u = a.u + b.u, v = a.v + b.v, y = a.y + b.y };
            public static Rot45Coords operator -(Rot45Coords a, Rot45Coords b)
            => new() { x = a.x - b.x, u = a.u - b.u, v = a.v - b.v, y = a.y - b.y };
            public static Rot45Coords operator -(Rot45Coords a)
            => new() { x = -a.x, u = -a.u, v = -a.v, y = -a.y };
            public static Rot45Coords operator *(int s, Rot45Coords a)
            => new() { x = s * a.x, u = s * a.u, v = s * a.v, y = s * a.y };

            public Rot45Coords(int x, int u, int v, int y)
            {
                this.x = x; this.u = u; this.v = v; this.y = y;
            }
        }
    }
}
