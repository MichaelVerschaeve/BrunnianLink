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
        private double r;
        private int xLimit;
        private double errorVal;
        int plateHeight;
        int color;

        private const double epsilon = 1.0-7;

        class Slope
        {
            public int leftmargin;
            public int bottommargin;
            public int dx;
            public int dy;
            public string id = "";
            public string mirrorId = "";
        }


        static Slope horzSlope = new Slope { leftmargin = 0, bottommargin = 0, dx = 1, dy = 0 };
        static Slope vertSlope = new Slope { leftmargin = 0, bottommargin = 0, dx = 0, dy = 1 };

        static List<Slope> m_slopes = new List<Slope>()
        {
            horzSlope,
            new Slope { leftmargin = 1, bottommargin = 1, dx = 1, dy = 1 },
            new Slope { leftmargin = 1, bottommargin = 1, dx = 2, dy = 2 },
            new Slope { leftmargin = 1, bottommargin = 1, dx = 3, dy = 3 },
            new Slope { leftmargin = 2, bottommargin = 2, dx = 4, dy = 4 },
            new Slope { leftmargin = 1, bottommargin = 1, dx = 7, dy = 7 },
            new Slope { leftmargin = 1, bottommargin = 0, dx = 1, dy = 2 },
            new Slope { leftmargin = 0, bottommargin = 0, dx = 2, dy = 4 },
            new Slope { leftmargin = 1, bottommargin = 0, dx = 1, dy = 3 },
            new Slope { leftmargin = 1, bottommargin = 0, dx = 1, dy = 4 },
            new Slope { leftmargin = 1, bottommargin = 0, dx = 1, dy = 6 },
            new Slope { leftmargin = 2, bottommargin = 2, dx = 8, dy = 8 },
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
        


        void PrintSlope(StringBuilder sb, Slope s, int x, int y, bool octoCopy=true)
        {
            Shape shape = new Shape() { PartID = s.id}.Rotate(s.dx==s.dy?90:180);
            Shape mirrorshape = new Shape() { PartID = s.mirrorId }.Rotate(180);
            double xd = x -s.leftmargin + (s.leftmargin + s.dx) * 0.5;
            double yd = (s.bottommargin + s.dy) * 0.5;
            sb.AppendLine(shape.Print(xd, yd, plateHeight, color));
            sb.AppendLine(mirrorshape.Print(-xd, yd, plateHeight, color));
            sb.AppendLine(shape.Rotate(180).Print(-xd, -yd, plateHeight, color));
            sb.AppendLine(mirrorshape.Rotate(180).Print(xd, -yd, plateHeight, color));
            if (octoCopy) //mirrored allong xy 
            {
                (shape, mirrorshape) = (mirrorshape.Rotate(-90), shape.Rotate(90));
                (xd, yd) = (yd, xd);
                sb.AppendLine(shape.Print(xd, yd, plateHeight, color));
                sb.AppendLine(mirrorshape.Print(-xd, yd, plateHeight, color));
                sb.AppendLine(shape.Rotate(180).Print(-xd, -yd, plateHeight, color));
                sb.AppendLine(mirrorshape.Rotate(180).Print(xd, -yd, plateHeight, color));
            }
        }

        private Dictionary<(int x, int y, int fwidth,  int fy), (double err, Slope s)> m_cache = new Dictionary<(int x, int y, int fwidth, int fy), (double, Slope)>();

        public void Generate(StringBuilder sb)
        {
            int bestr = 0;
            int bestSlope = 0;
            double bestError = double.MaxValue;
            var EvenSlopes = m_slopes.Where(s => s.dx == s.dy && (s.dx & 1) == 0);
            var OddSlopes = m_slopes.Where(s => s.dx == s.dy && (s.dx & 1) == 0);
            int lowlimit = (int)Math.Ceiling((r - 2) / Math.Sqrt(2.0));
            int uplimit = (int)Math.Floor((r + 2) / Math.Sqrt(2.0));

            for (int r = lowlimit; r <= uplimit; r++)
            {
                double candidateError = Recurse(r, r, 0, 1);
                if (candidateError < bestError)
                {
                    bestError = candidateError;
                    bestSlope = 0;
                    bestr = r;
                }
                foreach (Slope s in EvenSlopes)
                {
                    int newx = r + s.dx / 2;
                    int newy = r + s.dy / 2;
                    candidateError = CalcError((newx, newy), (r, r)) + Recurse(newx,newy, 0, 1);
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
                    int newy = (int)(r + s.dy * 0.5);
                    double candidateError = CalcError((newx, newy), (r, r)) + Recurse(newx, newy, 0, 1);
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
            int x = 0, y = 0, fx=0, fy=0;
            if (bestSlope != 0) //one diagonal element
            {
                if ((bestSlope & 1) == 0) //even
                {
                    lastSlope = EvenSlopes.First(s=>s.dx == bestSlope);
                    PrintSlope(sb, lastSlope, bestr- bestSlope / 2, bestr + bestSlope / 2, false);
                    x = bestr + bestSlope / 2;
                    y = bestr - bestSlope / 2;
                }
                else
                {
                    lastSlope = OddSlopes.First(s => s.dx == bestSlope);
                    PrintSlope(sb, lastSlope, (int)(bestr+0.5 - 0.5*bestSlope),(int)(bestr+0.5 + 0.5*bestSlope), false);
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

            List<bool> lineCommands = new List<bool>();
            while (m_cache.TryGetValue((x, y, fx, fy), out (double err, Slope s) val))
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
                            lineCommands = lineCommands.ToArray().Reverse().Concat(lineCommands).ToList();
                            HandleLineCommands(sb, lineCommands, x, y, slope.leftmargin, slope.leftmargin, false);
                        }
                        else
                            HandleLineCommands(sb, lineCommands, x, y, lastSlope.bottommargin, slope.leftmargin);
                        lineCommands.Clear();
                    }
                    lastSlope = slope;
                    PrintSlope(sb, lastSlope, x,y);
                }
            }
            lineCommands.AddRange(Enumerable.Repeat<bool>(false,y));
            if (lineCommands.Count > 0)
            {
                if (lastSlope == null)
                {
                    lineCommands = lineCommands.ToArray().Reverse().Concat(lineCommands).ToList();
                    HandleLineCommands(sb, lineCommands, x, 0, 2, 2, false);
                }
                else
                    HandleLineCommands(sb, lineCommands, x, 0, lastSlope.bottommargin, 2);
            }
        }


        void HandleLineCommands(StringBuilder sb, List<bool> lineCommands, int x, int y, int bottommarginPrev, int leftMarinNext, bool octoCopy = true)
        {



        }


        private double Recurse(int x, int y, int fx, int fy)
        {
            if (y - fy < 0 || x < 0 || x > Math.Ceiling(r)) return errorVal;
            if (y == 0 || x == xLimit) return 0; //target achieved
            //stay within a band of 2 plates
            double rxy = Math.Sqrt((double)(x)*x+(double)(y)*y);
            if (Math.Abs(rxy - r) > 2) return errorVal;
            var key = (x, y, fx, fy);
            if (m_cache.TryGetValue(key,out (double,Slope) val))
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
                double candidateError = CalcError((newX,newY), (x,y)) + Recurse(newX,newY,newfx,newfy);
                if (candidateError < bestError)
                {
                    bestError = candidateError;
                    bestSlope = slope;
                }
            }
            if (bestError >= errorVal) return errorVal; //no solution...
            m_cache.Add(key, (bestError, bestSlope));
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

        private double TriangleArea((double x, double y) p1, (double x, double y) p2) => 0.5*(p1.x * p2.y - p2.x*p1.y);
        private double SectorArea(double r, double alpha1, double alpha2) => r * r * (alpha2 - alpha1) * 0.5;


    }
}
