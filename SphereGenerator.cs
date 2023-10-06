using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace BrunnianLink
{
    public class SphereGenerator
    {
    }


    public class CircleAprroximator
    {
        private readonly double r;
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
            vertSlope
        };

        public CircleAprroximator(double _r, int _xlimit, int _plateHeight, int colorID)
        {
            r = _r;
            errorVal = (double)(r) * r;
            xLimit = _xlimit;
            plateHeight = _plateHeight;
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

        public void Generate(StringBuilder sb)
        {
            int bestr = 0;
            int bestSlope = 0;
            double bestError = double.MaxValue;
            var EvenSlopes = m_slopes.Where(s => s.dx == s.dy && (s.dx & 1) == 0);
            var OddSlopes = m_slopes.Where(s => s.dx == s.dy && (s.dx & 1) == 1);
            int lowlimit = (int)Math.Ceiling((r - band) / Math.Sqrt(2.0));
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
                }
                foreach (Slope s in EvenSlopes)
                {
                    int newx = r + s.dx / 2;
                    int newy = r - s.dy / 2;
                    candidateError = CalcError((newx, newy), (r, r));
                    if (symmetric)
                        candidateError += Recurse(newx, newy, 0, 1, true);
                    else
                    {
                        candidateError *= 2.0;
                        candidateError += Recurse(newx, newy, 0, 1, false);
                    }
                    if (candidateError < bestError)
                    {
                        bestError = candidateError;
                        bestSlope = s.dx;
                        bestr = r;
                    }
                }
            }
            for (double r = lowlimit - 0.5; r <= uplimit + 0.5; r += 1.0)
            {
                foreach (Slope s in OddSlopes)
                {
                    int newx = (int)(r + s.dx * 0.5);
                    int newy = (int)(r - s.dy * 0.5);
                    double candidateError = CalcError((newx, newy), (r, r));
                    if (symmetric)
                        candidateError += Recurse(newx, newy, 0, 1, true);
                    else
                    {
                        candidateError *= 2.0;
                        candidateError += Recurse(newx, newy, 0, 1, false);
                    }
                    if (candidateError < bestError)
                    {
                        bestError = candidateError;
                        bestSlope = s.dx;
                        bestr = (int)r;
                    }
                }
            }


            //now reconstruct and print
            Slope? lastSlope = null;
            int x = 0, y = 0, fx = 0, fy = 0;
            if (bestSlope != 0) //one diagonal element
            {
                if ((bestSlope & 1) == 0) //even
                {
                    lastSlope = EvenSlopes.First(s => s.dx == bestSlope);
                    PrintSlope(sb, lastSlope, bestr - bestSlope / 2, bestr + bestSlope / 2, PrintFlag.Quadrants);
                    x = bestr + bestSlope / 2;
                    y = bestr - bestSlope / 2;
                }
                else
                {
                    lastSlope = OddSlopes.First(s => s.dx == bestSlope);
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
                                lineCommands = lineCommands.ToArray().Reverse().Concat(lineCommands).ToList();
                                HandleLineCommands(sb, lineCommands, x, y, slope.leftmargin, slope.leftmargin, PrintFlag.Quadrants);
                            }
                            else
                                HandleLineCommands(sb, lineCommands, x, y, 0, slope.leftmargin, flags);
                        }
                        else
                            HandleLineCommands(sb, lineCommands, x, y, lastSlope.bottommargin, slope.leftmargin,flags);
                        lineCommands.Clear();
                    }
                    lastSlope = slope;
                    PrintSlope(sb, lastSlope, x,y,flags);
                    x += slope.dx;
                    y -= slope.dy;
                    fy = slope.bottommargin;
                    fx = 0;
                }
            }
            lineCommands.AddRange(Enumerable.Repeat<bool>(false,y));
            if (lineCommands.Count > 0)
            {
                HandleLineCommands(sb, lineCommands, x, 0, lastSlope?.bottommargin??0, 2, flags);
            }
        }


        void HandleLineCommands(StringBuilder sb, List<bool> lineCommands, int x, int y, int bottommarginPrev, int leftMarginNext, PrintFlag printFlag)
        {
            while (bottommarginPrev > 0 && lineCommands.Count > 0 && !lineCommands[0]) 
            { 
                lineCommands.RemoveAt(0);
                bottommarginPrev--;
            }
            while (leftMarginNext > 0 && lineCommands.Count > 0 && lineCommands.Last())
            {
                lineCommands.RemoveAt(lineCommands.Count - 1);
                leftMarginNext--;
                x--;
            }
            if (lineCommands.Count == 0) return;
            int w = lineCommands.Count(b => b);
            int h = lineCommands.Count(b => !b);
            if (bottommarginPrev == 0 && y+h<x-w) //spare room
            { 
                lineCommands.Insert(0, true);
                w++;
            }
            if(leftMarginNext == 0 && y>0) //spare room;
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
                    PrintRectange(sb,x, y, w, y - h > miny ? (h + 1) : h, printFlag);
                x += w;
                y -= h;
            }
        }

        void PrintRectange(StringBuilder sb, int x, int y, int w, int h, PrintFlag flag) 
        {
            if (w <= 0 || h <= 0) return;
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
                PrintRectange(sb, x, y, w / 2, h, flag);
                PrintRectange(sb, x + w/2, y, w-w/2, h, flag);
            }
            else
            {
                PrintRectange(sb, x, y, w ,h/2, flag);
                PrintRectange(sb, x, y-h/2, w, h-h/2, flag);
            }
        }

        void PrintSlope(StringBuilder sb, Slope s, int x, int y, PrintFlag flag = PrintFlag.All)
        {
            Shape shape = new Shape() { PartID = s.id }.Rotate(s.dx == s.dy ? 90 : 180);
            Shape mirrorshape = new Shape() { PartID = s.mirrorId }.Rotate(180);
            double xd = x - s.leftmargin + (s.leftmargin + s.dx) * 0.5;
            double yd = y - (s.bottommargin + s.dy) * 0.5;
            if ((flag & PrintFlag.Octant1) == PrintFlag.Octant1) sb.AppendLine(shape.Print(xd, yd, plateHeight, color));
            if ((flag & PrintFlag.Octant4) == PrintFlag.Octant4) sb.AppendLine(mirrorshape.Print(-xd, yd, plateHeight, color));
            if ((flag & PrintFlag.Octant5) == PrintFlag.Octant5) sb.AppendLine(shape.Rotate(180).Print(-xd, -yd, plateHeight, color));
            if ((flag & PrintFlag.Octant8) == PrintFlag.Octant8) sb.AppendLine(mirrorshape.Rotate(180).Print(xd, -yd, plateHeight, color));
            (shape, mirrorshape) = (mirrorshape.Rotate(-90), shape.Rotate(90));
            (xd, yd) = (yd, xd);
            if ((flag & PrintFlag.Octant2) == PrintFlag.Octant2) sb.AppendLine(shape.Print(xd, yd, plateHeight, color));
            if ((flag & PrintFlag.Octant3) == PrintFlag.Octant3) sb.AppendLine(mirrorshape.Print(-xd, yd, plateHeight, color));
            if ((flag & PrintFlag.Octant6) == PrintFlag.Octant6) sb.AppendLine(shape.Rotate(180).Print(-xd, -yd, plateHeight, color));
            if ((flag & PrintFlag.Octant7) == PrintFlag.Octant7) sb.AppendLine(mirrorshape.Rotate(180).Print(xd, -yd, plateHeight, color));
        }

        private double Recurse(int x, int y, int fx, int fy, bool symmetric)
        {
            if (y - fy < 0 || x < 0 || (!symmetric && x > Math.Ceiling(r) ) ) return errorVal;
            if (y == 0 || (!symmetric && x == xLimit)) return 0; //target achieved
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
                double candidateError = CalcError((newX,newY), (x,y)) + Recurse(newX,newY,newfx,newfy,symmetric);
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
        
        private double CalcError((double x, double y) p1, (double x, double y) p2) //code assumes in 1st quadrant and alpha1 < alpha2
        {
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
            System.Diagnostics.Debug.Assert(dy >  epsilon);
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
            throw new Exception("calculations incorrect");
        }

        //private static double TriangleArea((double x, double y) p1, (double x, double y) p2) => 0.5*(p1.x * p2.y - p2.x*p1.y);
        private static double SectorArea(double r, double alpha1, double alpha2) => r * r * (alpha2 - alpha1) * 0.5;


    }
}
