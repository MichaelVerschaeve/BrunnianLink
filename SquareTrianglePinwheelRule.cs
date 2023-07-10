using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    public class SquareTrianglePinwheelRule : SubstitutionRule
    {
        private readonly string[] m_colors = new[] { "Lavender", "Dark_Red", "Dark_Red", "Lavender" };
        public override string[] Colors => m_colors;

        public override int StateCount => 4;

        public override double ScaleFactor => Math.Sqrt(5);

        public override double InitialScale => Math.Sqrt(5);

        public override string MainName => "SquareTrianglePinwheel";

        public override string BasePart(int state, int color) => state switch { 1 => "65426", 2 => "65429" , _ => Tile.XYPartID(2, 2) };

        public override bool ColorByState => true;

        private static readonly  double angleDegree = 180*Math.Atan2(1,2)/Math.PI;

        readonly List<(double x, double y, double rotation, int state)>[] m_rules = new[]
        {
            new List<(double x, double y, double rotation, int state)>() 
            {
                (0.0,0.0,-angleDegree,3),
                (-1.0,0.0,-angleDegree,1),
                (1.0,0.0,180-angleDegree,1),
                (0.0,1.0,-90-angleDegree,1),
                (0.0,-1.0,90-angleDegree,1)
            },
            new List<(double x, double y, double rotation, int state)>() 
            {
                (1.0,1.0,angleDegree,2),
                (1.0,-1.0,angleDegree,2),
                (0.0,2.0,90+angleDegree,2),
                (0.4,0.2,angleDegree,0),
                (0.0,1.0,angleDegree,3)
            },
        };

        public override List<(double x, double y, double rotation, int state)> Rule(int state) => state > 1 ? m_rules[3 - state].Select(t => (-t.x, t.y, -t.rotation, 3 - t.state)).ToList() : m_rules[state];

        //public override (double x, double y, double rotation, int state)[] StartStates => new[] {(0.0,0.0,0.0,1) };
    }
}
