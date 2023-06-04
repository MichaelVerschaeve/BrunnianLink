using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    public class Poly : ICloneable
    {
        private List<(double x, double y)> m_points = new();
        public IEnumerable<(double x, double y)> Points { 
            
            get
            {
                double c = Math.Cos(RotationRad);
                double s = Math.Sin(RotationRad);
                foreach (var (x, y) in m_points)
                {
                    yield return (c*x-s*y, s*x + c*y);
                }
            }
            set
            {
                m_points = value.ToList();
                OffsetX = OffsetY = RotationRad = 0.0;
            }

        }
        public double OffsetX=0.0;
        public double OffsetY=0.0;
        public double RotationRad = 0.0;
        public double RotationDeg
        {
            get => RotationRad*180/Math.PI; 
            set => RotationRad = value*Math.PI/180;
        }

        public object Clone()
        {
            return new Poly()
            {
                Points = new List<(double x, double y)>(Points),
                OffsetX = OffsetX,
                OffsetY = OffsetY,
                RotationRad = RotationRad
            };
        }

        public void MirrorXThis()
        {
            OffsetX = -OffsetX;
            m_points = m_points.Select(p => (-p.x, p.y)).Reverse().ToList();
        }

        public Poly MirrorX() { Poly r = (this.Clone() as Poly)!; r.MirrorXThis(); return r; }

        public void FitThis(Poly arg, int argSideIndex, int thisSideIndex)
        {
#pragma warning disable IDE0042 // Deconstruct variable declaration
            (double x, double y) pArg1 = arg.Points.Skip(argSideIndex).First();
            (double x, double y) pArg2 = arg.Points.Skip((argSideIndex+1)%arg.Points.Count()).First();
            (double x, double y) pThisArg1 = Points.Skip(thisSideIndex).First();
            (double x, double y) pThisArg2 = Points.Skip((thisSideIndex + 1) % m_points.Count).First();
#pragma warning restore IDE0042 // Deconstruct variable declaration

            RotationRad += Math.Atan2(pArg1.y - pArg2.y, pArg1.x - pArg2.x) - Math.Atan2(pThisArg2.y - pThisArg1.y, pThisArg2.x - pThisArg1.x);
            if (RotationRad > Math.PI) 
                RotationRad -= 2* Math.PI;
            if (RotationRad <= Math.PI)
                RotationRad += 2 * Math.PI;

            pThisArg1 = Points.Skip(thisSideIndex).First();
            OffsetX += pArg2.x - pThisArg1.x;
            OffsetY += pArg2.y - pThisArg1.y;
        }

        public Poly Fit(Poly arg, int argSideIndex, int thisSideIndex) { Poly r = (this.Clone() as Poly)!; r.FitThis(arg,argSideIndex,thisSideIndex); return r; }


    }
}
