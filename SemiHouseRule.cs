using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    public class SemiHouseRule : SubstitutionRule
    {
        private readonly string[] m_colors = new[] { "Dark_Red", "Medium_Blue", "Medium_Blue", "Dark_Red" };
        public override string[] Colors => m_colors;
        public override bool ColorByState => true;

        public override int StateCount => 4;

        public override double ScaleFactor => Math.Sqrt(2.0);

        public override double InitialScale => 2;

        public override string MainName => "Semi_Detached_House";

        public override string BasePart(int state, int color) => state switch { 0 => "SmallHouse", 1 => "BigHouse", 2 => "BigHouse_mirror", 3 => "SmallHouse_mirror", _ => "" };


        static readonly List<(double x, double y, double rotation, int state)>[] m_rules = new[]
        {
           new List<(double x, double y, double rotation, int state)>
           {
                (0.0,0.0,0.0,1)
           },
           new List<(double x, double y, double rotation, int state)>
           {
              (0,0,0,0),
              (2.0,0,0,3),
              (2.0,2.0,45,2)
           }
        };
        public override List<(double x, double y, double rotation , int state)> Rule(int state) => state > 1 ? Rule(3 - state).Select(t => (-t.x, t.y, -t.rotation, 3 - t.state)).ToList() : m_rules[state];

        public override bool Level0IsComposite => true;

        public override void DefineCompositeBasePart(StringBuilder sb, int state, int color)
        {
            int colorId = ColorMap.Get(Colors[state]).id;
            Shape tria = new() { PartID = "35787" };
            int s = 1;
            if (state > 1) 
            {
                s = -1;
                state = 3 - state;
                tria.RotateThis(90);
            }
            if (state == 0)
            {
                sb.AppendLine(tria.Print(s, 3, 0, colorId));
                sb.AppendLine(new Tile(2, 2).Print(s, 1, 0, colorId));
            }
            else
            {
                double t = Math.Sqrt(2.0);
                Shape tria2 = tria.Rotate(s * 135);
                Shape tria3 = tria.Rotate(s * -45);
                Shape tria4 = tria.Rotate(-s * 135);
                sb.AppendLine(tria2.Print(0, t, 0, colorId));
                sb.AppendLine(tria2.Print(0, 3 * t, 0, colorId));
                sb.AppendLine(tria3.Print(2 * s * t, t, 0, colorId));
                sb.AppendLine(tria4.Print(s * t, 0, 0, colorId));
                sb.AppendLine(new Tile(2, 2).Rotate(s * 45).Print(s * t, 2 * t, 0, colorId));

            }

        }
    }
}
