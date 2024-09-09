using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    public static class PenroseFlat
    {

        //static readonly int[] colorIds = new[]
        //{   ColorMap.Get("Dark_Blue").id,
        //    ColorMap.Get("Medium_Azure").id,
        //    ColorMap.Get("Red").id,
        //    ColorMap.Get("Bright_Light_Orange").id,
        //    ColorMap.Get("Tan").id,
        //    ColorMap.Get("Dark_Blue").id,
        //    ColorMap.Get("Medium_Azure").id,
        //    ColorMap.Get("Red").id,
        //    ColorMap.Get("Bright_Light_Orange").id,
        //    ColorMap.Get("Tan").id
        //};

        static readonly int[] colorIds = Enumerable.Repeat(ColorMap.Get("Yellow").id,5).Concat(Enumerable.Repeat(ColorMap.Get("Red").id, 5)).ToArray();
        public static void Generate(StringBuilder sb, int level)
        {
            visitedMids.Clear();
            MetaData.StartSubModel(sb, $"PenroseWedges_{level}");
            for (int rot = 0; rot < 360; rot+=72)
                GenerateThick(sb, new Rot36Coords(), rot, level);

            Shape wedgeLeft = new() { PartID = "65426" };  //   \|
            Shape wedgeRight = new() { PartID = "65429" }; //   |/
            MetaData.StartSubModel(sb, $"ThickRhomb_0");
            sb.AppendLine(wedgeRight.Print(1, 2, 0, colorIds[0]));
            sb.AppendLine(wedgeLeft.Print(-1, 2, 0, colorIds[0]));
            sb.AppendLine(wedgeRight.Rotate(180).Print(-1, 6, 0, colorIds[0]));
            sb.AppendLine(wedgeLeft.Rotate(180).Print(1, 6, 0, colorIds[0]));
            MetaData.StartSubModel(sb, $"ThickRhomb_72");
            sb.AppendLine(wedgeLeft.Print(-5, 0, 0, colorIds[1]));
            sb.AppendLine(wedgeLeft.Rotate(90).Print(-2, -1, 0, colorIds[1]));
            sb.AppendLine(wedgeLeft.Rotate(180).Print(-1, 2, 0, colorIds[1]));
            sb.AppendLine(wedgeLeft.Rotate(270).Print(-4, 3, 0, colorIds[1]));
            sb.AppendLine(new Plate(2, 2).Print(-3, 1, 0, colorIds[1]));
            MetaData.StartSubModel(sb, $"ThickRhomb_144");
            sb.AppendLine(wedgeLeft.Rotate(270).Print(-2, -1, 0, colorIds[2]));
            sb.AppendLine(new Plate(4, 2).Print(-2, -3, 0, colorIds[2]));
            sb.AppendLine(wedgeLeft.Rotate(90).Print(-2, -5, 0, colorIds[2]));
            MetaData.StartSubModel(sb, $"ThickRhomb_216");
            sb.AppendLine(wedgeRight.Rotate(90).Print(2, -1, 0, colorIds[3]));
            sb.AppendLine(new Plate(4, 2).Print(2, -3, 0, colorIds[3]));
            sb.AppendLine(wedgeRight.Rotate(270).Print(2, -5, 0, colorIds[3]));
            MetaData.StartSubModel(sb, $"ThickRhomb_288");
            sb.AppendLine(wedgeRight.Print(5, 0, 0, colorIds[4]));
            sb.AppendLine(wedgeRight.Rotate(90).Print(4, 3, 0, colorIds[4]));
            sb.AppendLine(wedgeRight.Rotate(180).Print(1, 2, 0, colorIds[4]));
            sb.AppendLine(wedgeRight.Rotate(270).Print(2, -1, 0, colorIds[4]));
            sb.AppendLine(new Plate(2, 2).Print(3, 1, 0, colorIds[4]));
            MetaData.StartSubModel(sb, $"ThinRhomb_0");
            sb.AppendLine(wedgeRight.Rotate(90).Print(2, 3, 0, colorIds[5]));
            sb.AppendLine(wedgeLeft.Rotate(270).Print(-2, 3, 0, colorIds[5]));
            sb.AppendLine(wedgeRight.Rotate(270).Print(-2, 1, 0, colorIds[5]));
            sb.AppendLine(wedgeLeft.Rotate(90).Print(2, 1, 0, colorIds[5]));
            MetaData.StartSubModel(sb, $"ThinRhomb_72");
            sb.AppendLine(wedgeRight.Print(-1, -2, 0, colorIds[6]));
            sb.AppendLine(wedgeRight.Rotate(180).Print(-1, 2, 0, colorIds[6]));
            MetaData.StartSubModel(sb, $"ThinRhomb_144");
            sb.AppendLine(wedgeRight.Rotate(90).Print(-2, 1, -1, colorIds[7]));
            sb.AppendLine(wedgeLeft.Rotate(180).Print(1, -2, -1, colorIds[7]));
            sb.AppendLine(new Plate(3, 3).Print(-1.5, -1.5, -1, colorIds[7]));
            MetaData.StartSubModel(sb, $"ThinRhomb_216");
            sb.AppendLine(wedgeLeft.Rotate(270).Print(2, 1, -1, colorIds[8]));
            sb.AppendLine(wedgeRight.Rotate(180).Print(-1, -2, -1, colorIds[8]));
            sb.AppendLine(new Plate(3, 3).Print(1.5, -1.5, -1, colorIds[8]));
            MetaData.StartSubModel(sb, $"ThinRhomb_288");
            sb.AppendLine(wedgeLeft.Print(1, -2, 0, colorIds[9]));
            sb.AppendLine(wedgeLeft.Rotate(180).Print(1, 2, 0, colorIds[9]));
        }

        //long side vertical aligned, bottom is origin
        private static void GenerateThick(StringBuilder sb, Rot36Coords c, int rotation, int level)
        {
            Rot36Coords otherPoint = c + T[0].Rotate(rotation) + T[1].Rotate(rotation);
            double midx = (c.Cx + otherPoint.Cx) * 0.5;
            double midy = (c.Cy + otherPoint.Cy) * 0.5;
            if (visitedMids.Contains((midx, midy, level)))
                return;
            visitedMids.Add((midx, midy, level));

            if (level == 0)
            {
                if (rotation % 72 ==0)
                    sb.AppendLine(new Shape() { PartID = $"ThickRhomb_{rotation}", SubModel = true }.Print(c.Cx, c.Cy, 0, 16));
                else
                    sb.AppendLine(new Shape() { PartID = $"ThickRhomb_{(rotation+180)%360}", SubModel = true }.Rotate(180).Print(c.Cx, c.Cy, 0, 16));
                return;
            }
            level--;
            c = c.Rotate(36)+c.Rotate(360-36);
            Rot36Coords[] Tr = T.Select(t=>t.Rotate(rotation)).ToArray();
            GenerateThick(sb,c + Tr[0] + Tr[1],(rotation+180+36)%360,level); //left point
            GenerateThin(sb, c + Tr[0] + Tr[1], (rotation +360- 36) % 360, level); //left point
            GenerateThick(sb, c + Tr[0] + Tr[4], (rotation + 180 - 36) % 360, level); //right point
            GenerateThin(sb, c + Tr[0] + Tr[4], (rotation + 36) % 360, level); //right point
            GenerateThick(sb, c + Tr[0] - Tr[3] - Tr[2], (rotation + 180) % 360, level); //top
        }


        //long side vertical aligned, bottom is origin
        private static void GenerateThin(StringBuilder sb, Rot36Coords c, int rotation, int level)
        {
            Rot36Coords otherPoint = c + T[0].Rotate(rotation) + T[1].Rotate(rotation);
            double midx = (c.Cx + otherPoint.Cx) * 0.5;
            double midy = (c.Cy + otherPoint.Cy) * 0.5;
            if (visitedMids.Contains((midx, midy, level)))
                return;
            visitedMids.Add((midx, midy, level));

            if (level == 0)
            {
                if (rotation % 72 == 0)
                    sb.AppendLine(new Shape() { PartID = $"ThinRhomb_{rotation}", SubModel = true }.Print(c.Cx, c.Cy, 0, 16));
                else
                    sb.AppendLine(new Shape() { PartID = $"ThinRhomb_{(rotation + 180) % 360}", SubModel = true }.Rotate(180).Print(c.Cx, c.Cy, 0, 16));
                return;
            }
            level--;
            c = c.Rotate(36) + c.Rotate(360-36);
            Rot36Coords[] Tr = T.Select(t => t.Rotate(rotation)).ToArray();
            GenerateThin(sb, c + Tr[0], (rotation + 90 + 18) % 360, level);
            GenerateThin(sb, c + Tr[0], (rotation + 270 - 18) % 360, level);
            GenerateThick(sb, c - Tr[3] - Tr[4], (rotation + 270 - 18) % 360, level);
            GenerateThick(sb, c - Tr[1] - Tr[2], (rotation + 90 + 18) % 360, level);
        }

        private static Rot36Coords[] T = Enumerable.Range(0,5).Select(i=>Rot36Coords.UnitVector(i)).ToArray();


        private static readonly HashSet<(double x, double y, int level)> visitedMids = new();


        //rotation = 0 -> positive Y-axis
        public struct Rot36Coords
        {
            public readonly int[] t;
            public readonly int Cx => 4 * (t[4] - t[1]) + 2 * (t[3] - t[2]);
            public readonly int Cy => 4 * (t[0] - t[2] - t[3]) + 2 * (t[1] + t[4]);

            public Rot36Coords()
            {
                t =  new int[] { 0, 0, 0, 0, 0 };
            }


            public readonly Rot36Coords Rotate(int rot)
            {
                Rot36Coords res = new();
                int times = rot / 36;
                int sign = (times & 1) == 0 ? 1 : -1;
                if (res.t != null)
                    for (int i = 0; i < 5; i++)
                        res.t[i] = sign * t[(i + 2 * times) % 5];
                return res;
            }

            public static Rot36Coords operator +(Rot36Coords a, Rot36Coords b)
            {
                Rot36Coords res = new();
                if (res.t != null)
                    for (int i = 0; i < 5; i++)
                        res.t[i] = a.t[i] + b.t[i];
                return res;
            }

            public static Rot36Coords operator -(Rot36Coords a, Rot36Coords b)
            {
                Rot36Coords res = new();
                if (res.t != null)
                    for (int i = 0; i < 5; i++)
                        res.t[i] = a.t[i] - b.t[i];
                return res;
            }
            public static Rot36Coords operator -(Rot36Coords a)
            {
                Rot36Coords res = new();
                if (res.t != null)
                    for (int i = 0; i < 5; i++)
                        res.t[i] = -a.t[i];
                return res;
            }
            public static Rot36Coords operator *(int s, Rot36Coords a)
            {
                Rot36Coords res = new Rot36Coords();
                if (res.t != null)
                    for (int i = 0; i < 5; i++)
                        res.t[i] = s * a.t[i];
                return res;
            }

            public static Rot36Coords operator *( Rot36Coords a, int s)
            {
                Rot36Coords res = new Rot36Coords();
                if (res.t != null)
                    for (int i = 0; i < 5; i++)
                        res.t[i] = s * a.t[i];
                return res;
            }

            public Rot36Coords(int[] t)
            {
                this.t = t;
            }

            public Rot36Coords(Rot36Coords o)
            {
                this.t = (int[])o.t.Clone();
            }

            public static Rot36Coords UnitVector(int i)
            {
                Rot36Coords res = new Rot36Coords();
                if (res.t != null)
                    res.t[i] = 1;
                return res;
            }
        }
    }
}
