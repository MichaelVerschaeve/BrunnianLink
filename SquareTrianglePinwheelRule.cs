using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    public class SquareTrianglePinwheelRule : SubstitutionRule
    {
        string[] m_colors = new[] { "Aqua", "Dark_Red", "Dark_Red", "Aqua" };
        public override string[] Colors => m_colors;

        public override int StateCount => 4;

        public override double ScaleFactor => Math.Sqrt(5);

        public override double InitialScale => 0.5 * Math.Sqrt(5);

        public override string MainName => throw new NotImplementedException();

        public override string BasePart(int state, int color)
        {
            throw new NotImplementedException();
        }

        public override List<(double x, double y, double rotation, int state)> Rule(int state)
        {
            throw new NotImplementedException();
        }
    }
}
