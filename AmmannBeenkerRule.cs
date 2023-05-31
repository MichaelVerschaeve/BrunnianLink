using System.Text;

namespace BrunnianLink
{
    public class AmmannBeenkerRule : SubstitutionRule
    {
        //static readonly string[] m_colors = new string[] { "Magenta", "Magenta", "Medium_Azure" };
        static readonly string[] m_colors = new string[] { "Magenta", "Orange", "Medium_Azure" };

        public override string[] Colors => m_colors;

        public override int StateCount => 3;

        public override bool ColorByState => true;


        static readonly double m_sqrt2p1 = 1 + Math.Sqrt(2.0);
        public override double ScaleFactor => m_sqrt2p1;

        public override double InitialScale => 8 + 2 * Math.Sqrt(2.0);

        public override string MainName => "Ammann_Beenker";

        //private static readonly int[] m_startStates = new int[] { (0,1 };
        public override (double x, double y, double rotation, int state)[] StartStates
        {
            get
            {
                var a = base.StartStates!.Clone() as (double x, double y, double rotation, int state)[];
                var b = base.StartStates!.Clone() as (double x, double y, double rotation, int state)[];
                if (b != null) 
                    b[0].state = 1;
                return a?.Concat(b!)?.ToArray()!;

            }
        }

        public override string BasePart(int state) => state switch
        {
            0 => "TriangleUp",
            1 => "TriangleDown",
            2 => "Rhombus",
            _ => ""
        };


        static readonly List<(double x, double y, double rotation, int state)>[] m_rules = new[]
        {
           new List<(double x, double y, double rotation, int state)> //uppointing, mid hypo
           {
                (0,0,180,1),
                (-m_sqrt2p1*0.5,0.5,-135,0),
                (0.5,m_sqrt2p1*0.5,135,0),
                (-Math.Sqrt(2.0)*0.5,0,90,2),
                (0,Math.Sqrt(2.0)*0.5,0,2),
           },
           new List<(double x, double y, double rotation, int state)> //downpointing, mid hypo
           {
                (0,0,180,0),
                (-m_sqrt2p1*0.5,-0.5,135,1),
                (0.5,-m_sqrt2p1*0.5,-135,1),
                (-Math.Sqrt(2.0)*0.5,0,-45,2),
                (0,-Math.Sqrt(2.0)*0.5,45,2),
           },
           new List<(double x, double y, double rotation, int state)> //rhomb, upperleft \__\
           {
                (0,0,0,2),
                (1+0.5*Math.Sqrt(2.0),-1-0.5*Math.Sqrt(2.0),90,2),
                (1+Math.Sqrt(2.0),-1,0,2),

                (1+0.5*Math.Sqrt(2.0),0,0,1),
                (m_sqrt2p1,-1-0.5*Math.Sqrt(2.0),180,1),

                (0.5*m_sqrt2p1,-0.5*m_sqrt2p1,-45,0),
                (0.5+m_sqrt2p1,-0.5,135,0),
           },
        };

        override public List<(double x, double y, double rotation, int state)> Rule(int state) => m_rules[state];


        private static void DefineQuarter(StringBuilder sb, bool right)
        {
            int color = MetaData.StudIOColor16BugFixed ? 16 : ColorMap.Get(m_colors[0]).id;
            MetaData.StartSubModel(sb, right ? "QuarterRight" : "QuarterLeft");
            Shape wedge = new() { PartID = "15706" };   //from 135 to 180 degree wedge
            int s = right ? -1 : 1;
            sb.AppendLine(wedge.Rotate(right?-135:0).Print(s*3,0,0, color));
            sb.AppendLine(wedge.Rotate(right ?0 : -135).Print(-s*3,0,1, color));
            Plate p = new(2);
            sb.AppendLine(p.Print(s * 2, -0.5, 1, color));
            sb.AppendLine(p.Print(-s * 2, -0.5, 0, color));
        }

        private static void DefineHalfRhomb(StringBuilder sb)
        {
            int color = MetaData.StudIOColor16BugFixed ? 16 : ColorMap.Get(m_colors[2]).id;
            MetaData.StartSubModel(sb, "HalfRhumb");
            sb.AppendLine(new Shape() { PartID = "15706" }.Print(0, 0, 0, color));
            sb.AppendLine(new Shape() { PartID = "2429" }.Rotate(180).Print(-8, 0, 0, color));
            Plate p2 = new(2);
            Plate p8 = new(8);
            sb.AppendLine(p2.Print(-5, -0.5, 0, color));
            sb.AppendLine(p8.Print(-4, -0.5, 1, color));
            double t = 4 * Math.Sqrt(2.0);
            sb.AppendLine(new Shape() { PartID = "2430" }.Rotate(-45).Print(-t, t, 0, color));
            t = Math.Sqrt(2.0) * 0.5;
            sb.AppendLine(p2.Rotate(-45).Print(-4.5*t, 5.5*t, 0, color));
            sb.AppendLine(p8.Rotate(-45).Print(-3.5*t, 4.5*t, 1, color));
        }

        public override bool Level0IsComposite => true;

        public override void DefineCompositeBasePart(StringBuilder sb, int state)
        {
            int color = MetaData.StudIOColor16BugFixed ? 16 : ColorMap.Get(m_colors[state==1?0:state]).id;
            switch (state)
            {
                case 0:
                    Shape downTriangle = new() { PartID = BasePart(1), SubModel = true };
                    sb.AppendLine(downTriangle.Rotate(180).Print(0, 0, 0, color));
                    break;
                case 1:
                    Shape quarter = new() { PartID = "QuarterLeft", SubModel = true };
                    double t = 1.0 + 1.5 * Math.Sqrt(2.0);
                    sb.AppendLine(quarter.Rotate(-45).Print(-t, -t, 0, color));
                    quarter = new() { PartID = "QuarterRight", SubModel = true };
                    sb.AppendLine(quarter.Rotate(45).Print(t, -t, 0, color));
                    Plate plate = new(4);
                    t = -1 + 3 * Math.Sqrt(2.0);
                    sb.AppendLine(plate.Print(-t, -0.5, 0, color));
                    sb.AppendLine(plate.Print(t, -0.5, 0, color));
                    sb.AppendLine(new Plate(2, 4).Print(0, -t, 1, color));
                    DefineQuarter(sb, false);
                    DefineQuarter(sb, true);
                    break;
                case 2:
                    Shape HalfRumb = new() { PartID = "HalfRhumb", SubModel = true };
                    double x = 1 + Math.Sqrt(2.0);
                    double y = -1;
                    sb.AppendLine(HalfRumb.Rotate(180).Print(x,y, 0, color));
                    x += 8 + 4 * Math.Sqrt(2.0);
                    y -= 4 * Math.Sqrt(2.0);
                    sb.AppendLine(HalfRumb.Print(x,y, 0, color));
                    DefineHalfRhomb(sb);
                    break;

            }
        }
    }
}
