using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    public class SocolarSquareTriangleRule : SubstitutionRule
    {
        private readonly string[] colors = new string[] {"White","Yellow","Red"};
        public override string[] Colors => colors;

        public override int StateCount => 3;

        public override double ScaleFactor => 1 + Math.Sqrt(3.0);

        public override double InitialScale => 9 + 2*Math.Sqrt(3.0);

        public override string MainName => "SocolarSquareTriangle";

        public override string BasePart(int state, int color) => state switch { 0 => "Square", 1 => "Triangle1", _ => "Triangle2" };

        static private readonly double si60 = Math.Sqrt(3.0) * 0.5;
        private readonly List<(double x, double y, double rotation, int state)> halfSquareRule = new() {
            (0.0,-2*si60/3.0,0.0,1),
            (-0.5,-si60/3.0,180.0,2),
            (-0.5,si60/3.0,120.0,1),
            (-0.75-0.5*si60,0.5*si60+0.25,60.0,0),
            (-0.75-0.5*si60,-0.5*si60-0.25,30.0,0),
            (-1.0-2.0*si60/3.0,0.0,150.0,1)
        };
        
        private readonly List<(double x, double y, double rotation, int state)> halfTriangleRule = new() {
            (-2.0*si60/3.0-0.5, -si60/3.0,30.0,1),
            (-0.5*si60-0.25,-si60/3.0+0.25+0.5*si60,-30.0,0),
            (-0.5*si60+si60/6.0,2.0*si60/3.0+0.5,-150.0,1)
        };



        public override List<(double x, double y, double rotation, int state)> Rule(int state)
        {
            switch (state)
            {
                case 0:
                    return halfSquareRule.Concat(halfSquareRule.Select(t=>(-t.x,-t.y,t.rotation+180,t.state))).ToList();
                case 1:
                    return halfTriangleRule.Concat(halfTriangleRule.Select(t => (-t.x, t.y, t.rotation + 60, t.state))).Append((0.0, 0.0, 0.0, 2)).ToList();
                default:
                    return new() { (0.0, 0.0, 0.0, 2) };
            }
        }

        public override bool ColorByState => true;
        public override bool Level0IsComposite => true;



        private readonly string folder;
        public SocolarSquareTriangleRule()
        {
            string dir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.FullName!;
            dir = Directory.GetParent(dir)?.FullName!;
            dir = Directory.GetParent(dir)?.FullName!;
            dir = Directory.GetParent(dir)?.FullName!;
            folder = Path.Combine(dir, "Socolar");
        }


        public override void DefineCompositeBasePart(StringBuilder sb, int state, int color)
        {
            string file = Path.Combine(folder, state switch {1=>"triangle_red.ldr",2=>"triangle_yellow.ldr",_=> "square.ldr"});
            string[] fileLines = File.ReadAllLines(file);
            var specialLines = fileLines.Where(line => line.EndsWith("73230.dat"));
            double ldux_offset = 0;
            double lduy_offset = 0;
            double lduz_offset = 0;
            int cnt = 0;
            foreach (string line in specialLines) 
            {
                var parts = line.Split(' ');
                ldux_offset += double.Parse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture);
                lduy_offset += double.Parse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture);
                lduz_offset += double.Parse(parts[4], NumberStyles.Float, CultureInfo.InvariantCulture);
                cnt++;
            }
            ldux_offset /= cnt;
            lduy_offset /= cnt;
            lduz_offset /= cnt;

            foreach (string line in fileLines.SkipWhile(line => line.StartsWith('0')))
            {
                var parts = line.Split(' ');
                double ldux = double.Parse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture);
                double lduy = double.Parse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture);
                double lduz = double.Parse(parts[4], NumberStyles.Float, CultureInfo.InvariantCulture);
                sb.Append($"1 {parts[1]} {ldux - ldux_offset} {lduy - lduy_offset} {lduz - lduz_offset} ");
                sb.AppendLine(string.Join(" ", parts.Skip(5)));
            }
        }

        public override (double x, double y, double rotation, int state)[] StartStates => new[] {(0.0,0.0,0.0,0) };

    }
}
