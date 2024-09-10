using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design.Behavior;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BrunnianLink
{

    public static class RhombSquareOcta
    {
        static readonly int rhombId = ColorMap.Get("Red").id;
        static readonly int octaId = ColorMap.Get("Bright_Light_Orange").id;
        static readonly int squareId = ColorMap.Get("Dark_Blue").id;

        private static readonly HashSet<(double x, double y, int level)> visitedMids = new();

        public static void Generate(StringBuilder sb, int level)
        {
            visitedMids.Clear();
            MetaData.StartSubModel(sb, $"RhombSquareOctagon_{level}");
            GenerateSquare(sb, new Rot45Coords(), 0, level);
            MetaData.StartSubModel(sb, "Octagon"); //centered one
            Shape wTile = new() { PartID = "35787" };// |\
            Shape wedge3x3 = new() { PartID = "2450" };// |/
            sb.AppendLine(new Plate(1, 1).Print(0, 0, 0, octaId));
            sb.AppendLine(new Plate(3, 1).Print(2, 0, 0, octaId));
            sb.AppendLine(new Plate(3, 1).Print(-2, 0, 0, octaId));
            sb.AppendLine(new Plate(1, 3).Print(0, 2, 0, octaId));
            sb.AppendLine(new Plate(1, 3).Print(0, -2, 0, octaId));
            sb.AppendLine(wedge3x3.Rotate(90).Print(2,2,0,octaId));
            sb.AppendLine(wedge3x3.Rotate(180).Print(-2, 2, 0, octaId));
            sb.AppendLine(wedge3x3.Rotate(270).Print(-2, -2, 0, octaId));
            sb.AppendLine(wedge3x3.Print(2, -2, 0, octaId));
            MetaData.StartSubModel(sb, "Square_0"); //centered one
            sb.AppendLine(new Plate(3, 3).Print(0, 0, 0, squareId));
            MetaData.StartSubModel(sb, "Square_45"); //centered one
            sb.AppendLine(wTile.Print(1, 1, 0, squareId));
            sb.AppendLine(wTile.Rotate(90).Print(-1, 1, 0, squareId));
            sb.AppendLine(wTile.Rotate(180).Print(-1, -1, 0, squareId));
            sb.AppendLine(wTile.Rotate(270).Print(1, -1, 0, squareId));
            MetaData.StartSubModel(sb, "Rhomb_0");
            sb.AppendLine(wTile.Rotate(90).Print(1, 4, 0, rhombId));
            sb.AppendLine(wTile.Rotate(270).Print(1, 1, 0, rhombId));
            sb.AppendLine(new Plate(2, 1).Print(1, 2.5, 0, rhombId));
            MetaData.StartSubModel(sb, "Rhomb_45");
            sb.AppendLine(wTile.Print(-1, 4, 0, rhombId));
            sb.AppendLine(wTile.Rotate(180).Print(-1, 1, 0, rhombId));
            sb.AppendLine(new Plate(2, 1).Print(-1, 2.5, 0, rhombId));
        }

        //origin bottom-left (more bottom than left)
        public static void GenerateOctagon(StringBuilder sb, Rot45Coords c, int rotation, int level)
        {
            Rot45Coords Xr = X.Rotate(rotation);
            Rot45Coords Ur = U.Rotate(rotation);
            Rot45Coords Vr = V.Rotate(rotation);
            Rot45Coords Yr = Y.Rotate(rotation);
            Rot45Coords otherPoint = c + Xr + Yr + Vr - Ur;
            double midx = (c.Cx + otherPoint.Cx) * 0.5;
            double midy = (c.Cy + otherPoint.Cy) * 0.5;
            if (visitedMids.Contains((midx, midy, level)))
                return;
            visitedMids.Add((midx, midy, level));
            if (level == 0)
            {
                sb.AppendLine(new Shape() { PartID = "Octagon", SubModel = true }.Print(midx, midy, 0, 16));
                return;
            }
            c += c + c.Rotate(45) + c.Rotate(360 - 45);
            level--;
            Rot45Coords center = c + Xr + 2*Yr + 2*Vr-Ur;
            for (int i = 0; i < 8; i++)
            {
                int subrotation = (rotation + i * 45) % 360;
                Xr = X.Rotate(subrotation);
                //Ur = U.Rotate(subrotation);
                Vr = V.Rotate(subrotation);
                Yr = Y.Rotate(subrotation);
                GenerateRhomb(sb, center, subrotation, level);
                GenerateSquare(sb, center + Vr, subrotation, level);
                GenerateOctagon(sb, center + Vr + Yr, subrotation, level);
                GenerateSquare(sb, center + 2*Vr+Xr+Yr, subrotation, level);
            }
        }
        


        public static void GenerateSquare(StringBuilder sb, Rot45Coords c, int rotation, int level)
        {
            Rot45Coords Xr = X.Rotate(rotation);
           // Rot45Coords Ur = U.Rotate(rotation);
            Rot45Coords Vr = V.Rotate(rotation);
            Rot45Coords Yr = Y.Rotate(rotation);
            Rot45Coords otherPoint = c + Xr + Yr;
            double midx = (c.Cx + otherPoint.Cx) * 0.5;
            double midy = (c.Cy + otherPoint.Cy) * 0.5;
            if (visitedMids.Contains((midx, midy, level)))
                return;
            visitedMids.Add((midx, midy, level));
            if (level == 0)
            {
                sb.AppendLine(new Shape() { PartID = $"Square_{rotation % 90}", SubModel = true }.Print(midx, midy, 0, 16));
                return;
            }
            c += c + c.Rotate(45) + c.Rotate(360 - 45);
            level--;
            Rot45Coords center = c + Xr + Yr + Vr;
            for (int i = 0; i < 8; i++)
            {
                int subrotation = (rotation + i * 45) % 360;
                GenerateRhomb(sb,center, subrotation,level);
                GenerateSquare(sb,center+V.Rotate(subrotation), subrotation,level);   
            }
        }

        public static void GenerateRhomb(StringBuilder sb, Rot45Coords c, int rotation, int level)
        {
            //Rot45Coords Xr = X.Rotate(rotation);
            //Rot45Coords Ur = U.Rotate(rotation);
            Rot45Coords Vr = V.Rotate(rotation);
            Rot45Coords Yr = Y.Rotate(rotation);
            Rot45Coords otherPoint = c + Vr + Yr;
            double midx = (c.Cx + otherPoint.Cx) * 0.5;
            double midy = (c.Cy + otherPoint.Cy) * 0.5;
            if (visitedMids.Contains((midx, midy, level)))
                return;
            if (level == 0)
            {
                sb.AppendLine(new Shape() { PartID = $"Rhomb_{rotation % 90}", SubModel = true }.Rotate((rotation / 90) * 90).Print(c.Cx, c.Cy, 0, 16));
                return;
            }
            c += c + c.Rotate(45) + c.Rotate(360 - 45);
            otherPoint += otherPoint + otherPoint.Rotate(45) + otherPoint.Rotate(360 - 45);
            level--;
            GenerateRhomb(sb, c, rotation, level);
            GenerateRhomb(sb, otherPoint, (180 + rotation) % 360, level);
            GenerateOctagon(sb, c + Vr + Yr, rotation, level);
            GenerateSquare(sb, c + Vr, rotation, level);
            GenerateSquare(sb, otherPoint - Vr, (rotation + 180) % 360, level);
            GenerateSquare(sb, c + Yr, (rotation + 45) % 360, level);
            GenerateSquare(sb, otherPoint - Yr, (rotation + 180 + 45) % 360, level);
        }

        private static Rot45Coords X = new() { x = 1 };
        private static Rot45Coords U = new() { u = 1 };
        private static Rot45Coords V = new() { v = 1 };
        private static Rot45Coords Y = new() { y = 1 };
        public struct Rot45Coords
        {
            public int x; //0 degrees
            public int u; //-45 degrees
            public int v;//45 degrees
            public int y; //90 degrees
            public readonly int Cx => 2 * (u + v) + 3 * x;
            public readonly int Cy => 2 * (v - u) + 3 * y;

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
