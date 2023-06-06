using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    public class Pentomino : SubstitutionRule
    {
        private readonly string[] m_colors = new[] { "Dark_Blue", "Tan" };
        public override string[] Colors => m_colors;

        public override int StateCount => 2;

        public override double ScaleFactor => 2;

        public override double InitialScale => 1;

        public override string MainName => "Pentomino";

        public override bool ColorByState => true;

        public override string BasePart(int state, int color) => state switch { 0 => "straight", _ => "mirror" };
        List<(double x, double y, double rotation, int state)>[] m_rules = new[]
        {
            new List<(double x, double y, double rotation, int state)>()
            {
                (0,0,0,0),
                (0,6,180,1),
                (4,0,0,1)
            },
            new List<(double x, double y, double rotation, int state)>()
            {
                (0,0,0,1),
                (0,6,180,0),
                (-4,0,0,0)
            },
        };
        public override List<(double x, double y, double rotation, int state)> Rule(int state) => m_rules[state];

        public override bool Level0IsComposite => base.Level0IsComposite;

        public override void DefineCompositeBasePart(StringBuilder sb, int state, int color)
        {
            int s = state==0 ? 0 : 1;
            int colorId = ColorMap.Get(Colors[state]).id;
            sb.AppendLine(new Plate(2, 2).Print(s, 1, 0, colorId));
            sb.AppendLine(new Plate(1, 1).Print(0.5*s, 2.5, 0, colorId));
        }

    }
}
