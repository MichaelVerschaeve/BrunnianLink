using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    public static class EinsteinHat
    {
        //static readonly string[] m_colors = new string[] { "Lime", "Light_Bluish_Grey", "Medium_Azure", "Red", "Dark_Blue" };

        static readonly Dictionary<string, int> HatTypeColor = new()
        {
            { "H1", ColorMap.Get("Dark_Blue").id },
            { "H", ColorMap.Get("Medium_Azure").id },
            { "T", ColorMap.Get("Red").id },
            { "P", ColorMap.Get("Lime").id },
            { "F", ColorMap.Get("White").id }
        };

        static readonly bool test = false;
        public static void Generate(StringBuilder sb, int level)
        {
            if (test)
            {
                MetaData.StartSubModel(sb, "EinsteinHat");
                Shape h = GetHat((level - 1) / 2, ((level - 1) % 2) == 1);
                sb.AppendLine(h.Print(0, 0, 1, HatTypeColor["T"]));
            }
            else if (level==0)
            {
                MetaData.StartSubModel(sb, "EinsteinHat");
                Shape h = GetHat(0,false);
                sb.AppendLine(h.Print(0, 0, 1, HatTypeColor["T"]));
            }
            else
            {
                string dir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.FullName!;
                dir = Directory.GetParent(dir)?.FullName!;
                dir = Directory.GetParent(dir)?.FullName!;
                dir = Directory.GetParent(dir)?.FullName!;
                dir = Path.Combine(dir, "HatMatrices");

                string file = Directory.GetFiles(dir, "*.txt").OrderBy(a => new FileInfo(a).Length).ElementAt(level - 1);
                MetaData.StartSubModel(sb, "EinsteinHat_" + Path.GetFileNameWithoutExtension(file));
                List<(string hatType, double[] mat)> hatData = new();
                foreach (string line in File.ReadAllLines(file))                                                                                                                                                                                                                                                                           
                {
                    var parts = line.Split(' ');
                    hatData.Add((parts[0], parts.Skip(1).Select(s => double.Parse(s, NumberStyles.Float, CultureInfo.InvariantCulture)).ToArray()));
                }
                //TODO math...
                double c = hatData[0].mat[0];
                double s = hatData[0].mat[3];
                double scale = Math.Sqrt(c * c + s * s)/4.0;
                List<(double angleRad, bool mirror, double x, double y, int color)> parameters = hatData.Select(((string hatType, double[] mat) hat) =>
                {
                    bool mirrored = Math.Abs(hat.mat[0] + hat.mat[4]) < 1.0e-6 && Math.Abs(hat.mat[1] - hat.mat[3]) < 1.0e-6;
                    return (Math.Atan2(hat.mat[3], hat.mat[4]), mirrored, hat.mat[2], hat.mat[5], HatTypeColor[hat.hatType]);
                }).ToList();
                double closestToZeroAngle = parameters.MinBy(t => Math.Abs(t.angleRad)).angleRad;
                bool doRot = closestToZeroAngle > 1.0e-6;
                if (doRot)
                {
                    c = Math.Cos(-closestToZeroAngle);
                    s = Math.Sin(-closestToZeroAngle);
                }
                bool first = true;
                double offsetX=0, offsetY=0;
                foreach (var p in parameters)
                {
                    double angleRad = p.angleRad;
                    double x = p.x/scale;
                    double y = p.y/scale;
                    if (doRot)
                    {
                        angleRad -= closestToZeroAngle;
                        (x, y) = (c * x - s * y, s * x + c * y);
                    }
                    y *= 2.0 / Math.Sqrt(3.0);

                    if (first)
                    {
                        offsetX = x - Math.Round(x);
                        offsetY = y - Math.Round(y);
                        first = false;
                    }
                    (x, y) = (x-offsetX,y-offsetY);
                    
                    int rotIndex = ((int)(6.5 + 3 * angleRad / Math.PI)) % 6;
                    sb.AppendLine(GetHat(rotIndex, p.mirror).Print(x, y, 1, p.color));
                }
            }

            Tile0degrees(sb, false);
            Tile60degrees(sb, false);
            Tile120degrees(sb, false);
            Tile0degrees(sb, true);
            Tile60degrees(sb, true);
            Tile120degrees(sb, true);
        }
        static Shape GetHat(int rotation, bool mirror)
        {
            int deg = (rotation % 3) * 60;
            string suffix = mirror ? "_mirror" : "";
            var res = new Shape() { PartID = $"Tile{deg}{suffix}", SubModel = true };
            if (rotation > 2)
                res.RotateThis(180);
            return res;
        }

        static bool InHexagon((int x, int y) p)
        {
            int hexaY = p.y / 4;
            int hexaX = (p.x - 2 * hexaY) / 4;
            if ((hexaX & 1) == 1 || (hexaY & 1) == 1) return false;
            return ((hexaX - hexaY) / 2) % 3 == 0;
        }

        static void DoOutline(StringBuilder sb, IEnumerable<(int x, int y)> points)
        { //coordinate expected clockwise, cartesian (i.e. already translated from hexagonal)
            Shape wedgeLeft = new() { PartID = "65429", Width = 2, Depth = 4 };// |/
            Shape wedgeRight = new() { PartID = "65426", Width = 2, Depth = 4 };// \|
            Shape triangle = new() { PartID = "35787", Width = 2, Depth = 2 };// |\
            var segments = points.Zip(points.Skip(1).Concat(points.Take(1)));
            foreach(var (First, Second) in segments) 
            {
                (int x, int y) p1 = First;
                (int x, int y) p2 = Second;
                int dx = (p2.x - p1.x);
                int dy = (p2.y - p1.y);
                if (dx == 0 || dy == 0) continue; //straight lines are done by manual plates
                int adx = Math.Abs(dx);
                int ady = Math.Abs(dy);
                int sx = Math.Sign(dx);
                int sy = Math.Sign(dy);
                Shape? shape;
                if (2*adx == 3 * ady || 2*ady == 3 * adx)
                { //30 degrees, first triangle then wedge

                    int kwadrant = (sy > 0 ? 0 : 2) + (sx == sy ? 0 : 1);
                    shape = triangle.Rotate((kwadrant - 1) * 90);
                    if (InHexagon(p1))
                    {
                        sb.AppendLine(shape.Print(p1.x + sx, p1.y + sy, 0, 16));
                        p1.x += 2 * sx;
                        p1.y += 2 * sy;
                    }
                    else
                    {
                        sb.AppendLine(shape.Print(p2.x - sx, p2.y - sy, 0, 16));
                        p2.x -= 2 * sx;
                        p2.y -= 2 * sy;
                    }
                    dx -= 2 * sx;
                    //dy -= 2 * sy;
                    shape = (sx == sy ? wedgeRight : wedgeLeft).Rotate(90 * sy);
                }
                else
                {
                    shape = sx == sy ? wedgeLeft : wedgeRight;
                    if (sx < 0)
                        shape = shape.Rotate(180);
                }

                if (shape == null) continue; //straight edges, nothing to do
                int times = Math.Abs(dx / shape.Width);
                dx = sx * shape.Width;
                dy = sy * shape.Depth;

                int x = p1.x + dx / 2;
                int y = p1.y + dy / 2;
                for (int j = 0; j < times; j++)
                {
                    sb.AppendLine(shape.Print(x, y, 0, 16));
                    x += dx;
                    y += dy;
                }
            }
        }

        static readonly (int x, int y)[] hat_outline = {
            (0, 0), (-1, -1), (0, -2), (2, -2),
            (2, -1), (4, -2), (5, -1), (4, 0),
            (3, 0), (2, 2), (0, 3), (0, 2),
            (-1, 2)
        };

        static void Tile0degrees(StringBuilder sb, bool mirrored) //horizontal base, top point at center
        {
            MetaData.StartSubModel(sb, mirrored ? "Tile0_mirror" : "Tile0");
            int s = mirrored ? -1 : 1;
            var outline = hat_outline.Select(p => (s * (4 * p.x + 2 * p.y), 4 * p.y));
            if (mirrored) 
                outline = outline.Reverse();
            DoOutline(sb, outline);
            //add interior manually...
            sb.AppendLine(new Plate(8, 8).Print(s * 4, 4, 0, 16));
            sb.AppendLine(new Plate(2, 2).Print(-s, -3, 0, 16));
            Plate plate = new Plate(8, 4);
            sb.AppendLine(plate.Print(s * 4, -2, 0, 16));
            sb.AppendLine(plate.Print(s * 12, -2, 0, 16));
            sb.AppendLine(plate.Print(0, -6, 0, 16));
            plate.RotateThis(90);
            sb.AppendLine(plate.Print(s * 10, 4, 0, 16));
            plate = new Plate(4, 2);
            sb.AppendLine(plate.Print(s * 8, 9, 0, 16));
            sb.AppendLine(plate.Print(s * 12, -5, 0, 16));
        }

        static void Tile60degrees(StringBuilder sb, bool mirrored) 
        {
            MetaData.StartSubModel(sb, mirrored ? "Tile60_mirror" : "Tile60");
            int s = mirrored ? -1 : 1;
            var outline = hat_outline.Select(p => (2 * s * (p.x - p.y), 4 * (p.x + p.y)));
            if (mirrored)
                outline = outline.Reverse();
            DoOutline(sb, outline);
            //manually added interior
            sb.AppendLine(new Plate(2,8).Print(-3*s, 8, 0, 16));
            sb.AppendLine(new Plate(2,12).Print(-s, 8, 0, 16));
            sb.AppendLine(new Plate(2,2).Print(s, 13, 0, 16));
            sb.AppendLine(new Plate(6, 10).Print( 3*s, 7, 0, 16));
            sb.AppendLine(new Plate(6, 6).Print( 3*s, -1, 0, 16));
            sb.AppendLine(new Plate(4,4).Print(2 * s, -6, 0, 16));
            Plate plate = new(2, 4);
            sb.AppendLine(plate.Print(7 * s, 10, 0, 16));
            sb.AppendLine(plate.Rotate().Print(8 * s, 7, 0, 16));
            sb.AppendLine(new Plate(4, 8).Print(10 * s, 12, 0, 16));
        }

        static void Tile120degrees(StringBuilder sb, bool mirrored) 
        {
            MetaData.StartSubModel(sb, mirrored ? "Tile120_mirror" : "Tile120");
            int s = mirrored ? -1 : 1;
            var outline = hat_outline.Select(p => (-s * (4 * p.y + 2 * p.x), 4 * p.x));
            if (mirrored)
                outline = outline.Reverse();
            DoOutline(sb, outline);
            //manually added interior
            sb.AppendLine(new Plate(8, 8).Print(-8 * s, 4, 0, 16));
            sb.AppendLine(new Plate(6, 6).Print(-3 * s, 13, 0, 16));
            sb.AppendLine(new Plate(8, 4).Print(0, 6, 0, 16));
            sb.AppendLine(new Plate(10, 2).Print(-5 * s, 9, 0, 16));
            Plate plate = new(6, 4);
            sb.AppendLine(plate.Print(-s, 2, 0, 16));
            sb.AppendLine(plate.Rotate().Print(4*s, 1, 0, 16));
            plate = new(4,2);
            sb.AppendLine(plate.Print(-4 * s, 17, 0, 16));
            sb.AppendLine(plate.Print(-4 * s, -1, 0, 16));
        }
    }
}
