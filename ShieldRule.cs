using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    public class ShieldRule : SubstitutionRule
    {
        private static readonly string[] m_colors = new[] { "Yellow", "Yellow", "Orange", "DarkBlue"};
        public override string[] Colors => m_colors.Concat(m_colors.Reverse()).ToArray();

        public override int StateCount => 8;

        private static readonly double m_scaleFactor = (1.0 + Math.Sqrt(3.0)) / Math.Sqrt(2.0);
        public override double ScaleFactor => m_scaleFactor;

        public override double InitialScale => 6;

        public override string MainName => "ShieldTiling";

        private static readonly string[] m_baseParts = new[] { "Triangle1", "Triangle2", "Square", "Shield" };
        public override string BasePart(int state)
        {
            return (state >= 4) ? (m_baseParts[7-state] + "_mirror") : m_baseParts[state];
        }

        //state 0: triangle type one, center at top
        //state 1: triangle type two, center at top
        //state 2: square, center bottom left...
        //state 3 shield: center at bottom.
        // state 7-x, mirror of x
        private static readonly List<(double x, double y, double rotation, int state)>[] m_rules = new[]
        {
            new List<(double x, double y, double rotation, int state)> 
            { 
                (0,0,180,3)
            },
            new List<(double x, double y, double rotation, int state)>
            {
                (0,0,-135,2),
                (0,-Math.Sqrt(2),135,1),
                (0,-Math.Sqrt(2),-135,6),
            },
            new List<(double x, double y, double rotation, int state)>
            {
                ((m_scaleFactor+Math.Sqrt(2.0))*0.5,m_scaleFactor*0.5,45,4),
                ((m_scaleFactor-Math.Sqrt(2.0))*0.5,m_scaleFactor*0.5,15,0),
                (m_scaleFactor*0.5,(m_scaleFactor-Math.Sqrt(2.0))*0.5,105,0),
                ((m_scaleFactor-Math.Sqrt(2.0))*0.5,m_scaleFactor*0.5,165,7),
                (m_scaleFactor*0.5,(m_scaleFactor+Math.Sqrt(2.0))*0.5,75,7),
            },
            new List<(double x, double y, double rotation, int state)>
            {
                (0,1.0+Math.Sqrt(3.0),0,7), //center triangle
                (-0.5,1.0+Math.Sqrt(3.0)*0.5,-30,5), //left top square
                (0.5,1.0+Math.Sqrt(3.0)*0.5,30,2), //right top square
                (0.5,1.0+Math.Sqrt(3.0)*0.5,90,5), //bottom square
                (-0.5,Math.Sqrt(3.0)*0.5,60,0), //bottom outer triangle
                (-Math.Sqrt(3.0),1.5+Math.Sqrt(3.0),-60,0), //left outer triangle
                (Math.Sqrt(3.0),1.5+Math.Sqrt(3.0),60,7), //right outer triangle
                (-0.5*Math.Sqrt(3.0),2+Math.Sqrt(3.0),90,7), //duo on top, left triangle
                (0.5*Math.Sqrt(3.0),2+Math.Sqrt(3.0),-90,1), //duo on top, right,triangle
                (-0.5*(1+Math.Sqrt(3.0)),1.5+0.5*Math.Sqrt(3.0),30,0), //duo on left, top triangle
                (-0.5,0.5*Math.Sqrt(3.0),-150,6), //duo on left, bottom triangle
                (0.5*(1+Math.Sqrt(3.0)),1.5+0.5*Math.Sqrt(3.0),-30,7), //duo on right, top triangle
                (0.5,0.5*Math.Sqrt(3.0),150,1), //duo on right, bottom triangle
            }
        };

        public override List<(double x, double y, double rotation, int state)> Rule(int state)
        {
            if (state >= 4)
                return m_rules[7-state].Select(((double x, double y, double rotation, int state) t) => (-t.x, t.y, -t.rotation, 7 - state)).ToList();
            else
                return m_rules[state];
        }

        public override bool ColorByState => true;

        public override bool Level0IsComposite => true;

        public override int[] StartStates => new int[] {3};

        public override void DefineCompositeBasePart(StringBuilder sb, int state)
        {
            switch (state)
            {
                case 0: case 1: case 6: case 7: //triangle...
                    break;
                case 2: case 5: //square
                    sb.AppendLine(new Plate(6, 6).Print(0, 0, 0, ColorMap.Get(Colors[state]).id));
                    break;
                default: //shield
                    break;
            }
        }
    }
}
