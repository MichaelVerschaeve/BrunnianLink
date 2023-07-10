namespace BrunnianLink
{
    public class ChairRule : SubstitutionRule
    {
        static readonly string[] m_colors = new string[] { "Light_Bluish_Grey", "Tan", "Dark_Tan", "Dark_Bluish_Grey" };
        override public string[] Colors => m_colors;

        override public int StateCount => 1;

        override public double ScaleFactor => 2;
        override public double InitialScale => 1;

        override public string MainName => "ChairTiling";

        override public string BasePart(int state, int color)
        {
            return "2420";
        }

        static readonly List<(double x, double y, double rotation, int state)> m_rule = new ()
            {
                (-0.5,2.5,-90.0,0),
                (0.5,0.5,0.0,0),
                (-0.5,-0.5,0.0,0),
                (2.5,-0.5,90.0,0)
            };

        override public List<(double x, double y, double rotation, int state)> Rule(int _) => m_rule;

    }
}
