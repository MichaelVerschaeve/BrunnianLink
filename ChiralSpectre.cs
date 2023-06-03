using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace BrunnianLink
{
    public class ChiralSpectre
    {


        public static void Generate(StringBuilder sb, int level)
        {
            level = Math.Max(Math.Min(4, level), 1);
            string dir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.FullName!;
            dir = Directory.GetParent(dir)?.FullName!;
            dir = Directory.GetParent(dir)?.FullName!;
            dir = Directory.GetParent(dir)?.FullName!;
            dir = Path.Combine(dir, "SpectrePolies");

            string file = Path.Combine(dir, $"it{level}.svg");
            var allLines = File.ReadAllLines(file);
            var fixedLines = allLines.Where(l => l.StartsWith("<polygon")).Zip(allLines.Where(l => l.StartsWith(","))).Select(t => t.First.Trim() + t.Second.Trim());
            HashSet<int> usedColors = new();
            bool first = true;
            bool mirror = false;
            MetaData.StartSubModel(sb, $"Chiral_Spectre_tiling_{level}");

            List<string> someColorNames = new() { "White", "Bright_Pink", "Dark_Blue", "Red", "Lime", "Medium_Azure", "Medium_Orange", "Tan", "Light_Aqua" };
            

            Dictionary<Color, int> mappedColors = new();

            foreach (var line in fixedLines)
            {
                var parts = line.Split('\"');
                var points = parts[1].Split(" ").Take(3).Select(s => (0.3 * Double.Parse(s.Split(",")[0], NumberStyles.Float, CultureInfo.InvariantCulture), -0.3 * Double.Parse(s.Split(",")[1], NumberStyles.Float, CultureInfo.InvariantCulture))).ToArray();
                double angle1Deg = 180.0 * Math.Atan2(points[1].Item2 - points[0].Item2, points[1].Item1 - points[0].Item1) / Math.PI;
                if (first)
                {
                    double angle2Deg = 180.0 * Math.Atan2(points[2].Item2 - points[1].Item2, points[2].Item1 - points[1].Item1) / Math.PI;
                    double angleDiff = (360+ angle1Deg - angle2Deg)%360;
                    if (angleDiff > 180)
                        mirror = true;
                    
                    first = false;
                }
                if (mirror)
                {
                    points[0].Item1 = -points[0].Item1;
                    angle1Deg = 180 - angle1Deg;
                }

                int i1 = parts[^2].IndexOf("(") + 1;
                int i2 = parts[^2].IndexOf(")");
                var colorParts = parts[^2][i1..i2].Split(",").Select(s => Byte.Parse(s)).ToArray();
                Color svgColor = Color.FromArgb(colorParts[0], colorParts[1], colorParts[2]);
                if (!mappedColors.TryGetValue(svgColor, out int colorId))
                {
                    colorId = ColorMap.Get(someColorNames[usedColors.Count]).id;
                    mappedColors.Add(svgColor, colorId);
                    usedColors.Add(colorId);
                }
                Shape s = new() { PartID = $"Spectre_{colorId}", SubModel = true };
                s.RotateThis(angle1Deg);
                sb.AppendLine(s.Print(points[0].Item1, points[0].Item2, 4, 16)); //3 layers tile, 1 layer marker... connectors below 0

            }
            foreach (int colorId in usedColors)
            {
                MetaData.StartSubModel(sb, $"Spectre_{colorId}");
                sb.AppendLine(new Shape() { PartID = $"Bow_{colorId}", SubModel = true }.Print(0, 0, 0, 16));
                sb.AppendLine(new Shape() { PartID = $"Flower_{colorId}", SubModel = true }.Rotate(30).Print(9,- 3 * Math.Sqrt(3.0), 0, 16));
            }
            //copy .ldr contents...
            string[] templateFiles = { "bow.ldr", "flower.ldr" };
            string[] partNames = { "Bow_", "Flower_" };
            for (int i = 0; i < 2; i++)
            {

                string[] fileLines = File.ReadAllLines(Path.Combine(dir, templateFiles[i]));
                string specialLine = fileLines.First(line => line.EndsWith("6141.dat"));
                var parts = specialLine.Split(' ');
                double ldux_offset = double.Parse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture) - 10;
                double lduy_offset = double.Parse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture);
                double lduz_offset = double.Parse(parts[4], NumberStyles.Float, CultureInfo.InvariantCulture) - 10;
                foreach (int colorId in usedColors)
                {
                    MetaData.StartSubModel(sb, partNames[i] + colorId.ToString());
                    foreach (string line in fileLines.SkipWhile(line => line.StartsWith('0')))
                    {
                        parts = line.Split(' ');
                        int color = int.Parse(parts[1]);
                        if (color == 4) //the red dot,
                            continue;
                        else if (color == 15)
                            color = colorId;
                        double ldux = double.Parse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture);
                        double lduy = double.Parse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture);
                        double lduz = double.Parse(parts[4], NumberStyles.Float, CultureInfo.InvariantCulture);
                        sb.Append($"1 {color} {ldux - ldux_offset} {lduy - lduy_offset} {lduz - lduz_offset} ");
                        sb.AppendLine(string.Join(" ", parts.Skip(5)));
                    }
                }
            }
        }
    }
}