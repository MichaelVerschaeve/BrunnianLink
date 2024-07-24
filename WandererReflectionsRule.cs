using System.Text;

namespace BrunnianLink
{
    public class WandererReflectionsRule : SubstitutionRule
    {
        static readonly string[] m_colors = new string[] { "Sand_Blue", "Dark_Pink" };
        override public string[] Colors => m_colors;
        override public int StateCount => 2;

        override public double ScaleFactor => 2;
        override public double InitialScale => 1;

        override public string MainName => "Wanderer_reflections";
        override public string BasePart(int state, int color)
        {
            return state==0?"LeftHanded":"RightHanded";
        }

        public override bool Level0IsComposite => true;

        public override void DefineCompositeBasePart(StringBuilder sb, int state, int color)
        {
            Tile t = new(2);
            sb.AppendLine(t.Print(0, 0.5, 0, ColorMap.Get(m_colors[state]).id));
            sb.AppendLine(t.Print(0, -0.5, 0, ColorMap.Get(m_colors[1-state]).id));
        }


        static readonly List<(double x, double y, double rotation, int state)>[] m_rules = new[]
        {
           new List<(double x, double y, double rotation, int state)>
           {
                (-1,1,-90.0,0),
                (-1,-1,90.0,1),
                (1,1,0,0),
                (1,-1,0,1),
           },
           new List<(double x, double y, double rotation, int state)>
           {
              (-1,1,0,1),
              (-1,-1,0,0),
              (1,1,90,1),
              (1,-1,-90,0),
           }
        };

        override public List<(double x, double y, double rotation, int state)> Rule(int state) => m_rules[state];
    }
}
