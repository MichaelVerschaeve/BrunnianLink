using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    public class SpectreRule : SubstitutionRule
    {
        string[] m_colors = new[] { "Black", "Red", "Light_Bluish_Grey", "Green", "Dark_Bluish_Grey", "Blue", "Black", "Yellow" };
        public override string[] Colors => m_colors;

        public override int StateCount => 4;


        private static double X (IEnumerable<int> degrees)
        {
            degrees = degrees.Select(d => (d % 360 + 360) % 360);
            return 0.5*(degrees.Count(d=> d== 60 || d==300)- degrees.Count(d => d == 120 || d == 240)
                + Math.Sqrt(3.0)*(degrees.Count(d => d == 30 || d == 330) - degrees.Count(d => d == 150 || d == 210)));
        }

        private static double Y(IEnumerable<int> degrees)
        {
            return X(degrees.Select(d => 90 - d));
        }

        public override double ScaleFactor => 4;

        public override double InitialScale => 6;

        public override string MainName => "SpectreTiling";

        public override bool ColorByState => false; //we'll exploit the mod technic to get 

        public override bool Level0IsComposite => true;



        //private static string Color
        //{


        //}


        public override string BasePart(int state, int color)
        {
            throw new NotImplementedException();
        }

        public override List<(double x, double y, double rotation, int state)> Rule(int state)
        {
            throw new NotImplementedException();
        }

        public override void DefineCompositeBasePart(StringBuilder sb, int state, int color)
        {
            base.DefineCompositeBasePart(sb, state, color);
        }

    }
}
