using System;
using System.Collections.Generic;
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
            (0.0,-2*si60/3.0,0.0,1)

        };
        
        private readonly List<(double x, double y, double rotation, int state)> halfTriangleRule = new() {
            (0.0,-2*si60/3.0,0.0,1)

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

        public override void DefineCompositeBasePart(StringBuilder sb, int state, int color)
        {
            base.DefineCompositeBasePart(sb, state, color);
        }

    }
}
