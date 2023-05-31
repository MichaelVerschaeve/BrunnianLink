using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    public class MiniTangramRule : SubstitutionRule
    {
        static private readonly string[] m_colors = new[] { "Red", "Yellow", "Blue" };
        public override string[] Colors => m_colors.Concat(m_colors.Reverse()).ToArray();

        public override int StateCount => 6; //3 states are mirror immages

        public override double ScaleFactor => 2;

        public override double InitialScale => 1;

        public override bool ColorByState => true;

        public override string MainName => "MiniTangram";

        public override string BasePart(int state) => state switch { 
            0 => "square", 
            1 => "tria1", 
            2 => "tria2",
            3 => "tria2_m",
            4 => "tria1_m",
            5 => "square_m",
            _ => "" };

        static readonly List<(double x, double y, double rotation, int state)>[] m_rules = new[]
        {
            new List<(double x, double y, double rotation, int state)>()
            {
                (-0.5,2.5,-90.0,0),
                (0.5,0.5,0.0,0),
                (-0.5,-0.5,0.0,0),
                (2.5,-0.5,90.0,0)
            }
        };

        public override List<(double x, double y, double rotation, int state)> Rule(int state) 
        {
            if (state > 2)
                return m_rules[2].Select(t => (-t.x, t.y, -t.rotation, 5 - t.state)).ToList();
            else
                return m_rules[i];
        }

        public override bool Level0IsComposite => true; //because of mirror immages;

        public override void DefineCompositeBasePart(StringBuilder sb, int state)
        {
            switch (state)
            {
                case 0: case 5:
                    sb.AppendLine(new Tile(2, 2).Print(0, 0, 0, ColorMap.Get(m_colors[state]).id));
                    break;
                case 1: case 2:
                    sb.AppendLine(new Shape() { PartID = "35787"}.Print(0, 0, 0, ColorMap.Get(m_colors[state]).id));
                    break;
                case 3: case 4:
                    sb.AppendLine(new Shape() { PartID = "35787" }.Rotate(90).Print(0, 0, 0, ColorMap.Get(m_colors[state]).id));
                    break;
            }
        }

    }
}
