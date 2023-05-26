using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    public static class EinsteinHat2
    {//second version, with actual hats


        static readonly Dictionary<string, int> HatTypeColor = new()
        {
            { "H1", ColorMap.Get("Red").id },
            { "H", ColorMap.Get("Yellow").id },
            { "T", ColorMap.Get("Black").id },
            { "P", ColorMap.Get("White").id },
            { "F", ColorMap.Get("Light_Bluish_Grey").id }
        };


        public static void Generate(StringBuilder sb, int level)
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
            double c = hatData[0].mat[4];
            double s = -hatData[0].mat[1];
            double scale = Math.Sqrt(c * c + s * s);
            double unitLength = 7.0/Math.Sqrt(3.0) + 1.0;
            scale /= unitLength;
            List<(double angleRad, bool mirror, double x, double y, int color)> parameters = hatData.Select(((string hatType, double[] mat) hat) =>
            {
                bool mirrored = Math.Abs(hat.mat[0] + hat.mat[4]) < 1.0e-6 && Math.Abs(hat.mat[1] - hat.mat[3]) < 1.0e-6;
                return (Math.Atan2(-hat.mat[1], hat.mat[4]), mirrored, hat.mat[2], hat.mat[5], HatTypeColor[hat.hatType]);
            }).ToList();
            double closestToZeroAngle = parameters.MinBy(t => Math.Abs(t.angleRad)).angleRad;
            bool doRot = closestToZeroAngle > 1.0e-6;
            if (doRot)
            {
                c = Math.Cos(-closestToZeroAngle);
                s = Math.Sin(-closestToZeroAngle);
            }
            bool first = true;
            double offsetX = 0, offsetY = 0;

            HashSet<int> usedColors = new();
            HashSet<int> usedMirrorColors = new();

            foreach (var p in parameters)
            {
                double angleRad = p.angleRad;
                double x = p.x / scale;
                double y = p.y / scale;
                if (doRot)
                {
                    angleRad -= closestToZeroAngle;
                    (x, y) = (c * x - s * y, s * x + c * y);
                }

                if (first)
                {
                    offsetX = x - Math.Round(x);
                    offsetY = y - Math.Round(y);
                    first = false;
                }
                (x, y) = (x - offsetX, y - offsetY);

                int rotation = (((int)(6.5 + 3 * angleRad / Math.PI)) % 6) * 60;
                if (rotation > 180) rotation += 360;
                if (p.mirror)
                {
                    usedMirrorColors.Add(p.color);
                }
                else
                    usedColors.Add(p.color);
                sb.AppendLine(new Shape() { PartID = "Hat" + SubModelNameSuffix(p.mirror, p.color), SubModel = true }.Rotate(rotation).Print(x, y, 0, p.color));
            }

            //copy .ldr contents...
            string[] templateFiles = new[] { "whiteHat.ldr", "redMirrorHat.ldr" };
            string[] toReplace = new[] { "_white", "_red" };
            string[] whiteParts = new[] { "2420.dat", "2429.dat", "2430.dat" };
            for (int i = 0; i < 2; i++)
            {

                string[] fileLines = File.ReadAllLines(Path.Combine(dir, templateFiles[i]));
                string specialLine = fileLines.First(line => line.EndsWith("6141.dat"));
                var parts = specialLine.Split(' ');
                double ldux_offset = double.Parse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture)+((i==0)?-10:10);
                double lduy_offset = double.Parse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture);
                double lduz_offset = double.Parse(parts[4], NumberStyles.Float, CultureInfo.InvariantCulture) + 10 - 20.0 * (7 + Math.Sqrt(3.0));
                var set = i == 0 ? usedColors : usedMirrorColors;
                foreach (int color in set)
                {
                    string suffix = SubModelNameSuffix(i==1, color);
                    MetaData.StartSubModel(sb, "Hat" + suffix);
                    foreach (string line in fileLines.Skip(7).TakeWhile(line => !line.EndsWith("6141.dat")))
                    {
                        parts = line.Split(' ');
                        double ldux = double.Parse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture);
                        double lduy = double.Parse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture);
                        double lduz = double.Parse(parts[4], NumberStyles.Float, CultureInfo.InvariantCulture);
                        sb.Append($"1 {parts[1]} {ldux - ldux_offset} {lduy - lduy_offset} {lduz - lduz_offset} ");
                        foreach (string part in parts.Skip(5).Take(9))
                            sb.Append(part + " ");
                        sb.AppendLine(parts.Last().Replace(toReplace[i], suffix));
                    }
                    foreach (string line in fileLines.SkipWhile(line => !line.EndsWith("6141.dat")).Skip(1))
                    {
                        parts = line.Split(' ');
                        if (parts[1] == "16" || parts[0] == "0")
                            parts[parts.Length - 1] =  parts[parts.Length - 1].Replace(toReplace[i], suffix);
                        else if (parts[0] == "1" && parts[1] != "0" && !whiteParts.Contains(parts[parts.Length - 1]))
                            parts[1] = color.ToString();
                        sb.AppendLine(String.Join(' ',parts));
                    }
                }
            }
        }
        private static string SubModelNameSuffix(bool mirrored, int color) => (mirrored ? "_mirror_" : "_") + color.ToString();

    }
}
