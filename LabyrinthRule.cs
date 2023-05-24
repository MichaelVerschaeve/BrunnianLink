using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace BrunnianLink
{
    public class LabyrinthRule : SubstitutionRule
    {
        static private readonly string[] m_colors = new[] { "Dark_Pink", "Purple", "Aqua" };
        public override string[] Colors => m_colors;

        public override int StateCount => 3;

        static readonly double m_sqrt2p1 = 1 + Math.Sqrt(2.0);

        public override double ScaleFactor => m_sqrt2p1;


        public override string MainName => "Labyrinth";

        public override string BasePart(int state) => state switch
        {
            0 => "Square",
            1 => "Corner",
            2 => "Trapezium",
            _ => ""
        };

        static readonly double scale = 2.0 / (1+Math.Sqrt(2.0)); //total size maps to 2, so center is (0,0) and corners are (+/-1,+/-1)
        static readonly double trapHeight = scale * 0.5 * Math.Sqrt(2.0);
        static readonly double squareHeight = 2 - 2 * trapHeight;
        public override double InitialScale => (8 + 2 * Math.Sqrt(2.0))/squareHeight;

        List<(double x, double y, double rotation, int state)>[] m_rules = new[]
        {
            new List<(double x, double y, double rotation, int state)> //square, centered
            {
                (-1,1,-90,1),
                (1,1,180,1),
                (-1,-1,0,1),
                (1,-1,90,1),
                (0,1,0,2),
                (-1,0,90,2),
                (1,0,-90,2),
                (0,-1,180,2),
                (0,0,0,0)
            },
            new List<(double x, double y, double rotation, int state)> //corner, bottom left
            {
                (0,0,0,1),
                (2,0,90,1),
                (0,2,-90,1),
                (0,1,90,2),
                (1,0,180,2),
                (1,1,0,0)
            },
            new List<(double x, double y, double rotation, int state)> //trapezium, top center, smallest base at top
            {
                (0,-0.5*squareHeight,0,0),
                (-1,-squareHeight-trapHeight,0,1),
                (1,-squareHeight-trapHeight,90,1),
                (0,-squareHeight-trapHeight,180,2)
            }
        };

        public override List<(double x, double y, double rotation, int state)> Rule(int state) => m_rules[state];

        public override bool ColorByState => true;

        public override bool Level0IsComposite => true;

        public override int[] StartStates => new int[] { 0 };

        private static string QuarterId(bool right) => "Quarter" + (right ? "Right" : "Left");

        private static void DefineQuarter(StringBuilder sb, bool right)
        {
            int color = MetaData.StudIOColor16BugFixed ? 16 : ColorMap.Get(m_colors[0]).id;
            MetaData.StartSubModel(sb, QuarterId(right));
            Shape wedge = new() { PartID = "15706" };   //from 135 to 180 degree wedge
            int s = right ? -1 : 1;
            sb.AppendLine(wedge.Rotate(right ? -135 : 0).Print(s * 3, 0, 0, color));
            sb.AppendLine(wedge.Rotate(right ? 0 : -135).Print(-s * 3, 0, 1, color));
            Plate p = new(2);
            sb.AppendLine(p.Print(s * 2, -0.5, 1, color));
            sb.AppendLine(p.Print(-s * 2, -0.5, 0, color));
        }


        public override void DefineCompositeBasePart(StringBuilder sb, int state)
        {
            int color = MetaData.StudIOColor16BugFixed ? 16 : ColorMap.Get(m_colors[state]).id;
            int swivelColor = ColorMap.Get("Green").id;
            Shape wedge = new() { PartID = "15706" };   //from 135 to 180 degree wedge
            Shape swivelTop = new() { PartID = "2430" };
            Shape swivelBottom = new() { PartID = "2429" };
            double c = Math.Sqrt(2.0) * 0.5;
            switch (state)
            {
                case 0: //square
                    double t = 3 + Math.Sqrt(2.0);
                    Shape quarter = new() { PartID = QuarterId(false), SubModel = true };
                    sb.AppendLine(quarter.Print(0, -t, 0, color));
                    sb.AppendLine(quarter.Rotate(180).Print(0, t, 0, color));
                    quarter = new() { PartID = QuarterId(true), SubModel = true };
                    sb.AppendLine(quarter.Rotate(-90).Print(-t, 0, 0, color));
                    sb.AppendLine(quarter.Rotate(90).Print(t, 0, 0, color));
                    t -= 1.5 * Math.Sqrt(2.0);
                    Shape p = new Plate(4, 2).Rotate(45);
                    sb.AppendLine(p.Print(-t, -t, 0, color));
                    sb.AppendLine(p.Print(t, t, 0, color));
                    p = new Plate(4, 2).Rotate(-45);
                    sb.AppendLine(p.Print(-t, t, 1, color));
                    sb.AppendLine(p.Print(t, -t, 1, color));
                    DefineQuarter(sb, false);
                    DefineQuarter(sb, true);
                    break;
                case 1: //corner
                    Shape halfCorner = new() { PartID = "halfCorner", SubModel = true };
                    sb.AppendLine(halfCorner.Print(0, 0, 0, color));
                    sb.AppendLine(halfCorner.Rotate(45).Print(0, 0, 0, color));
                    MetaData.StartSubModel(sb, "halfCorner");
                    double offset = 1 + Math.Sqrt(2.0);
                    sb.AppendLine(wedge.Rotate(-135).Print(offset, 1, 0, color));
                    sb.AppendLine(new Plate(4).Print(offset + 6, 0.5, 0, color));
                    sb.AppendLine(swivelTop.Rotate(180).Print(offset + 8, 1, -1, swivelColor));
                    sb.AppendLine(new Plate(8).Print(offset + 4, 0.5, 1, color));
                    sb.AppendLine(new Plate(4).Rotate(45).Print(offset + 5.5 * c, offset + 4.5 * c, 0, color));
                    sb.AppendLine(new Plate(8).Rotate(45).Print(offset + 3.5 * c, offset + 2.5 * c, 1, color));
                    sb.AppendLine(swivelBottom.Rotate(45).Print(offset + 8 * c, offset + 6 * c, -1, swivelColor));
                    break;
                case 2: //trapezium
                    double yoffset = -trapHeight * InitialScale;
                    sb.AppendLine(wedge.Rotate(-135).Print(-3, yoffset + 1, 0, color));
                    sb.AppendLine(wedge.Print(3, yoffset + 1, 1, color));
                    sb.AppendLine(new Plate(4).Print(-3, yoffset + 0.5, 1, color));
                    sb.AppendLine(new Plate(4).Print(3, yoffset + 0.5, 0, color));
                    sb.AppendLine(new Plate(2).Print(-4, yoffset + 0.5, 0, color));
                    sb.AppendLine(new Plate(2).Print(4, yoffset + 0.5, 1, color));
                    sb.AppendLine(swivelTop.Rotate(180).Print(5, yoffset + 1, -1, swivelColor));
                    sb.AppendLine(swivelBottom.Rotate(180).Print(-5, yoffset + 1, -1, swivelColor));
                    sb.AppendLine(new Plate(8).Rotate(45).Print(-3 + 3.5 * c, yoffset + 1 + 4.5 * c, 1, color));
                    sb.AppendLine(new Plate(3).Rotate(-45).Print(3 - c, yoffset + 1 + 2 * c, 0, color));
                    sb.AppendLine(new Shape() { PartID = "92593" }.Rotate(-45).Print(3 - 4.5 * c, yoffset + 1 + 5.5 * c, 0, color));
                    sb.AppendLine(new Plate(1).Rotate(-45).Print(3 - 7 * c, yoffset + 1 + 8 * c, 0, color));
                    sb.AppendLine(new Plate(2).Rotate(-45).Print(3 - 6.5 * c, yoffset + 1 + 7.5 * c, 1, color));
                    sb.AppendLine(new Plate(2).Rotate(45).Print(-3 + 6.5 * c, yoffset + 1 + 7.5 * c, 0, color));
                    sb.AppendLine(swivelTop.Rotate(-45).Print(3 - 8 * c, yoffset + 1 + 8 * c, -1, swivelColor));
                    sb.AppendLine(swivelBottom.Rotate(45).Print(-3 + 8 * c, yoffset + 1 + 8 * c, -1, swivelColor));
                    break;
            }

        }
    }
}
