using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    public class ShieldRule : SubstitutionRule
    {
        private static readonly string[] m_colors = new[] { "Yellow", "Yellow", "Orange", "Dark_Blue"};
        public override string[] Colors => m_colors.Concat(m_colors.Reverse()).ToArray();

        public override int StateCount => 8;

        private static readonly double m_scaleFactor = (1.0 + Math.Sqrt(3.0)) / Math.Sqrt(2.0);
        public override double ScaleFactor => m_scaleFactor;

        public override double InitialScale => 6;

        public override string MainName => "ShieldTiling";

        private static readonly string[] m_baseParts = new[] { "Triangle1", "Triangle2", "Square", "Shield", "Shield_mirror", "Square_mirror", "Triangle2_mirror", "Triangle1_mirror" };
        public override string BasePart(int state)
        {
            return m_baseParts[state];
        }

        //state 0: triangle type one, center at top
        //state 1: triangle type two, center at top
        //state 2: square, center bottom left...
        //state 3 shield: center at bottom.
        // state 7-x, mirror of x
        private static readonly List<(double x, double y, double rotation, int state)>[] m_rules = new[]
        {
            new List<(double x, double y, double rotation, int state)> 
            { 
                (-m_scaleFactor*0.5,-m_scaleFactor*Math.Sqrt(3.0)*0.5, -60,3)
            },
            new List<(double x, double y, double rotation, int state)>
            {
                (0,0,-135,2),
                (0,-Math.Sqrt(2),105,1),
                (0,-Math.Sqrt(2),-105,6),
            },
            new List<(double x, double y, double rotation, int state)>
            {
                ((m_scaleFactor+Math.Sqrt(2.0))*0.5,m_scaleFactor*0.5,45,5),
                ((m_scaleFactor-Math.Sqrt(2.0))*0.5,m_scaleFactor*0.5,15,0),
                (m_scaleFactor*0.5,(m_scaleFactor-Math.Sqrt(2.0))*0.5,105,0),
                ((m_scaleFactor-Math.Sqrt(2.0))*0.5,m_scaleFactor*0.5,165,7),
                (m_scaleFactor*0.5,(m_scaleFactor+Math.Sqrt(2.0))*0.5,75,7),
            },
            new List<(double x, double y, double rotation, int state)>
            {
                (0,1.0+Math.Sqrt(3.0),0,7), //center triangle
                (-0.5,1.0+Math.Sqrt(3.0)*0.5,-30,5), //left top square
                (0.5,1.0+Math.Sqrt(3.0)*0.5,30,2), //right top square
                (0.5,1.0+Math.Sqrt(3.0)*0.5,90,5), //bottom square
                (-0.5,Math.Sqrt(3.0)*0.5,60,0), //bottom outer triangle
                (-Math.Sqrt(3.0)*0.5,1.5+Math.Sqrt(3.0),-60,0), //left outer triangle
                (Math.Sqrt(3.0)*0.5,1.5+Math.Sqrt(3.0),60,7), //right outer triangle
                (-0.5*Math.Sqrt(3.0),1.5+Math.Sqrt(3.0),90,7), //duo on top, left triangle
                (0.5*Math.Sqrt(3.0),1.5+Math.Sqrt(3.0),-90,1), //duo on top, right,triangle
                (-0.5*(1+Math.Sqrt(3.0)),1.5+0.5*Math.Sqrt(3.0),30,0), //duo on left, top triangle
                (-0.5,0.5*Math.Sqrt(3.0),-150,6), //duo on left, bottom triangle
                (0.5*(1+Math.Sqrt(3.0)),1.5+0.5*Math.Sqrt(3.0),-30,7), //duo on right, top triangle
                (0.5,0.5*Math.Sqrt(3.0),150,1) //duo on right, bottom triangle
            }
        };

        public override List<(double x, double y, double rotation, int state)> Rule(int state)
        {
            if (state >= 4)
                return m_rules[7-state].Select(((double x, double y, double rotation, int state) t) => (-t.x, t.y, -t.rotation, 7 - t.state)).ToList();
            else
                return m_rules[state];
        }

        public override bool ColorByState => true;

        public override bool Level0IsComposite => true;

        public override int[] StartStates => new int[] {3};

        public override void DefineCompositeBasePart(StringBuilder sb, int state)
        {
            int mainColor = ColorMap.Get(Colors[state]).id;
            string thirdID = BasePart(state) + "_third";
            int baseColor = ColorMap.Get("White").id;
            int grayColor = ColorMap.Get("Light_Bluish_Grey").id;
            bool mirrored = state >= 4;
            int s = mirrored ? -1 : 1;

            switch (state)
            {
                case 0:
                case 1:
                case 6:
                case 7: //triangle...
                    double yoffset = -Math.Sqrt(3.0) * 3;
                    Shape triangleThird = new() { PartID = thirdID, SubModel = true };
                    sb.AppendLine(triangleThird.Print(0, yoffset, 0, mainColor));
                    sb.AppendLine(triangleThird.Rotate(120).Print(1.5, yoffset + 1.5 * Math.Sqrt(3.0), 0, mainColor));
                    sb.AppendLine(triangleThird.Rotate(-120).Print(-1.5, yoffset +1.5 * Math.Sqrt(3.0), 0, mainColor));
                    sb.AppendLine(new Shape() { PartID = "3004" }.Print(0, yoffset + 0.5, - 1.5, mainColor));
                    sb.AppendLine(new Shape() { PartID = "99781", SwitchYZ2 = true }.Rotate(60).Print(-1.5, yoffset + 1.5 * Math.Sqrt(3.0), -3.25, mainColor));
                    sb.AppendLine(new Shape() { PartID = "99781", SwitchYZ2 = true }.Rotate(-60).Print(1.5, yoffset + 1.5 * Math.Sqrt(3.0), -3.25, mainColor));
                    double halfplated = 0.2;
                    sb.AppendLine(new Plate(2, 2) { SwitchYZ2 = true }.Rotate(60).Print(-1.5 - halfplated * Math.Sqrt(3.0), yoffset + halfplated + 1.5 * Math.Sqrt(3.0), -4.5, grayColor));
                    sb.AppendLine(new Plate(2, 2) { SwitchYZ2 = true }.Rotate(-60).Print(1.5 + halfplated * Math.Sqrt(3.0), yoffset + halfplated + 1.5 * Math.Sqrt(3.0), -4.5, grayColor));
                    sb.AppendLine(new Shape() { PartID = "30022", SwitchYZ2 = true }.Rotate(180).Print(0, yoffset, -5.75, mainColor));

                    MetaData.StartSubModel(sb, thirdID);
                    sb.AppendLine(new Shape() { PartID = "29120" }.Rotate(90).Print(1, 0.5, -1.5, baseColor));
                    sb.AppendLine(new Shape() { PartID = "29119" }.Rotate(-90).Print(-1, 0.5, -1.5, baseColor));
                    sb.AppendLine(new Plate(2).Print(0, 0.5, -0.5, baseColor));

                    break;
                case 2:
                case 5: //square
                    sb.AppendLine(new Plate(6, 6).Print(s*3, 3, 0, mainColor));
                    break;
                case 3:
                case 4: //shield
                    Shape shieldThird = new() { PartID = thirdID, SubModel = true };
                    sb.AppendLine(shieldThird.Rotate(s * 45).Print(0, 0, 0, mainColor));
                    double x = InitialScale * (0.5 * Math.Sqrt(2.0) + Math.Sin(Math.PI / 12));
                    double y = InitialScale * (0.5 * Math.Sqrt(2.0) + Math.Cos(Math.PI / 12));
                    sb.AppendLine(shieldThird.Rotate(-s * 75).Print(-s * x, y, 0, mainColor));
                    sb.AppendLine(shieldThird.Rotate(s * 165).Print(s * x, y, 0, mainColor));
                    int swivelColor = ColorMap.Get("Green").id;

                    MetaData.StartSubModel(sb, thirdID);
                    sb.AppendLine(new Plate(4, 4).Print(s*2, 2, 0, mainColor));
                    sb.AppendLine(new Shape() { PartID = mirrored? "43722":"43723" }.Rotate(s * 90).Print(s * 1.5, 5, 0, mainColor));
                    sb.AppendLine(new Shape() { PartID = mirrored? "43723":"43722" }.Rotate(180).Print(s * 5, 1.55, 0, mainColor));
                    sb.AppendLine(new Shape() { PartID = mirrored?"29119":"29120" }.Rotate(180).Print(s * 3.5, 5, -2, mainColor));
                    sb.AppendLine(new Plate(5).Print(s * 2.5, 0.5, -1, mainColor));
                    sb.AppendLine(new Plate(1, 4).Print(s * 0.5, 3, -1, mainColor));
                    sb.AppendLine(new Plate(1, 4).Print(s * 3.5, 3, -1, mainColor));
                    sb.AppendLine(new Shape() { PartID = mirrored?"2430":"2429" }.Rotate(-s * 90).Print(s * 3, 3, -2, swivelColor));
                    sb.AppendLine(new Shape() { PartID = mirrored ? "2429" : "2430" }.Rotate(s * 120).Print(s * 3, 3, -2, swivelColor));
                    sb.AppendLine(new Shape() { PartID = mirrored ? "2429" : "2430" }.Rotate(s * 120).Print(s * 3, 3, -3, swivelColor));
                    sb.AppendLine(new Shape() { PartID = mirrored ? "2430" : "2429" }.Rotate(s * 150).Print(s * 3, 3, -3, swivelColor));
                    sb.AppendLine(new Shape() { PartID = "30503" }.Rotate(mirrored?-120:30).Print(s * (2 + 0.5 * Math.Sqrt(3.0)), 3.5 + Math.Sqrt(3), -4, baseColor));
                    break;
            }
        }
    }
}
