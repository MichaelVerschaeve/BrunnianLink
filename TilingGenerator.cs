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
//MiniTangram
//hoffstedter
//Square Triangle Pinwheel Variant
//Square - triangle
//Monnier (scale Y)
//Pentomino
//Rhomb Square Octagon
//Socolar - C5 (reuse penrose)
//Tangram
//Tetris
//Tromino 2
//Watanabe Ito Soma 8-fold

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

        private static readonly int[] m_startStates = new int[] { 0 };

        public virtual int[] StartStates { get => m_startStates; }

        public virtual void Decorate(StringBuilder sb, int level, bool top, int state, int color)
        {            // add elements that cannot be defined by substitution alone, e.g. baseplates at top level
        }

        public virtual void DefineCompositeBasePart(StringBuilder sb, int state)
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
                    Shape s = new() { PartID = Rule.BasePart(state), SubModel = true };
                    sb.AppendLine(s.Print(0, 0, 0, ColorMap.Get(Rule.Colors[state]).id));
               // }
                HashSet<string> definedParts = new();
                for (state = 0; state < Rule.StateCount; state++)
                {
                    string id = Rule.BasePart(state);
                    if (definedParts.Contains(id))
                        continue;
                    definedParts.Add(id);
                    MetaData.StartSubModel(sb, id);
                    Rule.DefineCompositeBasePart(sb, state);
                }
                return;
            }
            SortedSet<(int level, int state, int color)> requiredParts =
                DetermineRequiredParts(level);

            if (Rule.StartStates.Length > 1)
            {
                MetaData.StartSubModel(sb, Rule.MainName);
                int count = 0;
                foreach (int state in Rule.StartStates)
                {
                    int color = (Rule.ColorByState ? state : (count++)) % Rule.Colors.Length;
                    Shape s = new() { PartID = PartName(level, state, color), SubModel = true };
                    sb.AppendLine(s.Print(0, 0, 0, ColorMap.Get(Rule.Colors[state]).id));
                }
            }
            foreach (var t in requiredParts.Reverse())
                DoSubPart(sb, t.level, t.level == level, t.state, t.color);

            if (Rule.Level0IsComposite)
            {
                HashSet<string> definedParts = new();
                for (int state = 0; state < Rule.StateCount; state++)
                {
                    string id = Rule.BasePart(state);
                    if (definedParts.Contains(id))
                        continue;
                    definedParts.Add(id);
                    MetaData.StartSubModel(sb, id);
                    Rule.DefineCompositeBasePart(sb, state);
                }
            }
        }
        private SortedSet<(int level, int state, int color)> DetermineRequiredParts(int level)
        {
            SortedSet<(int level, int state, int color)> res = new();
            int count = 0;
            foreach (int state in Rule.StartStates)
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
            if (level == 0) return Rule.BasePart(state);
            else if (Rule.ColorByState) 
                return $"SubPart_{level}_{state}";
            else
                return $"SubPart_{level}_{state}_{colorOffset}";
        }


        private void DoSubPart(StringBuilder sb, int level, bool top, int state, int color)
        {
            MetaData.StartSubModel(sb, top && Rule.StartStates.Length==1? Rule.MainName : PartName(level, state, color));
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
