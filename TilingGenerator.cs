using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace BrunnianLink
{
    public abstract class SubstitutionRule
    {
        public abstract string[] Colors { get; }

        public virtual bool ColorByState { get => false; }

        public abstract int StateCount { get; }

        public abstract double ScaleFactor { get; }
        public abstract double InitialScale { get; }

        public abstract string MainName { get; }

        public abstract List<(double x, double y, double rotation, int state)> Rule(int state);

        public abstract string BasePart(int state); //names of the base parts

        public virtual bool Level0IsComposite { get => false; }

        public virtual int StartState { get => 0; }

        public virtual void Decorate(StringBuilder sb, int level, bool top, int state, int color)
        {            // add elements that cannot be defined by substitution alone, e.g. baseplates at top level
        }

        public virtual void DefineCompositeBasePart(StringBuilder sb, int state)
        { //if level 0 elements are not base parts... no need to specify the "0 FILE" line... do use color 16 if not fixed colors...

        }

    }

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

    public class ChairRule : SubstitutionRule
    {
        static readonly string[] m_colors = new string[] { "Light_Bluish_Grey", "Tan", "Dark_Tan", "Dark_Bluish_Grey" };
        override public string[] Colors => m_colors;

        override public int StateCount => 1;

        override public double ScaleFactor => 2;
        override public double InitialScale => 1;

        override public string MainName => "ChairTiling";

        override public string BasePart(int state)
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

    public class TennisRule : SubstitutionRule
    {
        static readonly string[] m_colors = new string[] { "Light_Bluish_Grey", "Tan", "Dark_Tan", "Dark_Bluish_Grey" };
        override public string[] Colors => m_colors;

        override public int StateCount => 1;

        override public double ScaleFactor => 2;
        override public double InitialScale => 1;

        override public string MainName => "TennisTiling";

        override public string BasePart(int state)=> Plate.XYPartID(1, 2); 

        static readonly List<(double x, double y, double rotation, int state)> m_rule = new()
            {
                (-1.5,0,90.0,0),
                (0,0.5,0.0,0),
                (0,-0.5,0.0,0),
                (1.5,0,90.0,0)
            };

        override public List<(double x, double y, double rotation, int state)> Rule(int _) => m_rule;
    }

    public class WandererReflectionsRule : SubstitutionRule
    {
        static readonly string[] m_colors = new string[] { "Reddish_Brown", "White" };
        override public string[] Colors => m_colors;
        override public int StateCount => 2;

        override public double ScaleFactor => 2;
        override public double InitialScale => 1;

        override public string MainName => "Wanderer_reflections";
        override public string BasePart(int state)
        {
            return state==0?"LeftHanded":"RightHanded";
        }

        public override bool Level0IsComposite => true;

        public override void DefineCompositeBasePart(StringBuilder sb, int state)
        {
            MetaData.StartSubModel(sb, BasePart(state));
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


    public class WandererRotationsRule : SubstitutionRule
    {
        static readonly string[] m_colors = new string[] { "Dark_Blue", "Tan" };
        override public string[] Colors => m_colors;
        override public int StateCount => 4;

        override public double ScaleFactor => 2;
        override public double InitialScale => 1;

        override public string MainName => "Wanderer_rotations";

        override public string BasePart(int state)
        {
            bool vert = (state & 1) == 1;
            bool right = (state & 2) == 2;
            return (vert ? "Vertical" : "Horizontal") + (right?"Right":"Left");
        }

        public override bool Level0IsComposite => true;

        public override void DefineCompositeBasePart(StringBuilder sb, int state)
        {
            MetaData.StartSubModel(sb, BasePart(state));
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

    public class AmmannBeenker : SubstitutionRule
    {
        static readonly string[] m_colors = new string[] { "Magenta", "Magenta", "Medium_Azure" };

        public override string[] Colors => m_colors;

        public override int StateCount => 3;

        public override bool ColorByState => true;


        static readonly double m_sqrt2p1 = 1 + Math.Sqrt(2.0);
        public override double ScaleFactor => m_sqrt2p1;

        public override double InitialScale => 8 + 4 * Math.Sqrt(2.0);

        public override string MainName => "AmmannBeenker";

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
                (-m_sqrt2p1*0.5,0.5,-135,1),
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

        override public int StartState { get => 2; }

        private void DefineQuarter(StringBuilder sb, bool right)
        {
            MetaData.StartSubModel(sb, right?"QuarterRight":"QuarterLeft");
            Shape wedge = new() { PartID = "15706" };   //from 135 to 180 degree wedge
            int s = right ? -1 : 1;
            sb.AppendLine(wedge.Rotate(right?-135:0).Print(s*3,0,0,16));
            sb.AppendLine(wedge.Rotate(right ?0 : -135).Print(-s*3,0,1,16));
            Plate p = new(2);
            sb.AppendLine(p.Print(s * 2, -0.5, 1, 16));
            sb.AppendLine(p.Print(-s * 2, -0.5, 0, 16));
        }

        private void DefineHalfRhomb(StringBuilder sb)
        {
            MetaData.StartSubModel(sb, "HalfRhumb");
            sb.AppendLine(new Shape() { PartID = "15706" }.Print(0, 0, 0,16));
            sb.AppendLine(new Shape() { PartID = "2429" }.Print(-8, 0, 0, 16));
            Plate p2 = new(2);
            Plate p8 = new(8);
            sb.AppendLine(p2.Print(-5, -0.5, 0, 16));
            sb.AppendLine(p8.Print(-4, -0.5, 1, 16));
            double t = 4 * Math.Sqrt(2.0);
            sb.AppendLine(new Shape() { PartID = "2430" }.Rotate(-45).Print(-t, t, 0, 16));
            t = Math.Sqrt(2.0) * 0.5;
            sb.AppendLine(p2.Rotate(-45).Print(-4.5*t, 5.5*t, 0, 16));
            sb.AppendLine(p8.Rotate(-45).Print(-3.5*t, 4.5*t, 1, 16));
        }

        public override void DefineCompositeBasePart(StringBuilder sb, int state)
        {
            MetaData.StartSubModel(sb, BasePart(state));
            switch (state)
            {
                case 0:
                    Shape downTriangle = new() { PartID = BasePart(1), SubModel = true };
                    sb.AppendLine(downTriangle.Rotate(180).Print(0, 0, 0, 16));
                    break;
                case 1:
                   Shape quarter = new() { PartID = "QuarterLeft", SubModel = true };
                    double t = 1.0 + 1.5 * Math.Sqrt(2.0);
                    sb.AppendLine(quarter.Rotate(-45).Print(-t, -t, 0, 16));
                    quarter = new() { PartID = "QuarterRight", SubModel = true };
                    sb.AppendLine(quarter.Rotate(45).Print(t, -t, 0, 16));
                    Plate plate = new(4);
                    t = -1 + 3 * Math.Sqrt(2.0);
                    sb.AppendLine(plate.Print(-t, -0.5, 0, 16));
                    sb.AppendLine(plate.Print(t, -0.5, 0, 16));
                    sb.AppendLine(new Plate(2, 4).Print(0, -t, 1, 16));
                    DefineQuarter(sb, false);
                    DefineQuarter(sb, true);
                    break;
                case 2:
                    Shape HalfRumb = new() { PartID = "HalfRhumb", SubModel = true };
                    double x = 1 + 0.5 * Math.Sqrt(2.0);
                    double y = -1;
                    sb.Append(HalfRumb.Rotate(180).Print(x,y, 0, 16));
                    x += 3 * Math.Sqrt(2.0);
                    y -= 3 * Math.Sqrt(2.0);
                    sb.Append(HalfRumb.Print(x,y, 0, 16));
                    DefineHalfRhomb(sb);
                    break;

            }
        }
    }


    public class TilingGenerator
    {
        public required SubstitutionRule Rule { get; set; }

        public void Generate(StringBuilder sb, int level)
        {
            SortedSet<(int level, int state, int color)> requiredParts = RecurseDetermineRequiredParts(level, Rule.StartState, 0);
            foreach (var t in requiredParts.Reverse())
                DoSubPart(sb, t.level, t.level == level, t.state, t.color);

            if (Rule.Level0IsComposite)
                for (int i =0; i < Rule.StateCount; i++)
                    Rule.DefineCompositeBasePart(sb, i);
        }

        private SortedSet<(int level, int state, int color)> RecurseDetermineRequiredParts(int level, int state, int color)
        {
            SortedSet<(int level, int state, int color)> res = new()
            {
                (level, state, color)
            };
            if (level > 1)
            {
                color--;
                foreach (var t in Rule.Rule(state))
                {
                    if (Rule.ColorByState)
                        color = t.state % Rule.Colors.Length;
                    else
                    {
                        color++;
                        if (color == Rule.Colors.Length) color = 0;
                    }
                    res.UnionWith(RecurseDetermineRequiredParts(level - 1, t.state, color));
                }
            }
            
            return res;
        }

        private string PartName(int level, int state, int colorOffset)
        {
            if (level == 0) return Rule.BasePart(state);
            else if (Rule.ColorByState) 
                return $"SubPart_{level}_{state}";
            else
                return $"SubPart_{level}_{state}_{colorOffset}";
        }


        private void DoSubPart(StringBuilder sb, int level, bool top, int state, int color)
        {
            MetaData.StartSubModel(sb, top ? Rule.MainName : PartName(level, state, color));
            double scale = Math.Pow(Rule.ScaleFactor, level-1)*Rule.InitialScale;
            color--;
            foreach (var t in Rule.Rule(state))
            {
                if (Rule.ColorByState)
                    color = t.state % Rule.Colors.Length;
                else
                {
                    color++;
                    if (color == Rule.Colors.Length) color = 0;
                }
                Shape subShape = new() { PartID = PartName(level-1, t.state, color), SubModel = level > 1 || Rule.Level0IsComposite };
                subShape.RotateThis(t.rotation);
                sb.AppendLine(subShape.Print(scale*t.x,scale*t.y, 1, ColorMap.Get(Rule.Colors[color]).id));
            }
            Rule.Decorate(sb, level,top,state,color);
        }
    }
}
