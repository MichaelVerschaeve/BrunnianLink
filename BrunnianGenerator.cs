using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    public static class BrunnianGenerator
    {
        private static readonly string[] mainColors = new string[] { "Red", "Orange", "Yellow", "Bright_Green", "Medium_Azure" };
        private static readonly string[] sideColors = new string[] { "White", "Tan", "Dark_Bluish_Grey", "Reddish_Brown", "Black" };

        public static void Generate(StringBuilder sb)
        {
            sb.Append("0 Brunnian Linkage\r\n0 Name:  BrunnianLinkage\r\n0 Author:  Michael Verschaeve\r\n0 CustomBrick\r\n");
            //Shape bp = new BasePlate(48);
            //sb.AppendLine(bp.Print(0, 0, 0, colorMap["Light_Bluish_Grey"].id));

            foreach (var (s, x, y) in Base(0))
                sb.AppendLine(s.Print(x, y, 1, ColorMap.Get("Light_Bluish_Grey").id));

            sb.AppendLine("0 STEP");

            foreach (var (s, x, y) in Base(1))
                sb.AppendLine(s.Print(x, y, 2, ColorMap.Get("Light_Bluish_Grey").id));

            sb.AppendLine("0 STEP");

            foreach (var (s, c, x, y) in EntreLacs())
            {
                sb.AppendLine(s.Print(x, y, 3, ColorMap.Get(c).id));
            }

        }

        private static IEnumerable<(Shape s, string c, double x, double y)> DoCorner(int index, int ix, int iy, int orientation)
        {
            string[] colorList = new string[] { sideColors[index], mainColors[index], sideColors[index] };
            for (int s = 2; s <= 4; s++)
            {
                Shape t = new Bow(s);
                t.RotateThis(orientation * 90.0);
                int signX = orientation < 2 ? 1 : -1;
                int signY = (orientation % 3) == 0 ? -1 : 1;
                double x = 5 * ix + signX * s * 0.5;
                double y = 5 * iy + signY * s * 0.5;
                yield return (t, colorList[s - 2], x, y);
            }
        }

        private static IEnumerable<(Shape s, string c, double x, double y)> DoLine(int index, int from, int to, double center, bool horz)
        {
            int length = to - from;
            if (length == 0) yield break;
            if (length < 0)
            {
                (from, to) = (to, from);
                length = -length;
            }
            if (index == 3 && length > 4) //no tiles larger than 4 for bright green
            {
                foreach (var y in DoLine(index, from, from + 4, center, horz).Concat(DoLine(index, from + 4, to, center, horz)))
                    yield return y;
                yield break;
            }
            if (length > 8) //no tiles larger than 8
            {
                foreach (var y in DoLine(index, from, from + 8, center, horz).Concat(DoLine(index, from + 8, to, center, horz)))
                    yield return y;
                yield break;
            }
            if (length == 5 || length == 7) //no tiles of length 5 or 7
            {
                foreach (var y in DoLine(index, from, from + 3, center, horz).Concat(DoLine(index, from + 3, to, center, horz)))
                    yield return y;
                yield break;
            }
            Shape t = new Tile(length);
            if (horz)
            {
                yield return (t, sideColors[index], (to + from) * 0.5, center - 1);
                yield return (t, mainColors[index], (to + from) * 0.5, center);
                yield return (t, sideColors[index], (to + from) * 0.5, center + 1);
            }
            else
            {
                t.RotateThis();
                yield return (t, sideColors[index], center - 1, (to + from) * 0.5);
                yield return (t, mainColors[index], center, (to + from) * 0.5);
                yield return (t, sideColors[index], center + 1, (to + from) * 0.5);
            }
        }

        private static IEnumerable<(Shape s, string c, double x, double y)> EntreLacs()
        {
            foreach (var t in
                DoCorner(0, 4, 0, 0).Concat(
                DoCorner(0, 4, 0, 1)).Concat(
                DoCorner(0, -4, 0, 2)).Concat(
                DoCorner(0, -4, 0, 3)).Concat(
                DoLine(0, -20, -14, 2.5, true)).Concat(
                DoLine(0, 14, 20, 2.5, true)).Concat(
                DoLine(0, -20, -19, -2.5, true)).Concat(
                DoLine(0, 19, 20, -2.5, true)).Concat(
                DoLine(0, -16, -14, -2.5, true)).Concat(
                DoLine(0, 14, 16, -2.5, true))
                )
                yield return t;
            for (int from = -11; from <= 9; from += 5)
                foreach (var t in DoLine(0, from, from + 2, 2.5, true).Concat(DoLine(0, from, from + 2, -2.5, true)))
                    yield return t;

            foreach (var t in
                DoCorner(1, 3, -1, 0).Concat(
                DoCorner(1, 3, 1, 1)).Concat(
                DoCorner(1, -3, 1, 2)).Concat(
                DoCorner(1, -3, -1, 3)).Concat(
                DoLine(1, -15, -9, 7.5, true)).Concat(
                DoLine(1, -15, -9, -7.5, true)).Concat(
                DoLine(1, 9, 15, 7.5, true)).Concat(
                DoLine(1, 9, 15, -7.5, true)).Concat(
                DoLine(1, 4, 5, -17.5, false)).Concat(
                DoLine(1, 4, 5, 17.5, false)).Concat(
                DoLine(1, -5, 1, -17.5, false)).Concat(
                DoLine(1, -5, 1, 17.5, false))
                )
                yield return t;
            for (int from = -6; from <= 4; from += 5)
                foreach (var t in DoLine(1, from, from + 2, 7.5, true).Concat(DoLine(1, from, from + 2, -7.5, true)))
                    yield return t;

            foreach (var t in
                DoCorner(2, 2, -2, 0).Concat(
                DoCorner(2, 2, 2, 1)).Concat(
                DoCorner(2, -2, 2, 2)).Concat(
                DoCorner(2, -2, -2, 3)).Concat(
                DoLine(2, -10, -4, 12.5, true)).Concat(
                DoLine(2, -1, 1, 12.5, true)).Concat(
                DoLine(2, 4, 10, 12.5, true)).Concat(
                DoLine(2, -10, -4, -12.5, true)).Concat(
                DoLine(2, -1, 1, -12.5, true)).Concat(
                DoLine(2, 4, 10, -12.5, true)).Concat(
                DoLine(2, -10, -9, 12.5, false)).Concat(
                DoLine(2, -6, 6, 12.5, false)).Concat(
                DoLine(2, 9, 10, 12.5, false)).Concat(
                DoLine(2, -10, -9, -12.5, false)).Concat(
                DoLine(2, -6, 6, -12.5, false)).Concat(
                DoLine(2, 9, 10, -12.5, false))
                )
                yield return t;

            foreach (var t in
                DoCorner(3, 1, -3, 0).Concat(
                DoCorner(3, 1, 3, 1)).Concat(
                DoCorner(3, -1, 3, 2)).Concat(
                DoCorner(3, -1, -3, 3)).Concat(
                DoLine(3, -5, 1, 17.5, true)).Concat(
                DoLine(3, 4, 5, 17.5, true)).Concat(
                DoLine(3, -5, 1, -17.5, true)).Concat(
                DoLine(3, 4, 5, -17.5, true)).Concat(
                DoLine(3, -15, -14, 7.5, false)).Concat(
                DoLine(3, -11, 11, 7.5, false)).Concat(
                DoLine(3, 14, 15, 7.5, false)).Concat(
                DoLine(3, -15, -14, -7.5, false)).Concat(
                DoLine(3, -11, 11, -7.5, false)).Concat(
                DoLine(3, 14, 15, -7.5, false))
                )
                yield return t;

            foreach (var t in
                DoCorner(4, 0, -4, 0).Concat(
                DoCorner(4, 0, 4, 1)).Concat(
                DoCorner(4, 0, 4, 2)).Concat(
                DoCorner(4, 0, -4, 3)).Concat(
                DoLine(4, -20, -19, -2.5, false)).Concat(
                DoLine(4, -16, 16, -2.5, false)).Concat(
                DoLine(4, 19, 20, -2.5, false)).Concat(
                DoLine(4, -20, 20, 2.5, false))
                )
                yield return t;
        }

        private  static IEnumerable<(Shape s, double x, double y)> Base(int layer)
        {
            for (int i = 0; i < 4; i++)
            {
                double x = i * 6 + 3;
                double y = (3 - i) * 6 + 3;
                if (layer == 0)
                {
                    if (i == 3) break;
                    x += 3;
                    y -= 3;
                }
                Shape s = new() { Depth = 6, Width = 6, Height = 1, PartID = "6106" };
                yield return (s, x, -y);
                s.RotateThis();
                yield return (s, x, y);
                s.RotateThis();
                yield return (s, -x, y);
                s.RotateThis();
                yield return (s, -x, -y);
            }
            if (layer == 0)
            {
                Shape s = new() { Depth = 6, Width = 6, Height = 1, PartID = "2419" };
                double r = 23.5;
                yield return (s, 0, -r);
                s.RotateThis();
                yield return (s, r, 0);
                s.RotateThis();
                yield return (s, 0, r);
                s.RotateThis();
                yield return (s, -r, 0);

                yield return (new Plate(8, 1), 0, 21.5);
                yield return (new Plate(8, 1), 0, -21.5);
                yield return (new Plate(1, 8), 21.5, 0);
                yield return (new Plate(1, 8), -21.5, 0);

                for (int i = 0; i < 4; i++)
                {
                    int s1 = (i / 2) * 2 - 1;
                    int s2 = (i % 2) * 2 - 1;
                    yield return (new Plate(1, 1), s1 * 9.5, s2 * 15.5);
                    yield return (new Plate(1, 1), s1 * 15.5, s2 * 9.5);
                }

                for (int ix = -3; ix <= 3; ix++)
                {
                    int ly = 3 + (ix < 0 ? ix : -ix);
                    for (int iy = -ly; iy <= ly; iy++)
                        yield return (new Plate(6, 6), ix * 6, iy * 6);
                }
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    double x = 6 * i + 1;
                    double y = 6 * (4 - i) + 1;
                    Shape s = new() { Depth = 2, Width = 2, Height = 1, PartID = "35787" };
                    s.RotateThis(-90.0);
                    yield return (s, x, -y);
                    s.RotateThis();
                    yield return (s, x, y);
                    s.RotateThis();
                    yield return (s, -x, y);
                    s.RotateThis();
                    yield return (s, -x, -y);
                }
                for (int ix = 0; ix < 3; ix++)
                    for (int iy = 0; iy < 3 - ix; iy++)
                        for (int i = 0; i < 4; i++)
                        {
                            int s1 = (i / 2) * 2 - 1;
                            int s2 = (i % 2) * 2 - 1;
                            yield return (new Plate(6, 6), s1 * (3 + ix * 6), s2 * (3 + iy * 6));
                        }
            }
        }
    }
}
