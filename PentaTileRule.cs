using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    public class PentaTileRule : SubstitutionRule
    {
        private readonly string[] m_colors = new[] { "Dark_Blue", "Orange", "Tan" };
        public override string[] Colors => m_colors;

        public override bool ColorByState => true;


        public override int StateCount => 3;

        public override double ScaleFactor => 2*Math.Sqrt(2.0);

        public override double InitialScale => 0.5*Math.Sqrt(2.0);

        public override string MainName => "PentagonalTileTiling";
        public override string BasePart(int state, int color) => state switch { 0 => "22385", 1 => "35787", _ => Tile.XYPartID(2, 2) };

        private readonly List<(double x, double y, double rotation, int state)> m_ruleSquare = new()
        {
            (-2.5,2.5,-135,0),
            (2.5,2.5,135,0),
            (-2.5,-2.5,-45,0),
            (2.5,-2.5,45,0),
            (0,0,45,2),
            (0,4,45,1),
            (0,-4,-135,1),
            (4,0,-45,1),
            (-4,0,135,1)
        };

        private readonly List<(double x, double y, double rotation, int state)> m_ruleTriangle = new()
        {
            (-2,0,-135,1),
            (2,0,-135,1),
            (0,2,45,2)
        };

        static private readonly double unit = Math.Sqrt(2.0) * 0.5;

        //public override (double x, double y, double rotation, int state)[] StartStates => new[] { (0.0, 0.0, 0.0, 1) };

        public override List<(double x, double y, double rotation, int state)> Rule(int state)
       => state switch
       {
           0 => m_ruleSquare.Select(t => (t.x, t.y + 2, t.rotation, t.state)).Concat(m_ruleTriangle.Select(t => (-t.x, -t.y -2, t.rotation+180, t.state))).ToList(),
           1 => m_ruleTriangle.Select(t => (t.x, t.y -4, t.rotation, t.state)).Concat(m_ruleTriangle.Select(t => (t.y-4, t.x, t.rotation-90, t.state))).ToList(),
           2 => m_ruleSquare,
           _ => new()
       };
    }
}
