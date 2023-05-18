using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    public class PenroseRhomb : SubstitutionRule
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

        static readonly double c36 = phi * 0.5;
        static readonly double c18 = Math.Cos(Math.PI/10);
        static readonly double s18 = (Math.Sqrt(5.0)-1) * 0.25;


        private static List<(double x, double y, double rotation, int state)>[] m_rules = new[]
        {
           new List<(double x, double y, double rotation, int state)> //thick longest side vertical, origin at bottom
           {
                (0,1+2*c36,180,0),
                (c18,1+s18,180-36,0),
                (c18,1+s18,36,1),
           },
           new List<(double x, double y, double rotation, int state)> //tin longest side horizontal, origin at bottom
           {
               (0,0,108,1)
           }
        };

        public override List<(double x, double y, double rotation, int state)> Rule(int state)
        {
            return m_rules[state];
        }

        public override void DefineCompositeBasePart(StringBuilder sb, int state)
        {
            base.DefineCompositeBasePart(sb, state);
        }
    }
}
