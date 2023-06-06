namespace BrunnianLink
{
    public class DominoRule : SubstitutionRule
    {
        static readonly string[] m_colors = new string[] { "Light_Bluish_Grey", "Tan", "Dark_Tan", "Dark_Bluish_Grey" };
        override public string[] Colors => m_colors;

        override public int StateCount => 1;

        override public double ScaleFactor => 2;
        override public double InitialScale => 1;

        override public string MainName => "TennisTiling";

        override public string BasePart(int state, int color)=> Plate.XYPartID(1, 2); 

        static readonly List<(double x, double y, double rotation, int state)> m_rule = new()
            {
                (-1.5,0,90.0,0),
                (0,0.5,0.0,0),
                (0,-0.5,0.0,0),
                (1.5,0,90.0,0)
            };

        override public List<(double x, double y, double rotation, int state)> Rule(int _) => m_rule;
    }
}
