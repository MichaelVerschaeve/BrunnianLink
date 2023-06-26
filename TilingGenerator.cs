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

//tilings to still consider
//Square Triangle Pinwheel Variant
//Square - triangle
//Monnier (scale Y)
//Rhomb Square Octagon
//Socolar - C5 (reuse penrose)
//Tangram
//Tetris
//Tromino 2
//Watanabe Ito Soma 8-fold
//Nischke - Danzer 6 - fold 2
//TriTriangle
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

        public abstract string BasePart(int state, int color); //names of the base parts

        public virtual bool Level0IsComposite { get => false; }

        public virtual double XPostScale { get => 1; }
        public virtual double YPostScale { get => 1; }

        private static readonly (double x, double y, double rotation, int state)[] m_startStates = new [] { (0.0,0.0,0.0,0) };

        public virtual (double x, double y, double rotation, int state)[] StartStates { get => m_startStates; }

        public virtual void Decorate(StringBuilder sb, int level, bool top, int state, int color)
        {            // add elements that cannot be defined by substitution alone, e.g. baseplates at top level
        }

        public virtual void DefineCompositeBasePart(StringBuilder sb, int state, int color)
        { //if level 0 elements are not base parts... no need to specify the "0 FILE" line... do use color 16 if not fixed colors...

        }

    }

    public class TilingGenerator
    {
        public required SubstitutionRule Rule { get; set; }

        public void Generate(StringBuilder sb, int level)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                MetaData.StartSubModel(sb, Rule.MainName);
                int state = level - 1;
                //test mode: just composite base part...
               // for (int state = 0; state < Rule.StateCount; state++)
                //{
                    Shape s = new() { PartID = Rule.BasePart(state,0), SubModel = true };
                    sb.AppendLine(s.Print(0, 0, 0, ColorMap.Get(Rule.Colors[state]).id));
               // }
                HashSet<string> definedParts = new();
                for (state = 0; state < Rule.StateCount; state++)
                {
                    string id = Rule.BasePart(state, 0);
                    if (definedParts.Contains(id))
                        continue;
                    definedParts.Add(id);
                    MetaData.StartSubModel(sb, id);
                    Rule.DefineCompositeBasePart(sb, state,0);
                }
                return;
            }
            SortedSet<(int level, int state, int color)> requiredParts =
                DetermineRequiredParts(level);

            if (Rule.StartStates.Length > 1)
            {
                MetaData.StartSubModel(sb, Rule.MainName);
                int count = 0;
                double scale = Math.Pow(Rule.ScaleFactor, level ) * Rule.InitialScale;
                foreach (var (x, y, rotation, state) in Rule.StartStates)
                {
                    int color = (Rule.ColorByState ? state : (count++)) % Rule.Colors.Length;
                    Shape s = new() { PartID = PartName(level, state, color), SubModel = true };
                    sb.AppendLine(s.Rotate(rotation).Print(scale*x, scale*y, 0, ColorMap.Get(Rule.Colors[state]).id));
                }
            }
            foreach (var t in requiredParts.Reverse())
                DoSubPart(sb, t.level, t.level == level, t.state, t.color);

            if (Rule.Level0IsComposite)
            {
                HashSet<string> definedParts = new();
                for (int state = 0; state < Rule.StateCount; state++)
                {
                    for (int color = 0; color < Rule.Colors.Length; color++)
                    {
                        string id = Rule.BasePart(state, color);
                        if (string.IsNullOrEmpty(id) || id.EndsWith(".dat") || definedParts.Contains(id)) //do nothing if no basepart is required, it's not composite or already defined
                            continue;
                        definedParts.Add(id);
                        MetaData.StartSubModel(sb, id);
                        Rule.DefineCompositeBasePart(sb, state, color);
                    }
                }
            }
        }
        private SortedSet<(int level, int state, int color)> DetermineRequiredParts(int level)
        {
            SortedSet<(int level, int state, int color)> res = new();
            int count = 0;
            foreach (var (x, y, rotation, state) in Rule.StartStates)
            {
                int color = (Rule.ColorByState ? state: (count++))%Rule.Colors.Length;
                res.UnionWith(RecurseDetermineRequiredParts(level, state, color));

            }
            return res;
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
            if (level == 0) return Rule.BasePart(state, colorOffset);
            else if (Rule.ColorByState) 
                return $"SubPart_{level}_{state}";
            else
                return $"SubPart_{level}_{state}_{colorOffset}";
        }


        private void DoSubPart(StringBuilder sb, int level, bool top, int state, int color)
        {
            MetaData.StartSubModel(sb, top && Rule.StartStates.Length == 1 ? Rule.MainName : PartName(level, state, color));
            double xscale = Math.Pow(Rule.ScaleFactor, level - 1) * Rule.InitialScale;
            double yscale = xscale;
            if (level == 0)
            {
                xscale *= Rule.XPostScale;
                yscale *= Rule.YPostScale;
            }
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
                string id = PartName(level - 1, t.state, color);
                if (string.IsNullOrEmpty(id)) //no lego elements required
                    continue;
                Shape subShape = new() { PartID = id, SubModel = level > 1 || Rule.Level0IsComposite };
                subShape.RotateThis(t.rotation);
                sb.AppendLine(subShape.Print(xscale * t.x, yscale * t.y, 1, ColorMap.Get(Rule.Colors[color]).id));
            }
            Rule.Decorate(sb, level, top, state, color);
        }
    }
}
