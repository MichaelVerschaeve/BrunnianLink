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
            0 => "T1", 
            1 => "T2", 
            2 => "T3",
            3 => "T3_m",
            4 => "T2_m",
            5 => "T1_m",
            _ => "" };

        static private readonly List<(double x, double y, double rotation, int state)>[] m_rules = new[]
        {
            new List<(double x, double y, double rotation, int state)>()
            {
                (-1,1,0,0),
                (-1,-1,-90,4),
                (-1,-1,90,3),
                (1,-1,-90,3),
            },
            new List<(double x, double y, double rotation, int state)>()
            {
                (-1,1,0,1),
                (-1,-1,-90,0),
                (-1,-1,0,5),
                (1,-1,0,1),
            },
            new List<(double x, double y, double rotation, int state)>()
            {
                (-1,1,0,2),
                (-1,-1,0,1),
                (-1,-1,180,2),
                (1,-1,0,1),
            }
        };


        static private readonly (double x, double y, double rotation, int state)[] m_startStates = new[] {(0.0,0.0,0.0,0), (0.0,0.0,90.0,5)  };

        public override (double x, double y, double rotation, int state)[] StartStates => m_startStates;

        public override List<(double x, double y, double rotation, int state)> Rule(int state) 
        {
            if (state > 2)
                return m_rules[5-state].Select(t => (-t.x, t.y, -t.rotation, 5 - t.state)).ToList();
            else
                return m_rules[state];
        }

        public override bool Level0IsComposite => true; //because of mirror immages;

        public override void DefineCompositeBasePart(StringBuilder sb, int state)
        {
                if (state < 3)
                    sb.AppendLine(new Shape() { PartID = "35787"}.Print(0, 0, 0, ColorMap.Get(m_colors[state]).id));
                else 
                sb.AppendLine(new Shape() { PartID = "35787" }.Rotate(90).Print(0, 0, 0, ColorMap.Get(Colors[state]).id));
            
        }

    }
}
