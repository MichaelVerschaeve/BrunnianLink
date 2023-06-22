using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace BrunnianLink
{
    public class PinWheel10 : SubstitutionRule
    {
        private readonly string[] m_colors = new[] {"White","Red","Dark_Tan","Orange","Yellow","Lime","Dark_Green","Blue","Sand_Blue","Dark_Pink"};

        public override string[] Colors => m_colors;

        public override int StateCount => 2;

        public override double ScaleFactor => Math.Sqrt(10);

        public override double InitialScale => 3;

        public override string MainName => "PinWheel10";

        public override string BasePart(int state, int color) => (state switch { 0 => "Right", _ => "Left" })+$"_{state}_{color}" ;

        public override bool Level0IsComposite => true;
        public override void DefineCompositeBasePart(StringBuilder sb, int state, int color)
        {
            Shape s = new Shape() { PartID = state == 0 ? "54383" : "54384" }.Rotate(180);
            int sign = state == 0 ? 1 : -1;
            sb.AppendLine(s.Print(1.5 * sign, 3, 0, ColorMap.Get(Colors[color]).id));
        }


        private readonly (int x, int y, int r)[] rights = new[]
        {(1,0,0),(2,0,0),
         (2,1,2),(3,1,2),(2,1,0),
         (3,2,2)
        };

        private readonly (int x, int y, int r)[] lefts = new[]
        {(3,0,1),(1,0,0),(2,1,0),(3,2,0)};


        public override List<(double x, double y, double rotation, int state)> Rule(int state)
        {
            double c = 3.0/Math.Sqrt(10.0);
            double s = 1.0/Math.Sqrt(10.0);
            double d = 180*Math.Atan2(1.0, 3.0)/Math.PI;
            if (state == 0)
                return rights.Select(t=>(c*t.x-s*t.y*3.0,s*t.x+c*t.y*3.0,t.r*90+d,0)).Concat(lefts.Select(t => (c * t.x - s * t.y * 3.0, s * t.x + c * t.y * 3.0, t.r * 90 + d, 1))).ToList();
            else
                return rights.Select(t => (-c * t.x + s * t.y * 3.0, s * t.x + c * t.y * 3.0, - t.r * 90 - d, 1)).Concat(lefts.Select(t => (-c * t.x + s * t.y * 3.0, s * t.x + c * t.y * 3.0, -t.r * 90 - d, 0))).ToList();

        }
    }
}
