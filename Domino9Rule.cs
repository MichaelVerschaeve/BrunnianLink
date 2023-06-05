using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    public class Domino9Rule : SubstitutionRule
    {
        private readonly string[] m_colors = new[] { "Dark_Orange", "Bright_Light_Yellow", "Bright_Light_Yellow", "Dark_Orange" };
        public override string[] Colors => m_colors;

        public override int StateCount => 4;

        public override double ScaleFactor => 3;

        public override double InitialScale => 1;

        public override string MainName => "Domino9";

        public override bool ColorByState => true;

        public override bool Level0IsComposite => false; //easy!

        public override string BasePart(int state, int color) => "35480";

        private readonly List<(double x, double y, double rotation, int state)> baseRule = new()
        {
            (-2.0,-1.0,0,0),
            (0.0,1.0,180,0),
            (2.0,-1.0,0,3),

            (-2.5,0.5,-90,2),
            (-1.5,0.5,90,1),

            (-0.5,-0.5,-90,1),
            (0.5,-0.5,90,2),

            (1.5,0.5,-90,2),
            (2.5,0.5,90,1)
        };

        public override List<(double x, double y, double rotation, int state)> Rule(int state)
        {
            if (state > 1)
                return Rule(3 - state).Select(t => (-t.x, t.y, -t.rotation, 3 - state)).ToList();
            else if (state == 1)
                return baseRule.Select(t => (t.x, t.y, t.rotation, t.state ^ 1)).ToList(); //swap state 0 and 1, swap state 2 and 3
            else
                return baseRule;
        }
    }
}
