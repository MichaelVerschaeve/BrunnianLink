using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    public static class HalfHexWedges
    {
        private static readonly string[] m_colors = new string[] { "Red", "Lime", "Medium_Azure", "Dark_Blue", "Bright_Light_Orange", "Tan"};

        public static void Generate(StringBuilder sb, int level)
        {
            MetaData.StartSubModel(sb, $"HalfHexFlat_{level}");
            GenerateRec(sb, new Rot60Coords(), 0, level);

            Shape wedgeLeft = new() { PartID = "65426" };  //   \|
            Shape wedgeRight = new() { PartID = "65429" }; //   |/
            Shape swLeft = new() { PartID = "24307" };  //   \|
            Shape swRight = new() { PartID = "24299" }; //   |/
            List<int> colorIds = m_colors.Select(c => ColorMap.Get(c).id).ToList();
            MetaData.StartSubModel(sb, "Trap_0");
            sb.AppendLine(swLeft.Rotate(180).Print(1, 1, 0, colorIds[0]));
            sb.AppendLine(swRight.Rotate(180).Print(-1, 1, 0, colorIds[0]));
            MetaData.StartSubModel(sb, "Trap_60");
            sb.AppendLine(wedgeRight.Print(0, 0, 0, colorIds[1]));
            sb.AppendLine(swRight.Rotate(180).Print(-1, 1, -1, colorIds[1]));
            sb.AppendLine(swLeft.Print(-1, -1, -1, colorIds[1]));
            MetaData.StartSubModel(sb, "Trap_120");
            sb.AppendLine(wedgeLeft.Rotate(180).Print(0, 0, 0, colorIds[2]));
            sb.AppendLine(swRight.Rotate(180).Print(-1, 1, -1, colorIds[2]));
            sb.AppendLine(swLeft.Print(-1, -1, -1, colorIds[2]));
            MetaData.StartSubModel(sb, "Trap_180");
            sb.AppendLine(swLeft.Print(-1, -1, 0, colorIds[3]));
            sb.AppendLine(swRight.Print(1, -1, 0, colorIds[3]));
            MetaData.StartSubModel(sb, "Trap_240");
            sb.AppendLine(wedgeRight.Rotate(180).Print(0, 0, 0, colorIds[4]));
            sb.AppendLine(swRight.Print(1, -1, -1, colorIds[4]));
            sb.AppendLine(swLeft.Rotate(180).Print(1, 1, -1, colorIds[4]));
            MetaData.StartSubModel(sb, "Trap_300");
            sb.AppendLine(wedgeLeft.Print(0, 0, 0, colorIds[5]));
            sb.AppendLine(swRight.Print(1, -1, -1, colorIds[5]));
            sb.AppendLine(swLeft.Rotate(180).Print(1, 1, -1, colorIds[5]));
        }

        public static void GenerateRec(StringBuilder sb, Rot60Coords c, int rotation, int level)
        {
            Rot60Coords Xr = new Rot60Coords() { x = 1 }.Rotate(rotation);
            Rot60Coords Vr = Xr.Rotate(60);
            if (level == 0)
            {
                sb.AppendLine(new Shape() { PartID = $"Trap_{rotation}", SubModel = true }.Print(c.Cx, c.Cy, 0, 16));
                return;
            }
            c *= 2;
            level--;
            GenerateRec(sb, c, rotation, level);
            GenerateRec(sb, c + 2 * Vr - Xr, (rotation + 180) % 360, level);
            GenerateRec(sb, c + Xr + Vr, (rotation + 120) % 360, level);
            GenerateRec(sb, c - 2 * Xr + Vr, (rotation + 240) % 360, level);
        }



        //rotation = 0 -> to the right 
        public struct Rot60Coords
        {
            public int x;
            public int v;
            public readonly int Cx => 2 * x + v;
            public readonly int Cy => 2 * v;

            public readonly Rot60Coords Rotate(int rot)
            {
                Rot60Coords res = this;
                for (int r = 0; r < rot; r += 60)
                    (res.x, res.v) = (-res.v, res.x + res.v);

                return res;
            }
            public static Rot60Coords operator +(Rot60Coords a, Rot60Coords b)
            => new() { x = a.x + b.x, v = a.v + b.v };
            public static Rot60Coords operator -(Rot60Coords a, Rot60Coords b)
            => new() { x = a.x - b.x, v = a.v - b.v };
            public static Rot60Coords operator -(Rot60Coords a)
            => new() { x = -a.x, v = -a.v };
            public static Rot60Coords operator *(int s, Rot60Coords a)
            => new() { x = s * a.x, v = s * a.v };
            public static Rot60Coords operator *(Rot60Coords a, int s)
            => new() { x = s * a.x, v = s * a.v };
        }
    }
}