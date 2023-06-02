using System.Text;

namespace BrunnianLink
{
    public class WandererRotationsRule : SubstitutionRule
    {
        static readonly string[] m_colors = new string[] { "Dark_Blue", "Tan" };
        override public string[] Colors => m_colors;
        override public int StateCount => 4;

        override public double ScaleFactor => 2;
        override public double InitialScale => 1;

        override public string MainName => "Wanderer_rotations";

        override public string BasePart(int state, int color)
        {
            bool vert = (state & 1) == 1;
            bool right = (state & 2) == 2;
            return (vert ? "Vertical" : "Horizontal") + (right?"Right":"Left");
        }

        public override bool Level0IsComposite => true;

        public override void DefineCompositeBasePart(StringBuilder sb, int state, int color)
        {
            MetaData.StartSubModel(sb, BasePart(state,0));
            bool vert = (state & 1) == 1;
            bool right = (state & 2) == 2;
            bool xor = vert ^ right;
            Tile t = new(1, 2);
            sb.AppendLine(t.Print(0.5, 0, 0, ColorMap.Get(m_colors[xor ? 1 : 0]).id));
            sb.AppendLine(t.Print(-0.5, 0, 0, ColorMap.Get(m_colors[xor ? 0 : 1]).id));

        }

        override public List<(double x, double y, double rotation, int state)> Rule(int state)
        {
            bool vert = (state & 1) == 1;
            bool right = (state & 2) == 2;
            var toState = (bool v, bool r)=>(v ? 1 : 0) + (r ? 2 : 0);
            int x = right ? -1 : 1;
            double angle = right ? -90.0 : 90.0;
            return new List<(double x, double y, double rotation, int state)> 
            {
              (x,1,0,toState(vert,right)),
              (-x,1,-angle,toState(!vert,right)),
              (x,-1,0,toState(vert,!right)),
              (-x,-1,angle,toState(! vert,! right)),
            };
        }
    }
}
