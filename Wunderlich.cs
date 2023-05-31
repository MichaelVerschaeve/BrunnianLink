using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    public class Wunderlich : SubstitutionRule
    {
        static private readonly string[] m_colors = new[] { "Red", "Orange", "Yellow" };
        public override string[] Colors => m_colors;
        public int CurveType { get; set; }

        public override int StateCount => CurveType==0?4:8;

        public override double ScaleFactor => 3;

        public override double InitialScale => 15;

        public override string MainName => "Wonderlich_I";

        public override bool ColorByState => true;

        public override string BasePart(int state) => "SShape_" + ((state & 1) == 1 ? "c" : "s") + ((state & 2) == 2 ? "c" : "s")+ ((state & 4) == 4 ? "c" : "s");




        static private readonly List<(double x, double y, double rotation, int state)>[] m_rule = new[]
        {   new List<(double x, double y, double rotation, int state)>() 
            { //rotations
                (1,1,0,2),
                (0,1,-90,2),
                (-1,1,0,2),
                (-1,0,90,0),
                (0,0,180,3),
                (1,0,90,0),
                (1,-1,0,1),
                (0,-1,-90,1),
                (-1,-1,0,1)
            },
            new List<(double x, double y, double rotation, int state)>() 
            { //reflections
                (1,1,0,2),
                (1,0,180,7),
                (1,-1,0,1),
                (0,-1,0,5),
                (0,0,180,3),
                (0,1,0,6),
                (-1,1,0,2),
                (-1,0,180,7),
                (-1,-1,0,1)
            },
             new List<(double x, double y, double rotation, int state)>()
            { //R-shape combo
                (1,1,0,2),
                (1,0,90,7),
                (0,0,90,4),
                (0,1,180,0),
                (1,1,0,3),
                (0,1,0,2),
                (-1,1,0,0),
                (-1,0,-90,7),
                (-1,-1,-90,4),
             }
        };


        public override List<(double x, double y, double rotation, int state)> Rule(int state)
        {
            List<(double x, double y, double rotation, int state)> res = new(m_rule[CurveType]);
            res[0] = (res[0].x, res[0].y, res[0].rotation, res[0].state + (state & 1));
            res[8] = (res[8].x, res[8].y, res[8].rotation, res[8].state + (state & 2));

            if ((state & 4) == 4) //mirror
                res = res.Select(t => (-t.x, t.y, -t.rotation, t.state ^ 4)).ToList();

            return res;
        }

        public override bool Level0IsComposite => true;

        static readonly int DBGID = ColorMap.Get("Dark_Bluish_Grey").id;

        public override void DefineCompositeBasePart(StringBuilder sb, int state)
        {
            //uses again other subparts (defined after state 3)
            Shape corner1 = new() { PartID = "corner_up_left", SubModel = true };  //corner 1, from up to left color 1 in inner macaroni
            Shape corner2 = new() { PartID = "corner_right_down", SubModel = true };  //corner 2, from right to down color 3 in inner macaroni
            Shape straight = new() { PartID = "straight", SubModel = true }; //horizontal, color 1 on top


            if (CurveType == 2)
            {
                (corner1, corner2) = (corner2, corner1);
            }
            
            bool mirror = (state & 4)== 4;

            if (mirror)
            {
                corner1.RotateThis(-90);
                corner2.RotateThis(-90);
            }

            int s = mirror?-1:1;


            if (CurveType == 2)
            {

            }
            else
            {
                sb.AppendLine((((state & 1) == 0) ? straight : corner1).Print(s * 5, 5, 0, 16));
                sb.AppendLine(straight.Print(0, 5, 0, 16));
                sb.AppendLine(corner2.Print(-s * 5, 5, 0, 16));
                sb.AppendLine(corner2.Rotate(s * 90).Print(-s * 5, 0, 0, 16));
                sb.AppendLine(straight.Rotate(180).Print(0, 0, 0, 16));
                sb.AppendLine(corner1.Rotate(s * 90).Print(s * 5, 0, 0, 16));
                sb.AppendLine(corner1.Print(s * 5, -5, 0, 16));
                sb.AppendLine(straight.Print(0, -5, 0, 16));
                sb.AppendLine((((state & 2) == 0) ? straight : corner2).Print(-s * 5, -5, 0, 16));
            }

            Plate bottom = new(6, 6);
            sb.AppendLine(bottom.Print(4.5, 4.5, -1, DBGID));
            sb.AppendLine(bottom.Print(4.5, -4.5, -1, DBGID));
            sb.AppendLine(bottom.Print(-4.5, 4.5, -1, DBGID));
            sb.AppendLine(bottom.Print(-4.5, -4.5, -1, DBGID));
            bottom = new Plate(3, 3);
            sb.AppendLine(bottom.Print(0, 0, -1, DBGID));
            for (int i=3; i <= 6; i+=3)
            {
                sb.AppendLine(bottom.Print(i, 0, -1, DBGID));
                sb.AppendLine(bottom.Print(-i, 0, -1, DBGID));
                sb.AppendLine(bottom.Print(0,i, -1, DBGID));
                sb.AppendLine(bottom.Print(0,-i, -1, DBGID));
            }

            if (state < StateCount-1) return;


            MetaData.StartSubModel(sb, "straight");
            for (int i = 0; i <= 2; i++)
            {
                sb.AppendLine(new Tile(2).Print(-1.5, 1 - i, 0, ColorMap.Get(m_colors[i]).id));
                sb.AppendLine(new Tile(1).Print(0, 1 - i, 0, ColorMap.Get(m_colors[i]).id));
                sb.AppendLine(new Tile(2).Print(1.5, 1 - i, 0, ColorMap.Get(m_colors[i]).id));

            }
             MetaData.StartSubModel(sb, "corner_up_left");
             for (int r = 2; r <= 4; r++)
                 sb.AppendLine(new Bow(r,true).Print(-2.5, 2.5, 0, ColorMap.Get(m_colors[r - 2]).id));
             
             MetaData.StartSubModel(sb, "corner_right_down");
             for (int r = 2; r <= 4; r++)
                 sb.AppendLine(new Bow(r,true).Rotate(180).Print(2.5, -2.5, 0, ColorMap.Get(m_colors[4 - r]).id));
        }
    }
}
