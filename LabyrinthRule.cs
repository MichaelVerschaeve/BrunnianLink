using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    public class LabyrinthRule : SubstitutionRule
    {
        static private readonly string[] m_colors = new[] { "Lavender", "Dark_Pink", "Aqua" };
        public override string[] Colors => m_colors;

        public override int StateCount => 3;

        public override double ScaleFactor => 1+Math.Sqrt(2.0);

        public override double InitialScale => 7;

        public override string MainName => "Labyrinth";


        static private readonly string[] m_baseParts = new[] { "Square", "Corner", "Trapezium" };
        public override string BasePart(int state) => m_baseParts[state];

        List<(double x, double y, double rotation, int state)>[] m_rules = new[]
        {
            new List<(double x, double y, double rotation, int state)> //square, centered
            {
                (-1,1,-90,1),
                (1,1,180,1),
                (-1,-1,0,1),
                (1,-1,90,1),
                (0,1,0,2),
                (-1,0,90,2),
                (1,0,-90,2),
                (0,-1,180,2),
                (0,0,0,0)
            },
            new List<(double x, double y, double rotation, int state)> //corner, bottom left
            {
                (0,0,0,1),
                (2,0,90,1),
                (0,2,-90,1),
                (0,1,90,2),
                (1,0,180,2),
                (1,1,0,0)
            },
            new List<(double x, double y, double rotation, int state)> //corner, bottom left
            {
                (0,0,0,1)


            }
        };

        public override List<(double x, double y, double rotation, int state)> Rule(int state) => m_rules[state];

        public override bool ColorByState => true;

        public override bool Level0IsComposite => true;

        public override void DefineCompositeBasePart(StringBuilder sb, int state)
        {
            //base.DefineCompositeBasePart(sb, state);
        }

    }
}
