using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    public class PenroseRhombRule : SubstitutionRule
    {
        private static string[] m_colors = { "Red", "Yellow" };
        public override string[] Colors => m_colors;

        public override int StateCount => 2;

        static readonly double phi = (1+Math.Sqrt(5.0))*0.5;

        public override double ScaleFactor => phi;

        public override double InitialScale => 16;

        public override string MainName => "PenroseRhomb";

        public override string BasePart(int state)
        {
            return state == 0 ? "Thick" : "Thin";
        }

        public override bool ColorByState => true;
        public override bool Level0IsComposite => true;

        static readonly Dictionary<int, double> c = new();
        static readonly Dictionary<int, double> s = new();



        private static List<(double x, double y, double rotation, int state)>[] m_rules;
        static PenroseRhombRule()
        {
            for (int i = 0; i < 10; i++)
            {
                c.Add(i * 18, Math.Cos(i * Math.PI / 10.0));
                s.Add(i * 18, Math.Sin(i * Math.PI / 10.0));

            }
            m_rules = new[]
            {
                new List<(double x, double y, double rotation, int state)> //thick longest side vertical, origin at bottom
                {
                    (0, 1 + 2 * c[36], 180, 0),
                    (c[18], 1 + s[18], 180 - 36, 0),
                    (c[18], 1 + s[18], 36, 1)
                },
                new List<(double x, double y, double rotation, int state)> //thin longest side horizontal, origin at bottom
                {
                    (0, 1, 108, 1),
                    (c[18] + c[72], -s[18] + s[72], 108, 0)
                }
            };
        }

        public override List<(double x, double y, double rotation, int state)> Rule(int state)
        {
            return m_rules[state];
        }

        public override void DefineCompositeBasePart(StringBuilder sb, int state)
        {
            Shape liftarm = new() { PartID = "40490" };

            if (state == 0)
            {
                int color = ColorMap.Get(m_colors[0]).id;
                double x = 7 * c[72] - 0.5 * c[18];
                double y = 7 * s[72] + 0.5 * s[18];
                sb.AppendLine(liftarm.Print(x, y, 18, color));
                sb.AppendLine(liftarm.Print(-x, y, -18, color));
                sb.AppendLine(liftarm.Print(x, -y, -18, color));
                sb.AppendLine(liftarm.Print(-x, -y, 18, color));
            }
            else
            {
                int color = ColorMap.Get(m_colors[0]).id;
                double x = 7 * c[18] - 0.5 * c[72];
                double y = 7 * s[18] - 0.5 * s[72];
                sb.AppendLine(liftarm.Print(x, y, 72, color));
                sb.AppendLine(liftarm.Print(-x, y, -72, color));
                sb.AppendLine(liftarm.Print(x, -y, -72, color));
                sb.AppendLine(liftarm.Print(-x, -y, 72, color));
            }
        }
    }
}
