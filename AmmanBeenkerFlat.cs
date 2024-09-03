using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    public static class AmmanBeenkerFlat
    {

        static readonly int triangleId = ColorMap.Get("Orange").id;
        static readonly int triangleMirror2Id = ColorMap.Get("Yellow").id;
        static readonly int rhombId = ColorMap.Get("Dark_Blue").id;


        public static void Generate(StringBuilder sb, int level)
        {
            MetaData.StartSubModel(sb, $"AmmanBeenker_{level}");
            //for (int rot = 0; rot < 360; rot+=45)
            //GenerateRhomb(sb, new Rot45Coords(), rot, false,level);
            GenerateTriangle(sb, new Rot45Coords(), 0, false, level);
            GenerateTriangle(sb, U-V, 0, true, level);
            Shape wCorner = new() { PartID = "35787" };// |\
            MetaData.StartSubModel(sb, "Triangle_0");
            sb.AppendLine(wCorner.Print(1, -1, 0, triangleId));
            sb.AppendLine(wCorner.Rotate(90).Print(-1, -1, 0, triangleId));
            MetaData.StartSubModel(sb, "Triangle_0_M");
            sb.AppendLine(wCorner.Rotate(270).Print(1, 1, 0, triangleMirror2Id));
            sb.AppendLine(wCorner.Rotate(180).Print(-1, 1, 0, triangleMirror2Id));
            //Shape wLeft = new() { PartID = "5091" }; //[ \
            Shape wRight = new() { PartID = "5092" }; //[ /
            MetaData.StartSubModel(sb, "Triangle_45");
            sb.AppendLine(wRight.Rotate(90).Print(2,-0.5,0, triangleId));
            sb.AppendLine(new Plate(1,1).Print(0.5, -0.5, 0, triangleId));
            sb.AppendLine(wCorner.Rotate(270).Print(1, -2, 0, triangleId));
            MetaData.StartSubModel(sb, "Triangle_45_M");
            sb.AppendLine(wRight.Rotate(270).Print(-2, 0.5, 0, triangleMirror2Id));
            sb.AppendLine(new Plate(1, 1).Print(-0.5, 0.5, 0, triangleMirror2Id));
            sb.AppendLine(wCorner.Rotate(90).Print(-1, 2, 0, triangleMirror2Id));
            MetaData.StartSubModel(sb, "Rhomb_0");
            sb.AppendLine(wCorner.Rotate(180).Print(1, -1, 0, rhombId));
            sb.AppendLine(wCorner.Print(4, -1, 0, rhombId));
            sb.AppendLine(new Plate(1,2).Print(2.5, -1, 0, rhombId));
            MetaData.StartSubModel(sb, "Rhomb_45");
            sb.AppendLine(wCorner.Rotate(90).Print(1, 1, 0, rhombId));
            sb.AppendLine(wCorner.Rotate(270).Print(4, 1, 0, rhombId));
            sb.AppendLine(new Plate(1, 2).Print(2.5, 1, 0, rhombId));
        }


        //top is origin, mirror is around X-axis
        public static void GenerateTriangle(StringBuilder sb, Rot45Coords c, int rotation, bool mirror, int level)
        {
            if (level == 0)
            {
                sb.AppendLine(new Shape() { PartID = $"Triangle_{rotation%90}{(mirror?"_M":"")}", SubModel = true }.Rotate((rotation / 90) * 90).Print(c.Cx, c.Cy, 0, 16));
                return;
            }
            level--;
            c += c.Rotate(45) + c.Rotate(360 - 45);
            Rot45Coords Xr = X.Rotate(rotation);
            Rot45Coords Ur = U.Rotate(rotation);
            Rot45Coords Vr = V.Rotate(rotation);
            Rot45Coords Yr = Y.Rotate(rotation);
            if (mirror) 
                (_, Vr, Yr) = (Vr, Ur, -Yr);
            int addR(int r) => ((mirror ? (360 - r) : r) + rotation) % 360;
            GenerateRhomb(sb,c, addR(270),mirror,level);
            GenerateRhomb(sb, c-Yr, rotation, mirror, level);
            GenerateTriangle(sb, c - Yr, addR(180), !mirror, level);
            GenerateTriangle(sb, c - Yr, addR(135), mirror, level);
            GenerateTriangle(sb, c - Yr-Vr, addR(225), mirror, level);
        }

        public static void GenerateRhomb(StringBuilder sb, Rot45Coords c, int rotation, bool mirror, int level)
        {
            if (mirror)
                rotation += 45;
            if (level == 0)
            {
                sb.AppendLine(new Shape() { PartID = $"Rhomb_{rotation%90}", SubModel = true }.Rotate((rotation / 90) * 90).Print(c.Cx, c.Cy, 0, 16));
                return;
            }
            level--;
            c += c.Rotate(45) + c.Rotate(360 - 45);
            Rot45Coords Xr = X.Rotate(rotation);
            Rot45Coords Ur = U.Rotate(rotation);
            Rot45Coords Vr = V.Rotate(rotation);
            Rot45Coords Yr = Y.Rotate(rotation);
            GenerateRhomb(sb, c, rotation, false, level);
            GenerateRhomb(sb, c+Xr+Ur-Yr, (rotation+90)%360, false, level);
            GenerateRhomb(sb, c + Xr + Ur - Yr+Vr, rotation, false, level);
            GenerateTriangle(sb, c+Xr+Ur, (rotation+360-45)%360, false, level);
            GenerateTriangle(sb, c + Xr + Ur, rotation, true, level);
            GenerateTriangle(sb, c + Xr + Ur-Yr+Vr, (rotation + 135) % 360, false, level);
            GenerateTriangle(sb, c + Xr + Ur-Yr+Vr, (rotation+180)%360, true, level);
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
