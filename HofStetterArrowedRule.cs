using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    public class HofStetterArrowedRule : SubstitutionRule
    {
        private static readonly string[] m_colors = new[] { "Black", "Dark_Bluish_Grey", "Light_Bluish_Grey", "White" };
        public override string[] Colors => m_colors;

        public override int StateCount => 4;

        public override double ScaleFactor => 2;

        public override double InitialScale => 0.5;

        public override string MainName => "Hofstetter";

        public override string BasePart(int state, int color)
        {
            return Tile.XYPartID(1, 1);
        }

        public override bool ColorByState => true;

        private static readonly (double x, double y, double rotation, int state)[] m_startStates = new[] { (0.0, 0.0, 0.0, 2) };

        public override (double x, double y, double rotation, int state)[] StartStates => m_startStates;

        private static readonly List<(double x, double y, double rotation, int state)>[] m_rules = new[]
{
            new List<(double x, double y, double rotation, int state)>
            {
                (-1,1,-90,3),
                (1,1,0,0),
                (-1,-1,0,1),
                (1,-1,-90,2)
            },
            new List<(double x, double y, double rotation, int state)>
            {
                (-1,1,90,2),
                (1,1,0,0),
                (-1,-1,0,1),
                (1,-1,90,3)
            },
            new List<(double x, double y, double rotation, int state)>
            {
                (-1,1,-90,3),
                (1,1,0,0),
                (-1,-1,0,1),
                (1,-1,90,3)
            },
            new List<(double x, double y, double rotation, int state)>
            {
                (-1,1,-90,3),
                (1,1,0,0),
                (-1,-1,0,1),
                (1,-1,90,3)
            },
        };
        public override List<(double x, double y, double rotation, int state)> Rule(int state) => m_rules[state];
    }
}
