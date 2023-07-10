using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    public static class CrossDissectionTiling
    {
        private static readonly string[] colors = new[] { "Red", "Lime", "Medium_Azure", "Dark_Blue",  };

        public static void Generate(StringBuilder sb, int level)
        {
            MetaData.StartSubModel(sb,"CrossSquarePattern");
            for (int ix = 0; ix < level; ix++)
            {
                for (int iy = 0; iy < level; iy++)
                {
                    double x = 8.0 * ix - 4.0 * iy;
                    double y = -4.0 * ix - 8.0 * iy;
                    int squareType = (ix + iy) & 1;
                    Shape s = new() { PartID = $"Square_{squareType}", SubModel = true };
                    sb.AppendLine(s.Print(x, y, 0, 16));
                }
            }
            List<int> colorIDs = colors.Select(s=>ColorMap.Get(s).id).ToList();
            List<int> coslist = new() { 1, 0, -1, 0 };
            List<int> sinlist = new() { 0, 1, 0, -1 };
            Shape wedge = new() { PartID = "65429" };
            Shape lshape = new() { PartID = "2639" };
            for (int squareType = 0; squareType < 2; squareType++)
            {
                MetaData.StartSubModel(sb, $"Square_{squareType}");
                for (int irot = 0; irot < 4; irot++)
                {
                    int colorID = colorIDs[2*squareType + (irot&1)];
                    double angle = irot * 90.0;
                    double cos = (double) coslist[irot];
                    double sin = (double) sinlist[irot];
                    sb.AppendLine(wedge.Rotate(angle).Print(5*cos, 5*sin, 0, colorID));
                    sb.AppendLine(wedge.Rotate(angle+90).Print(4*cos-3*sin, 4*sin+3*cos, 0, colorID));
                    sb.AppendLine(lshape.Rotate(angle).Print(cos-sin, sin+cos, 0, colorID));
                }
            }
        }
    }
}