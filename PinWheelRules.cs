namespace BrunnianLink
{
    public class PinWheelRules : SubstitutionRule
    {
        private static readonly double acuteAngleRad = Math.Atan(0.5);
        private static readonly double acuteAngleDeg = acuteAngleRad * 180 / Math.PI;
        private static readonly double scaleFactor = Math.Sqrt(5);

        static readonly string[] m_colors = new string[] { "Lime", "Light_Bluish_Grey", "Medium_Azure", "Red", "Dark_Blue" };
        override public string[] Colors => m_colors;

        override public string MainName => "PinWheel";

        override public int StateCount => 2;
        override public double ScaleFactor => scaleFactor;
        override public double InitialScale => scaleFactor;

        override public string BasePart(int state)
        {
            return state==1 ? "65429" : "65426";
        }

        override public List<(double x, double y, double rotation, int state)> Rule(int state)
        {
            int sign = state == 1 ? -1 : 1;
            double angle = acuteAngleDeg * sign;
            double x = sign * 0.2;
            return new List<(double x, double y, double rotation, int state)>
            {
                (0,2,sign*90+angle,1-state),
                (sign,1,angle,1-state),
                (x,0.6,angle,state),
                (x,0.6,180+angle,state),
                (sign,-1,angle,1-state)
            };
        }
    }
}
