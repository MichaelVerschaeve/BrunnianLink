using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BrunnianLink
{
    public static class ShieldFlatTiling
    {
        static int azureId = ColorMap.Get("Medium_Azure").id;
        public static void Generate(StringBuilder sb, int level)
        {
            MetaData.StartSubModel(sb, $"ShieldTiling{level}");

            GenerateShield(sb, new Rot30Coords(), 0, false, level);
            MetaData.StartSubModel(sb, $"Shield");
            Plate p = new Plate(2, 2);
            sb.AppendLine(p.Print(1, -3, 0, azureId));
            sb.AppendLine(p.Print(1, -5, 0, azureId));
            sb.AppendLine(p.Print(-1, -5, 0, azureId));
            p = new Plate(4, 4);
            sb.AppendLine(p.Print(-2, -2, 0, azureId));
            Shape w = new Shape() { PartID = "65426" };
            sb.AppendLine(w.Print(-3,-6, 0, azureId));
            sb.AppendLine(w.Rotate(90).Print(0,-7, 0, azureId));
            w = new Shape() { PartID = "65429" };
            sb.AppendLine(w.Print(3, -4, 0, azureId));
            sb.AppendLine(w.Rotate(90).Print(2,-1, 0, azureId));
        }

        private static Rot30Coords X = new Rot30Coords() { x = 1 };
        private static Rot30Coords U = new Rot30Coords() { u = 1 };
        private static Rot30Coords V = new Rot30Coords() { v = 1 };
        private static Rot30Coords Y = new Rot30Coords() { y = 1 };

        public static void GenerateShield(StringBuilder sb, Rot30Coords c, int rotation, bool flip, int level)
        {
            Rot30Coords dc = new Rot30Coords();
            int drot = 0;
            if (level == 0)
            {
                switch (rotation % 90)
                {
                    case 0: //square in top right corner
                        break;
                    case 30:
                        dc = X - Y;
                        drot = 270;
                        break;
                    case 60:
                        dc = U - V - Y;
                        drot = 180;
                        break;
                }
                int cutrot = (rotation / 90) * 90;
                c += dc.Rotate(cutrot);
                rotation = drot + cutrot;
                if (rotation >= 360) rotation -= 360;
                sb.AppendLine(new Shape() { PartID = "Shield", SubModel = true }.Rotate(rotation).Print(c.cx, c.cy, rotation, 16));
                return;
            }
            //enlarge, rotate by 15 degrees...
            c += c.Rotate(30);
            level--;
            GenerateTriangle1(sb, c - X - V, 60, false, level); //leftmost
            GenerateTriangle1(sb, c + X + V - Y, 60, true, level); //rightmost
            GenerateTriangle1(sb, c - V, 90, true, level); //top center left
            GenerateTriangle2(sb, c + V - Y, 270, false, level); //top center right
            GenerateSquare(sb, c - Y, 30, true, level); //left square
            GenerateTriangle1(sb, c - Y, 30, true, level); //center triangle
            GenerateSquare(sb, c - Y, 30, false, level); //right square

            GenerateTriangle1(sb, c - U - V, 30, false, level); //left center
            GenerateTriangle1(sb, c + X + U - V - Y, 330, true, level); //right center

            GenerateTriangle2(sb, c -U-2*Y, 210, true, level); //bottom left
            GenerateSquare(sb, c - Y - U, 90, true, level); //bottom squarev
            GenerateTriangle2(sb, c +X- U - 2 * Y, 150, false, level); //bottom left

            GenerateTriangle1(sb, c + X - 2 * U - 2 * Y, 180, false, level); //bottom triangle


        }

        public static void GenerateTriangle1(StringBuilder sb, Rot30Coords c, int rotation, bool flip, int level)
        {


        }

        public static void GenerateTriangle2(StringBuilder sb, Rot30Coords c, int rotation, bool flip, int level)
        {


        }

        public static void GenerateSquare(StringBuilder sb, Rot30Coords c, int rotation, bool flip, int level)
        {


        }

        public struct Rot30Coords
        {
            public int x;
            public int u;
            public int v;
            public int y;
            public readonly int cx => 4 * x + 4 * u + 2 * v;
            public readonly int cy => 2 * u + 4 * v + 4 * y;

            public Rot30Coords Rotate(int rot)
            {
                Rot30Coords res = this;
                for (int r = 0; r < rot; r += 30)
                    (res.x, res.u, res.v, res.y) = (-res.y, res.x, res.u + res.y, res.v);

                return res;
            }
            public static Rot30Coords operator +(Rot30Coords a, Rot30Coords b)
            => new Rot30Coords() { x = a.x + b.x, u = a.u + b.u, v = a.v + b.v, y = a.y + b.y };
            public static Rot30Coords operator -(Rot30Coords a, Rot30Coords b)
            => new Rot30Coords() { x = a.x - b.x, u = a.u - b.u, v = a.v - b.v, y = a.y - b.y };
            public static Rot30Coords operator -(Rot30Coords a)
            => new Rot30Coords() { x = -a.x, u = -a.u, v = -a.v, y = -a.y };
            public static Rot30Coords operator *(int s,Rot30Coords a)
            => new Rot30Coords() { x = s*a.x, u = s*a.u, v = s*a.v, y = s*a.y };

            public Rot30Coords(int x, int u, int v, int y)
            {
                this.x = x; this.u = u; this.v = v; this.y = y;
            }
        }



    }
}
