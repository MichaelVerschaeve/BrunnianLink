using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace BrunnianLink
{
    static public class SphereGenerator
    {
        private const double epsilon = 1.0e-7;

        const bool forceColor = true; //set to true for renderings
        static readonly int colorId = ColorMap.Get("Black").id; //only black has currently all parts...
        public static void Generate(StringBuilder sb, int level)
        {
            int innerR = level;
            double outerR = Math.Sqrt(3) * innerR;
            MetaData.StartSubModel(sb, $"Sphere{level}");
            //List<string> colors = new List<string>() { "Violet", "Bright_Light_Orange", "Sand_Green", "Tan","Lime", "Medium_Azure" };
            List<string> colors = new() { "Red", "Red", "Green", "Green", "Blue", "Blue" };
            foreach (Direction dir in Enum.GetValues<Direction>())
            {
                PrintPart(sb, innerR, dir, ColorMap.Get(colors[(int)dir]).id);
            }
            MetaData.StartSubModel(sb, "Slice");
            int i = 0;
            double plateHeight = 0.4;
            double currentVolume = CapVolume(innerR, outerR);
            List<double> radii = new();
            for (double h = innerR; h < outerR - epsilon; h += plateHeight)
            {
                Shape layer = new() { SubModel = true, PartID = $"layer{i++}" };
                sb.AppendLine(layer.Print(0, 0, i, 16));
                double nextVolume = CapVolume(h + plateHeight, outerR);
                double r = Math.Sqrt((currentVolume - nextVolume) / plateHeight);
                if (r < 0.25) break;
                radii.Add(r);
                currentVolume = nextVolume;
            }
          
            int whiteId = ColorMap.Get("Black").id;
            i = 0;
            for (double h = innerR; h < outerR - epsilon && i <radii.Count; h += plateHeight)
            {
                double r = radii[i];
                MetaData.StartSubModel(sb, $"layer{i++}");
                double nextr = i<radii.Count?radii[i]:0;
                if (r < 0.5) break;
                CircleAprroximator ca = new(r, nextr, innerR, forceColor?whiteId:16);
                ca.Generate(sb);
            }
        }

        private static double CapVolume(double h, double outerR)
        {   //volume of sphere/Pi
            double hrem = outerR - h;
            if (hrem < 0) return 0;
            return hrem * hrem * (3 * outerR - hrem) / 3.0;
        }

        enum Direction { Top, Bottom, Left, Right, Front, Back };

        static void PrintPart(StringBuilder sb, int distance, Direction dir, int color)
        {
            string rotmat = dir switch
            {
                Direction.Top => "1 0 0 0 1 0 0 0 1",
                Direction.Bottom => "-1 0 0 0 -1 0 0 0 1",
                Direction.Left => "0 1 0 0 0 -1 -1 0 0",
                Direction.Right => "0 -1 0 0 0 -1 1 0 0",
                Direction.Front => "0 0 1 1 0 0 0 1 0",
                Direction.Back => "0 0 1 -1 0 0 0 -1 0",
                _ => ""
            };
            double ldudistance = distance * 20;
            double dx = dir switch { Direction.Left => -ldudistance, Direction.Right => ldudistance, _ => 0.0 };
            double dy = dir switch { Direction.Top => -ldudistance, Direction.Bottom => ldudistance, _ => 0.0 }; ;
            double dz = dir switch { Direction.Front => -ldudistance, Direction.Back => ldudistance, _ => 0.0 }; ;
            sb.AppendLine($"1 {color} {dx} {dy} {dz} {rotmat} Slice");
        }
    }

    public class CircleAprroximator
    {
        private readonly double r;
        private readonly double nextR;
        private readonly int xLimit;
        private readonly double errorVal;
        private readonly int plateHeight;
        private readonly int color;
        const int band = 3;

        private const double epsilon = 1.0e-7;

        class Slope
        {
            public int leftmargin;
            public int bottommargin;
            public int dx;
            public int dy;
            public string id = "";
            public string mirrorId = "";
            public bool rounded=false;
        }


        static readonly Slope horzSlope = new() { leftmargin = 0, bottommargin = 0, dx = 1, dy = 0 };
        static readonly Slope vertSlope = new() { leftmargin = 0, bottommargin = 0, dx = 0, dy = 1 };

        static readonly List<Slope> m_slopes = new()
        {
            horzSlope,
            new Slope { leftmargin = 1, bottommargin = 1, dx = 1, dy = 1, id="26601", mirrorId="26601"},
            new Slope { leftmargin = 1, bottommargin = 1, dx = 2, dy = 2, id="2450", mirrorId="2450"},
            new Slope { leftmargin = 1, bottommargin = 1, dx = 3, dy = 3, id="30503", mirrorId="30503"},
            new Slope { leftmargin = 2, bottommargin = 2, dx = 4, dy = 4, id="6106", mirrorId="6106"},
            new Slope { leftmargin = 1, bottommargin = 1, dx = 7, dy = 7, id="30504", mirrorId="30504"},
            new Slope { leftmargin = 1, bottommargin = 0, dx = 1, dy = 2, id="24307", mirrorId="24299"},
            new Slope { leftmargin = 0, bottommargin = 0, dx = 2, dy = 4, id="65426", mirrorId="65429"},
            new Slope { leftmargin = 1, bottommargin = 0, dx = 1, dy = 3, id="43722", mirrorId="43723"},
            new Slope { leftmargin = 1, bottommargin = 0, dx = 1, dy = 4, id="41769", mirrorId="41770"},
            new Slope { leftmargin = 1, bottommargin = 0, dx = 1, dy = 6, id="78444", mirrorId="78443"},
            new Slope { leftmargin = 2, bottommargin = 2, dx = 8, dy = 8, id="92584", mirrorId="92584"},
            new Slope { leftmargin = 1, bottommargin = 1, dx = 1, dy = 1, id="79491", mirrorId="79491", rounded=true},
            new Slope { leftmargin = 1, bottommargin = 1, dx = 2, dy = 2, id="30357", mirrorId="30357", rounded=true},
            new Slope { leftmargin = 0, bottommargin = 0, dx = 1, dy = 1, id="25269", mirrorId="25269", rounded=true},
            new Slope { leftmargin = 0, bottommargin = 0, dx = 4, dy = 4, id="30565", mirrorId="30565", rounded=true},
            new Slope { leftmargin = 0, bottommargin = 0, dx = 2, dy = 2, id="35787", mirrorId="35787" },
            vertSlope,

        };

        public CircleAprroximator(double _r, double _nextR, int _xlimit, int colorID)
        {
            r = _r;
            nextR = _nextR;
            errorVal = (double)(r) * r;
            xLimit = _xlimit;
            plateHeight = 0;
            color = colorID;
        }

        [Flags] 
        enum PrintFlag : short
        {
            None = 0,
            Octant1 = 1,
            Octant2 = 2,
            Octant3 = 4,
            Octant4 = 8,
            Octant5 = 16,
            Octant6 = 32,
            Octant7 = 64,
            Octant8 = 128,
            Quadrants = Octant1 | Octant4 | Octant5 | Octant8,
            Quadrants2 = Octant2 | Octant3 | Octant6 | Octant7,
            All = 255
        }

        private readonly Dictionary<(int x, int y, int fwidth,  int fy), (double err, Slope s)> m_cache1 = new();
        private readonly Dictionary<(int x, int y, int fwidth, int fy), (double err, Slope s)> m_cache2 = new();

        private readonly List<(int x, int y, int w, int h)> m_rects1 = new();
        private readonly List<(int x, int y, int w, int h)> m_rects2 = new();

        public void Generate(StringBuilder sb)
        {
            midTaken = false;
            int bestr = 0;
            int bestSlope = 0;
            string bestId = "" ;
            double bestError = double.MaxValue;
            var EvenSlopes = m_slopes.Where(s => s.dx == s.dy && (s.dx & 1) == 0);
            var OddSlopes = m_slopes.Where(s => s.dx == s.dy && (s.dx & 1) == 1);
            int lowlimit = (int)Math.Max(0,Math.Ceiling((r - band) / Math.Sqrt(2.0)));
            int uplimit = (int)Math.Floor((r + band) / Math.Sqrt(2.0));
            bool symmetric = xLimit >= Math.Ceiling(r);

            for (int r = lowlimit; r <= uplimit; r++)
            {
                double candidateError = Recurse(r, r, 0, 1, true);
                if (!symmetric) candidateError += Recurse(r, r, 0, 1, false);
                if (candidateError < bestError)
                {
                    bestError = candidateError;
                    bestSlope = 0;
                    bestr = r;
                    bestId = "";
                }
                foreach (Slope s in EvenSlopes)
                {
                    if (r - s.dx/2-s.leftmargin<0) continue;
                    int newx = r + s.dx / 2;
                    int newy = r - s.dy / 2;
                    if (newy - s.bottommargin < 0 ) continue;
                    candidateError = CalcError((newx, newy), (r, r),s.rounded);
                    if (symmetric)
                        candidateError += Recurse(newx, newy, 0, s.bottommargin, true);
                    else
                    {
                        candidateError *= 2.0;
                        candidateError += Recurse(newx, newy, 0, s.bottommargin, true);
                        candidateError += Recurse(newx, newy, 0, s.leftmargin, false);
                    }
                    if (candidateError < bestError)
                    {
                        bestError = candidateError;
                        bestSlope = s.dx;
                        bestr = r;
                        bestId = s.id;
                    }
                }
            }
            for (double r = lowlimit - 0.5; r <= uplimit + 0.5; r += 1.0)
            {
                foreach (Slope s in OddSlopes)
                {
                    if (r - s.dx *0.5 - s.leftmargin < 0) continue;
                    int newx = (int)(r + s.dx * 0.5);
                    int newy = (int)(r - s.dy * 0.5);
                    if (newy - s.bottommargin < 0) continue;
                    double candidateError = CalcError((newx, newy), (r, r), s.rounded);
                    if (symmetric)
                        candidateError += Recurse(newx, newy, 0, s.bottommargin, true);
                    else
                    {
                        candidateError *= 2.0;
                        candidateError += Recurse(newx, newy, 0, s.bottommargin, true);
                        candidateError += Recurse(newx, newy, 0, s.leftmargin, false);
                    }
                    if (candidateError < bestError)
                    {
                        bestError = candidateError;
                        bestSlope = s.dx;
                        bestr = (int)r;
                        bestId = s.id;
                    }
                }
            }

            if (symmetric && r<band+4)
            {
                int candidateR = (int) Math.Max(0.0,Math.Min(4.0,Math.Round(r)));
                double candidateError = Math.PI * (r+ candidateR)*Math.Abs(r-candidateR)*0.125;
                double dotCandidateError = Math.PI * (r + 0.5) * Math.Abs(r - 0.5) * 0.125;
                if (dotCandidateError < bestError && dotCandidateError < candidateError)
                { //dot wins
                    Shape circle = new() { PartID ="4073"};
                    sb.AppendLine(circle.Print(0, 0, plateHeight, color));
                    return;
                }
                if (candidateError < bestError)
                {
                    if (candidateR == 0) return; //nothing is best...
                    Shape circle = new() { PartID = candidateR switch { 1 => "4032", 2 => "60474", 3 => "11213", 4 => "74611", _ => "" } };
                    sb.AppendLine(circle.Print(0, 0, plateHeight, color));
                    return;
                }
            }

            //now reconstruct and print
            Slope? lastSlope = null;
            int x = 0, y = 0, fx = 0, fy = 0;
            if (bestSlope != 0) //one diagonal element
            {
                if ((bestSlope & 1) == 0) //even
                {
                    lastSlope = EvenSlopes.First(s => s.dx == bestSlope && s.id == bestId);
                    PrintSlope(sb, lastSlope, bestr - bestSlope / 2, bestr + bestSlope / 2, PrintFlag.Quadrants);
                    x = bestr + bestSlope / 2;
                    y = bestr - bestSlope / 2;
                }
                else
                {
                    lastSlope = OddSlopes.First(s => s.dx == bestSlope && s.id == bestId);
                    PrintSlope(sb, lastSlope, (int)(bestr + 0.5 - 0.5 * bestSlope), (int)(bestr + 0.5 + 0.5 * bestSlope), PrintFlag.Quadrants);
                    x = (int)(bestr + 0.5 + 0.5 * bestSlope);
                    y = (int)(bestr + 0.5 - 0.5 * bestSlope);
                }
                fy = lastSlope.bottommargin;
            }
            else
            {
                lastSlope = null;
                x = y = bestr;
                fy = 1;
            }

            Reconstruct(sb, symmetric, true, lastSlope, x, y, fx, fy);
            if (!symmetric) 
                Reconstruct(sb, false, false, lastSlope, x, y, fx, fy);
            
            HandleInnerRects(sb, symmetric);
        }

        void HandleInnerRects(StringBuilder sb, bool symmetric)
        {
            List<(int x, int y, int w, int h)> rects = m_rects1.Concat((symmetric?m_rects1:m_rects2).Select(r=>(r.y-r.h,r.x+r.w,r.h,r.w))).Distinct().ToList();
            var flags = PrintFlag.Quadrants;

            rects.Sort((r1, r2) => r1.x == r2.x ? r2.y.CompareTo(r1.y) : r1.x.CompareTo(r2.x));
            while (rects.Count > 1)
            {
                int bestindex = 0;
                double largestRadius = 0;
                for (int i = 0; i < rects.Count - 1; i++)
                {
                    var cr1 = rects[i];
                    var cr2 = rects[i + 1];
                    double cx = Math.Min(cr1.x + cr1.w, cr2.x);
                    double cy = Math.Min(cr1.y - cr1.h, cr2.y);
                    double cr = Math.Sqrt(cx * cx + cy * cy);
                    if (cr > largestRadius)
                    {
                        largestRadius = cr;
                        bestindex = i;
                    }
                }
                if (largestRadius == 0) break;
                var r1 = rects[bestindex];
                var r2 = rects[bestindex + 1];
                double x = Math.Min(r1.x + r1.w, r2.x) + 0.5;
                double y = Math.Min(r1.y - r1.h, r2.y) +0.5;
                if (Math.Sqrt(x * x + y * y) <= nextR) break; //all good
                PrintRectangle(sb, r1.x, Math.Min(r1.y - r1.h, r2.y), Math.Min(r2.x - r1.x, r1.w), Math.Min(r1.y -r1.h - r2.y+r2.h, r2.h), flags, true);
                rects[bestindex] = (r1.x, r1.y, r2.x + r2.w - r1.x, r1.y - r2.y + r2.h);
                rects.RemoveAt(bestindex + 1);


            }
        }

        void Reconstruct(StringBuilder sb, bool symmetric, bool firstPass, Slope? lastSlope, int x, int y, int fx, int fy)
        {
            var cache = firstPass ? m_cache1 : m_cache2;

            List<bool> lineCommands = new();
            var flags = symmetric ? PrintFlag.All : (firstPass ? PrintFlag.Quadrants : PrintFlag.Quadrants2);
            while (cache.TryGetValue((x, y, fx, fy), out (double err, Slope s) val))
            {
                Slope slope = val.s;
                if (slope==horzSlope)
                {
                    x++;
                    if (fy > 0)
                    {
                        fx++;
                        if (fx >= 2) //no limit imposed...
                            fx=fy = 0;
                    }
                    lineCommands.Add(true);
                }
                else if (slope==vertSlope)
                {
                    y--;
                    if ( fy > 0)
                    {
                        fy--;
                        if (fy == 0)
                            fx = 0;
                    }
                    lineCommands.Add(false);
                }
                else
                {
                    if (lineCommands.Count > 0)
                    {
                        if (lastSlope == null)
                        {
                            if (symmetric)
                            {
                                lineCommands = lineCommands.Select(b=>!b).ToArray().Reverse().Concat(lineCommands).ToList();
                                HandleLineCommands(sb, lineCommands, x, y, slope.leftmargin, slope.leftmargin, PrintFlag.Quadrants);
                            }
                            else
                                HandleLineCommands(sb, lineCommands, x, y, 0, slope.leftmargin, flags);
                        }
                        else
                            HandleLineCommands(sb, lineCommands, x, y, lastSlope.bottommargin, slope.leftmargin, flags);
                        lineCommands.Clear();
                    }
                    lastSlope = slope;
                    PrintSlope(sb, lastSlope, x, y, flags);
                    x += slope.dx;
                    y -= slope.dy;
                    fy = slope.bottommargin;
                    fx = 0;
                }
            }
            if (y>=0)
                lineCommands.AddRange(Enumerable.Repeat<bool>(false,y));
            if (lineCommands.Count > 0)
            {
                HandleLineCommands(sb, lineCommands, x, 0, lastSlope?.bottommargin??(midTaken?1:0), 2, flags);
            }
        }

        bool midTaken;

        void HandleLineCommands(StringBuilder sb, List<bool> lineCommands, int x, int y, int bottommarginPrev, int leftMarinNext, PrintFlag printFlag)
        {
            while (bottommarginPrev > 0 && lineCommands.Count > 0 && lineCommands[0]==false) 
            { 
                lineCommands.RemoveAt(0);
                bottommarginPrev--;
            }
            while (leftMarinNext > 0 && lineCommands.Count > 0 && lineCommands.Last() == true)
            {
                lineCommands.RemoveAt(lineCommands.Count - 1);
                leftMarinNext--;
                x--;
            }
            if (lineCommands.Count == 0) return;
            int w = lineCommands.Count(b => b);
            int h = lineCommands.Count(b => !b);
            if (bottommarginPrev == 0 && ((y+h<x-w) || (!midTaken && y+h==x-w))) //spare room
            { 
                if (y + h == x - w)
                    midTaken = true;
                lineCommands.Insert(0, true);
                w++;
            }
            if(leftMarinNext == 0 && y>0) //spare room;
            {
                lineCommands.Add(false);
                y--;
                h++;
            }
            if (lineCommands.Count > 0 && !lineCommands[0])
            {//this should not happen...
                h--;
                lineCommands.RemoveAt(0);
            }
            x -= w;
            int miny = y;
            y += h;

            while (lineCommands.Count > 0)
            {
                w = 0;
                h = 0;
                while (lineCommands.Count > 0 && lineCommands[0])
                {
                    lineCommands.RemoveAt(0);
                    w++;
                }
                while (lineCommands.Count > 0 && !lineCommands[0])
                {
                    lineCommands.RemoveAt(0);
                    h++;
                }
                if (w>0 && h>0)
                    PrintRectangle(sb,x, y, w, y - h > miny ? (h + 1) : h, printFlag);
                x+= w;
                y -= h;
            }
        }

        void PrintRectangle(StringBuilder sb, int x, int y, int w, int h, PrintFlag flag, bool recursing = false) 
        {
            if (w <= 0 || h <= 0) return;
            if (!recursing)
                (flag == PrintFlag.Quadrants2 ? m_rects2 : m_rects1).Add((x,y,w,h));

            if (Plate.PlateExists(w,h))
            {
                double xd = x + 0.5*w;
                double yd = y - 0.5*h;
                Plate p = new(w, h);
                if ((flag & PrintFlag.Octant1) == PrintFlag.Octant1) sb.AppendLine(p.Print(xd, yd, plateHeight, color));
                if ((flag & PrintFlag.Octant4) == PrintFlag.Octant4) sb.AppendLine(p.Print(-xd, yd, plateHeight, color));
                if ((flag & PrintFlag.Octant5) == PrintFlag.Octant5) sb.AppendLine(p.Print(-xd, -yd, plateHeight, color));
                if ((flag & PrintFlag.Octant8) == PrintFlag.Octant8) sb.AppendLine(p.Print(xd, -yd, plateHeight, color));
                p = new(h, w);
                (xd, yd) = (yd, xd);
                if ((flag & PrintFlag.Octant2) == PrintFlag.Octant2) sb.AppendLine(p.Print(xd, yd, plateHeight, color));
                if ((flag & PrintFlag.Octant3) == PrintFlag.Octant3) sb.AppendLine(p.Print(-xd, yd, plateHeight, color));
                if ((flag & PrintFlag.Octant6) == PrintFlag.Octant6) sb.AppendLine(p.Print(-xd, -yd, plateHeight, color));
                if ((flag & PrintFlag.Octant7) == PrintFlag.Octant7) sb.AppendLine(p.Print(xd, -yd, plateHeight, color));
            }
            else if (w >= h)
            {
                PrintRectangle(sb, x, y, w / 2, h, flag,true);
                PrintRectangle(sb, x + w/2, y, w-w/2, h, flag,true);
            }
            else
            {
                PrintRectangle(sb, x, y, w ,h/2, flag,true);
                PrintRectangle(sb, x, y-h/2, w, h-h/2, flag,true);
            }
        }

        void PrintSlope(StringBuilder sb, Slope s, int x, int y, PrintFlag flag = PrintFlag.All)
        {
            (flag == PrintFlag.Quadrants2 ? m_rects2 : m_rects1).Add((x-s.leftmargin, y, s.leftmargin+s.dx, s.bottommargin+s.dy));

            Shape shape = new Shape() { PartID = s.id }.Rotate(s.dx == s.dy ? 90 : 180);
            Shape mirrorshape = new Shape() { PartID = s.mirrorId }.Rotate(180);
            double xd = x - s.leftmargin + (s.leftmargin + s.dx) * 0.5;
            double yd = y - (s.bottommargin + s.dy) * 0.5;
            double height = plateHeight;
            if (s.id=="79491") //rotary 
            {
                shape.RotateThis(-90);
                mirrorshape.RotateThis(-90);
                height -= 1;
            }
            else if (s.id == "30357" )
            {
                shape.RotateThis(-90);
                mirrorshape.RotateThis(-90);
                xd -= 1;
                yd -= 1;
            }
            else if ( s.id == "35787")
            {
                shape.RotateThis(-90);
                mirrorshape.RotateThis(-90);
            }

            if ((flag & PrintFlag.Octant1) == PrintFlag.Octant1) sb.AppendLine(shape.Print(xd, yd, height, color));
            if ((flag & PrintFlag.Octant4) == PrintFlag.Octant4) sb.AppendLine(mirrorshape.Print(-xd, yd, height, color));
            if ((flag & PrintFlag.Octant5) == PrintFlag.Octant5) sb.AppendLine(shape.Rotate(180).Print(-xd, -yd, height, color));
            if ((flag & PrintFlag.Octant8) == PrintFlag.Octant8) sb.AppendLine(mirrorshape.Rotate(180).Print(xd, -yd, height, color));
            (shape, mirrorshape) = (mirrorshape.Rotate(-90), shape.Rotate(90));
            (xd, yd) = (yd, xd);
            if ((flag & PrintFlag.Octant2) == PrintFlag.Octant2) sb.AppendLine(shape.Print(xd, yd, height, color));
            if ((flag & PrintFlag.Octant3) == PrintFlag.Octant3) sb.AppendLine(mirrorshape.Print(-xd, yd, height, color));
            if ((flag & PrintFlag.Octant6) == PrintFlag.Octant6) sb.AppendLine(shape.Rotate(180).Print(-xd, -yd, height, color));
            if ((flag & PrintFlag.Octant7) == PrintFlag.Octant7) sb.AppendLine(mirrorshape.Rotate(180).Print(xd, -yd, height, color));
        }

        private double Recurse(int x, int y, int fx, int fy, bool symmetric)
        {
            if (y - fy < 0 || x < 0 || (symmetric && (x > Math.Ceiling(r)) ) || (!symmetric && (x > xLimit)) ) return errorVal;
            if (y == 0 || (!symmetric && (x == xLimit))) return 0; //target achieved
            //stay within a band of 2 plates
            double rxy = Math.Sqrt((double)(x)*x+(double)(y)*y);
            if (Math.Abs(rxy - r) > band) return errorVal;
            var key = (x, y, fx, fy);
            var cache = symmetric?m_cache1 : m_cache2;
            if (cache.TryGetValue(key,out (double,Slope) val))
            {
                return val.Item1;
            }

            Slope bestSlope = m_slopes[0];
            double bestError = double.MaxValue;

            foreach(Slope slope in m_slopes) 
            {
                if (fy > 0 && slope.leftmargin > fx || x-slope.leftmargin < y) continue;
                int newX = x + slope.dx;
                int newY = y - slope.dy;
                if (newY - slope.bottommargin < 0) continue;
                int newfy = slope.bottommargin;
                int newfx = 0;
                if(slope==horzSlope && fy>0) //horz
                {
                    newfy = fy;
                    newfx = fx + 1;
                    if (newfx >= 2) //no limit imposed...
                    {
                        newfy = newfx = 0;
                    }
                }
                if (slope == vertSlope && fy>0) //vert
                {
                    newfy = fy - 1;
                    newfx = fx;
                    if (newfy <=0)//no limit imposed...
                    {
                        newfy = newfx = 0;
                    }

                }
                double candidateError = CalcError((newX,newY), (x,y),slope.rounded) + Recurse(newX,newY,newfx,newfy,symmetric);
                if (candidateError < bestError)
                {
                    bestError = candidateError;
                    bestSlope = slope;
                }
            }
            if (bestError >= errorVal) return errorVal; //no solution...
            cache.Add(key, (bestError, bestSlope));
            return bestError;
        }
        
        private double CalcError((double x, double y) p1, (double x, double y) p2, bool rounded=false) //code assumes in 1st quadrant and alpha1 < alpha2
        {
            if (rounded) return CalcErrorRoundPlate(p1,p2);
            double r1 = Math.Sqrt(p1.x * p1.x + p1.y * p1.y);
            double r2 = Math.Sqrt(p2.x * p2.x + p2.y * p2.y);
            double alpha1 = Math.Atan2(p1.y, p1.x);
            double alpha2 = Math.Atan2(p2.y, p2.x);
            if (Math.Abs(alpha2 - alpha1) < epsilon)
                return 0.0;
            System.Diagnostics.Debug.Assert(alpha2 > alpha1);
            double D = p1.x * p2.y - p2.x * p1.y;

            if (r1 < (r+epsilon) && r2 < (r+epsilon)) //segment entirely inside
            {
                return SectorArea(r, alpha1, alpha2) -0.5*D;
            }
            double dx = p2.x- p1.x;
            double dy = p2.y- p1.y; 
            double dr2 = dx * dx + dy * dy;
            double rootArg = r*r*dr2-D*D;
            if (rootArg < -epsilon) //no intersection
            {
                return 0.5*D-SectorArea(r, alpha1, alpha2);
            }
            double root = Math.Sqrt(rootArg);
            (double x, double y) p3 = ((D * dy + dx * root) / dr2, (-D * dx + dy * root) / dr2);
            double alpha3 = Math.Atan2(p3.y, p3.x);
            if (alpha1 < alpha3 - epsilon && alpha3 < alpha2-epsilon)
            {
                return CalcError (p1,p3)+CalcError(p3,p2);
            }
            p3 = ((D * dy - dx * root) / dr2, (-D * dx - dy * root) / dr2);
            alpha3 = Math.Atan2(p3.y, p3.x);
            if (alpha1 < alpha3 - epsilon && alpha3 < alpha2 - epsilon)
            {
                return CalcError( p1, p3) + CalcError(p3, p2);
            }
            //no intersection
            return 0.5 * D - SectorArea(r, alpha1, alpha2);
        }

        private double CalcErrorRoundPlate((double x, double y) p1, (double x, double y) p2)
        {
            double plateR = Math.Abs(p1.x - p2.x);
            if (plateR > r - epsilon) return errorVal; //do not consider plates larger than the target radius
            double r1 = Math.Sqrt(p1.x * p1.x + p1.y * p1.y);
            double r2 = Math.Sqrt(p2.x * p2.x + p2.y * p2.y);
            double alpha1 = Math.Atan2(p1.y, p1.x);
            double alpha2 = Math.Atan2(p2.y, p2.x);
            double D = p1.x * p2.y - p2.x * p1.y;
            
            (double x, double y) plateCenter = (p2.x, p1.y);
            if (r1 > (r - epsilon) && r2 > (r - epsilon)) //slope entirely inside
            {
                return 0.5*D + (0.25*Math.PI-0.5)*plateR*plateR - SectorArea(r, alpha1, alpha2);
            }
            var intersects = CircleIntersectionPoints(plateCenter,plateR).Select(p=>(p,Math.Atan2(p.y,p.x), Math.Atan2(p.y - plateCenter.y, p.x - plateCenter.x))).Where(pa=>
            {
                if (pa.Item2 < alpha1 + epsilon || pa.Item2 > alpha2 - epsilon) return false;
                if (pa.Item3 < epsilon || pa.Item3 > Math.PI * 0.5 - epsilon) return false;
                return true;
            }).OrderBy(t3=>t3.Item2).ToList();

            switch (intersects.Count)
            {
                case 0:
                    return SectorArea(r, alpha1, alpha2) - 0.5 * D - (0.25 * Math.PI - 0.5) * plateR * plateR;
                case 1:
                    {
                        (double x, double y) = intersects[0].p;
                        double tria1 = 0.5 * (p1.x * y - x * p1.y);
                        double tria2 = 0.5 * (x * p2.y - p2.x * y);
                        double anglePlate = intersects[0].Item3;
                        double seg1 = 0.5 * plateR * plateR * (anglePlate - Math.Sin(anglePlate));
                        double seg2 = 0.5 * plateR * plateR * (0.5 * Math.PI - anglePlate - Math.Cos(anglePlate));
                        if (r1 > r + epsilon && r2 < r - epsilon)
                        {
                            return tria1 + seg1 - SectorArea(r, alpha1, intersects[0].Item2) 
                                 - tria2 - seg2 + SectorArea(r, intersects[0].Item2, alpha2);
                        }
                        else if (r1 < r - epsilon && r2 > r - epsilon)
                        {
                            return tria2 + seg2 - SectorArea(r, intersects[0].Item2, alpha2) 
                                 - tria1 - seg1 + SectorArea(r, alpha1, intersects[0].Item2);
                        }
                        else System.Diagnostics.Debug.Assert(false);
                    }
                    break;
                case 2:
                    {                        
                        System.Diagnostics.Debug.Assert(r1 < (r - epsilon) && r2 < (r - epsilon));
                        (double x3, double y3) = intersects[0].p;
                        (double x4, double y4) = intersects[1].p;
                        double tria1 = 0.5 * (p1.x * y3 - x3 * p1.y);
                        double tria2 = 0.5 * (x3 * y4 - x4 * y3);
                        double tria3 = 0.5 * (x4 * p2.y - p2.x * y4);
                        double anglePlate1 = intersects[0].Item3;
                        double anglePlate2 = intersects[1].Item3-anglePlate1;
                        double anglePlate3 = 0.5 * Math.PI - intersects[1].Item3;
                        double seg1 = 0.5 * plateR * plateR * (anglePlate1 - Math.Sin(anglePlate1));
                        double seg2 = 0.5 * plateR * plateR * (anglePlate2 - Math.Sin(anglePlate2));
                        double seg3 = 0.5 * plateR * plateR * (anglePlate3 - Math.Sin(anglePlate3));
                        return tria2 + seg2 - SectorArea(r, intersects[0].Item2, intersects[1].Item2)
                             - tria1 - seg1 + SectorArea(r, alpha1, intersects[0].Item2)
                             - tria3 - seg3 + SectorArea(r, intersects[1].Item2, alpha2);
                    }
                default:
                    System.Diagnostics.Debug.Assert(false);
                    break;
            }
            return 0.0;
        }

        List<(double x, double y)> CircleIntersectionPoints((double x, double y) p1, double r1)
        {
            double d = Math.Sqrt(p1.x * p1.x + p1.y * p1.y);
            if (d > r + r1 - epsilon) return new(); //disjunct or touching -> can be handled the same as calling code, return empty list
            double a = (r * r - r1 * r1 + d * d) / (2 * d);
            double h = Math.Sqrt(r * r - a * a);
            (double xmid, double ymid) = (a * p1.x / d, a * p1.y / d);
            return new() { (xmid+ h * p1.y / d, ymid - h * p1.x / d), (xmid - h * p1.y / d, ymid + h * p1.x / d) };
        }



        //private static double TriangleArea((double x, double y) p1, (double x, double y) p2) => 0.5*(p1.x * p2.y - p2.x*p1.y);
        private static double SectorArea(double r, double alpha1, double alpha2) => r * r * (alpha2 - alpha1) * 0.5;


    }
}
