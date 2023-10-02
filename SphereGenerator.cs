using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    public class SphereGenerator
    {
    }


    public class CircleAprroximator
    {
        private double r;
        private double errorVal;

        private const double epsilon = 1.0-7;

        struct Slope
        {
            public int leftmargin;
            public int bottommargin;
            public int dx;
            public int dy;
        }

        static List<Slope> m_slopes = new List<Slope>()
        {
            new Slope { leftmargin = 0, bottommargin = 0, dx = 1, dy = 0 },
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
            new Slope { leftmargin = 0, bottommargin = 0, dx = 0, dy = 1 }
        };

        public CircleAprroximator(double _r) { r = _r; errorVal = (double)(r) * r; }



        private Dictionary<(int x, int y, int bm), (double err, Slope s)> m_cache = new Dictionary<(int x, int y, int bm), (double, Slope)>();

        private double Recurse(int x, int y, int bottomMargin)
        {
            if (y==0) return 0; //target achieved
            if (y-bottomMargin < 0 || x < 0 || x > Math.Ceiling(r)) return errorVal;
            //stay within a band of 2 plates
            double rxy = Math.Sqrt((double)(x)*x+(double)(y)*y);
            if (Math.Abs(rxy - r) > 2) return errorVal;
            var key = (x, y, bottomMargin);
            if (m_cache.TryGetValue(key,out (double,Slope) val))
            {
                return val.Item1;
            }

            Slope bestSlope = m_slopes[0];
            double bestError = double.MaxValue;
            
            foreach (Slope slope in m_slopes)
            {
                if (bottomMargin > 0 && slope.leftmargin > 0) continue;
                int newX = x + slope.dx;
                int newY = y - slope.dy;
                int newBm = Math.Max(0, bottomMargin - slope.dy);
                double candidateError = CalcError((newX,newY), (x,y)) + Recurse(newX,newY,newBm);
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
            if (r1 < (r+epsilon) && r2 < (r+epsilon)) //segment entirely inside
            {
                return SectorArea(r, alpha1, alpha2) - TriangleArea(p1, p2);
            }
            double dx = p2.x- p1.x;
            double dy = p2.y- p1.y; 
            System.Diagnostics.Debug.Assert(dy >  epsilon);
            double dr2 = dx * dx + dy * dy;
            double D = p1.x * p2.y - p2.x * p1.y;
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
