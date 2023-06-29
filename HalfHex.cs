using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    public class HalfHex : SubstitutionRule
    {
        private static readonly string[] m_colors = new string[] { "Red", "Lime", "Medium_Azure", "Dark_Blue" };
        public override string[] Colors => m_colors;

        public override int StateCount => 1;

        public override double ScaleFactor => 2;

        public override double InitialScale => 5 + Math.Sqrt(3.0);

        public override string MainName => "HalfHex";

        public override string BasePart(int state, int color) => $"Trapezoid_{color}";

        readonly List<(double x, double y, double rotation, int state)> m_rule = new()
        {
            (0.0,0.0,0.0,0),
            (0.0,Math.Sqrt(3.0),180.0,0),
            (-1.5,0.5*Math.Sqrt(3.0),-120.0,0),
            (1.5,0.5*Math.Sqrt(3.0),120.0,0)
        };
        public override List<(double x, double y, double rotation, int state)> Rule(int state) => m_rule;

        public override bool Level0IsComposite => true;


        private readonly string[] fileLines;
        private readonly double ldux_offset, lduy_offset, lduz_offset;
        public HalfHex()
        {
            string folder = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.FullName!;
            folder = Directory.GetParent(folder)?.FullName!;
            folder = Directory.GetParent(folder)?.FullName!;
            folder = Directory.GetParent(folder)?.FullName!;
            string file = Path.Combine(folder, "HalfHex.ldr");
            fileLines = File.ReadAllLines(file);
            string specialLine = fileLines.First(line => line.EndsWith("22385.dat"));
            var parts = specialLine.Split(' ');
            ldux_offset = double.Parse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture);
            lduy_offset = double.Parse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture);
            lduz_offset = double.Parse(parts[4], NumberStyles.Float, CultureInfo.InvariantCulture) + 30 - 10 * InitialScale * Math.Sqrt(3.0); ;
        }

        public override void DefineCompositeBasePart(StringBuilder sb, int state, int color)
        {

            int colorId = ColorMap.Get(m_colors[color]).id;
            foreach (string line in fileLines.SkipWhile(line => line.StartsWith('0')))
            {
                var parts = line.Split(' ');

                double ldux = double.Parse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture);
                double lduy = double.Parse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture);
                double lduz = double.Parse(parts[4], NumberStyles.Float, CultureInfo.InvariantCulture);
                sb.Append($"1 {(parts[1] != "0" ? colorId : 0)} {ldux - ldux_offset} {lduy - lduy_offset} {lduz - lduz_offset} ");
                sb.AppendLine(string.Join(" ", parts.Skip(5)));
            }
        }
    }

}
