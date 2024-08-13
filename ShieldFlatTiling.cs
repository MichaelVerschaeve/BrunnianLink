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
        static int whiteId = ColorMap.Get("White").id;
        static int limeId = ColorMap.Get("Lime").id;
        public static void Generate(StringBuilder sb, int level)
        {
            MetaData.StartSubModel(sb, $"ShieldTiling{level}");

            //replace by other call (GenerateSquare, GenerateTriangle1,GenerateTriangle2) for dfferent results
            GenerateSquare(sb, new Rot30Coords(), 0, false, level);

            //shield subpart
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
            //square sub part. rotated 30
            MetaData.StartSubModel(sb, "Square30");
            w = new Shape() { PartID = "65426" };
            sb.AppendLine(w.Print(1, -2, 0, whiteId));
            sb.AppendLine(w.Rotate(90).Print(4, -3, 0, whiteId));
            sb.AppendLine(w.Rotate(180).Print(5, 0, 0, whiteId));
            sb.AppendLine(w.Rotate(270).Print(2, 1, 0, whiteId));
            p = new Plate(2, 2);
            sb.AppendLine(p.Print(3, -1, 0, whiteId));
            //square sub part. rotated 60
            MetaData.StartSubModel(sb, "Square60");
            w = new Shape() { PartID = "65429" };
            sb.AppendLine(w.Print(5, 0, 0, whiteId));
            sb.AppendLine(w.Rotate(90).Print(4, 3, 0, whiteId));
            sb.AppendLine(w.Rotate(180).Print(1, 2, 0, whiteId));
            sb.AppendLine(w.Rotate(270).Print(2, -1, 0, whiteId));
            sb.AppendLine(p.Print(3, 1, 0, whiteId));
            //triangle subpart
            MetaData.StartSubModel(sb, "Triangle");
            w = new Shape() { PartID = "65426" };
            sb.AppendLine(w.Rotate(180).Print(1, -2, 0, limeId));
            w = new Shape() { PartID = "65429" };
            sb.AppendLine(w.Rotate(180).Print(-1, -2, 0, limeId));
        }

        private static Rot30Coords X = new Rot30Coords() { x = 1 };
        private static Rot30Coords U = new Rot30Coords() { u = 1 };
        private static Rot30Coords V = new Rot30Coords() { v = 1 };
        private static Rot30Coords Y = new Rot30Coords() { y = 1 };


        public static void GenerateShield(StringBuilder sb, Rot30Coords c, int rotation, bool flip, int level)
        {
            if (level == 0)
            {
                Rot30Coords dc = new Rot30Coords();
                int drot = 0;
                switch (rotation % 90)
                {
                    case 0: //square in top left corner
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
                sb.AppendLine(new Shape() { PartID = "Shield", SubModel = true }.Rotate(rotation).Print(c.cx, c.cy, 0, 16));
                return;
            }
            //enlarge, rotate by 15 degrees...
            c += c.Rotate(30);
            level--;
            Rot30Coords Xr = X.Rotate(rotation);
            Rot30Coords Ur = U.Rotate(rotation);
            Rot30Coords Vr = V.Rotate(rotation);
            Rot30Coords Yr = Y.Rotate(rotation);
            if (flip)
                (Xr, Ur, Yr) = (-Xr - Vr, -Ur, Yr + Ur);

            var addR = (int r) => ((flip?(540-r):r) + rotation) % 360;

            GenerateTriangle1(sb, c - Ur, addR(300), flip, level); //leftmost
            GenerateTriangle1(sb, c + Ur - Yr, addR(60), !flip, level); //rightmost
            GenerateTriangle1(sb, c - Ur, addR(90), !flip, level); //top center left
            GenerateTriangle2(sb, c + Ur - Yr, addR(270), flip, level); //top center right

            GenerateSquare(sb, c - Yr - Vr, addR(240), !flip, level); //left square
            GenerateTriangle1(sb, c - Yr, rotation, !flip, level); //center triangle
            GenerateSquare(sb, c - Yr, addR(30), flip, level); //right sqUrare


            GenerateTriangle2(sb, c - Vr - 2 * Yr, addR(210), !flip, level); //bottom left
            GenerateSquare(sb, c - Yr - Vr, addR(90), !flip, level); //bottom sqUrare
            GenerateTriangle2(sb, c + Xr - Vr - 2 * Yr, addR(150), flip, level); //bottom left

            GenerateTriangle1(sb, c - Vr - 2 * Yr, addR(60), flip, level); //bottom triangle
        }

        public static void GenerateTriangle1(StringBuilder sb, Rot30Coords c, int rotation, bool flip, int level)
        {
            if (level == 0)
            {
                Rot30Coords dc = new Rot30Coords();
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
                sb.AppendLine(new Shape() { PartID = "Triangle", SubModel = true }.Rotate(rotation).Print(c.cx, c.cy, 0, 16));
                return;
            }
            //enlarge, rotate by 15 degrees...
            c += c.Rotate(30);
            level--;
            Rot30Coords Xr = X.Rotate(rotation);
            Rot30Coords Ur = U.Rotate(rotation);
            Rot30Coords Vr = V.Rotate(rotation);
            Rot30Coords Yr = Y.Rotate(rotation);
            if (flip)
            {
                (Xr, Ur, Yr) = (-Xr - Vr, -Ur, Yr + Ur);
                (Xr, Ur, Vr, Yr) = (Xr.Rotate(30),Ur.Rotate(30),Vr.Rotate(30),Yr.Rotate(30)); //symmetry axis has tilted also
            }
            var addR = (int r) => ((flip ? (570 - r) : r) + rotation) % 360;
            GenerateShield(sb, c + Ur-Yr, addR(330), flip, level);
        }

        public static void GenerateTriangle2(StringBuilder sb, Rot30Coords c, int rotation, bool flip, int level)
        {
            if (level == 0) //same as triangle 1
            {
                GenerateTriangle1(sb,c,rotation, flip, 0);
                return;
            }
            //enlarge, rotate by 15 degrees...
            c += c.Rotate(30);
            level--;
            Rot30Coords Xr = X.Rotate(rotation);
            Rot30Coords Ur = U.Rotate(rotation);
            Rot30Coords Vr = V.Rotate(rotation);
            Rot30Coords Yr = Y.Rotate(rotation);
            if (flip)
            {
                (Xr, Ur, Yr) = (-Xr - Vr, -Ur, Yr + Ur);
                (Xr, Ur, Vr, Yr) = (Xr.Rotate(30), Ur.Rotate(30), Vr.Rotate(30), Yr.Rotate(30)); //symmetry axis has tilted also
            }
            var addR = (int r) => ((flip ? (570 - r) : r) + rotation) % 360;
            GenerateSquare(sb, c + Ur - Yr, addR(240), flip, level);
            GenerateTriangle2(sb, c + Ur - Vr -Yr, addR(120), flip, level);
            GenerateTriangle2(sb, c + Ur - Vr - Yr, addR(270), flip, level);
        }

        public static void GenerateSquare(StringBuilder sb, Rot30Coords c, int rotation, bool flip, int level)
        {
            if (level == 0)
            {
                if (flip)
                    c -= X.Rotate(rotation);
                int cutrot = (rotation / 90) * 90;
                switch (rotation % 90)
                {
                    case 0:
                        int dx = (rotation <= 90) ? 2 : -2;
                        int dy = (rotation == 180 || rotation == 90) ? 2 : -2;
                        sb.AppendLine(new Plate(4,4).Print(c.cx+dx,c.cy+dy,0,whiteId));
                        break;
                    case 30:
                        sb.AppendLine(new Shape() { PartID="Square30",SubModel=true}.Rotate(cutrot).Print(c.cx, c.cy, 0, 16));
                        break;
                    case 60:
                        sb.AppendLine(new Shape() { PartID = "Square60", SubModel = true }.Rotate(cutrot).Print(c.cx, c.cy, 0, 16));
                        break;
                }
                return;
            }
            //enlarge, rotate by 15 degrees...
            c += c.Rotate(30);
            level--;
            Rot30Coords Xr = X.Rotate(rotation);
            Rot30Coords Ur = U.Rotate(rotation);
            Rot30Coords Vr = V.Rotate(rotation);
            Rot30Coords Yr = Y.Rotate(rotation);
            if (flip)
            {
                (Xr, Ur, Yr) = (-Xr - Vr, -Ur, Yr + Ur);
                (Xr, Ur, Vr, Yr) = (Xr.Rotate(30), Ur.Rotate(30), Vr.Rotate(30), Yr.Rotate(30)); //symmetry axis has tilted also
            }
            var addR = (int r) => ((flip ? (570 - r) : r) + rotation) % 360;

            GenerateSquare(sb, c + Xr, addR(60), !flip, level);
            GenerateTriangle1(sb, c - Vr + Xr, addR(30), flip, level);
            GenerateTriangle1(sb, c - Vr + Xr, addR(180), !flip, level);
            GenerateTriangle1(sb, c + Xr + Ur - Vr - Yr, addR(120), flip, level);
            GenerateTriangle1(sb, c + Xr, addR(90), !flip, level);
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
