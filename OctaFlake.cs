using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    public static class OctaFlake
    {
        static private readonly Shape elbow = new Shape() { PartID = "65473", SwitchYZ = true }.Rotate(90);
        static private readonly Shape axle = new() { PartID = "32062", SwitchYZ = true };

        static private readonly double rad = 3.6;

        public static void Generate(StringBuilder sb, int level)
        {
            double x = 0;
            double y = 0;
            int rot = 0;
            for (int i = 0; i < 8; i++)
            {
                Recurse(sb, level, true,ref x, ref y, ref rot);
            }
        }

        static private readonly int yellowId = ColorMap.Get("Yellow").id;
        static private readonly int redId = ColorMap.Get("Red").id;

        static private void Recurse(StringBuilder sb, int level, bool left, ref double x, ref double y, ref int rotation)
        {
            if (level == 0)
            {
                sb.AppendLine(axle.Rotate(rotation * 45).Print(x, y, 0, redId));
                if (left) 
                    sb.AppendLine(elbow.Rotate(rotation * 45).Print(x, y, 0, yellowId));

                double oldAngle = Math.PI * rotation * 0.25;
                int sign = left ? 1 : -1;
                x += rad * Math.Cos(oldAngle + sign * 0.5 * Math.PI);
                y += rad * Math.Sin(oldAngle + sign * 0.5 * Math.PI);
                if (left)
                {
                    rotation++;
                    if (rotation > 4)
                        rotation -= 8;
                }
                else
                {
                    rotation--;
                    if (rotation <= -4)
                        rotation += 8;
                }
                double newAngle = Math.PI * rotation * 0.25;
                x += rad * Math.Cos(newAngle - sign * 0.5 * Math.PI);
                y += rad * Math.Sin(newAngle - sign * 0.5 * Math.PI);

                if (!left)
                    sb.AppendLine(elbow.Rotate(rotation * 45+180).Print(x, y, 0, yellowId));
                return;
            }
            for (int i = 0; i < 3; i++)
                Recurse(sb, level - 1, left, ref x, ref y, ref rotation);
            for (int i = 0; i < 5; i++)
                Recurse(sb, level - 1, !left, ref x, ref y, ref rotation);
            for (int i = 0; i < 3; i++)
                Recurse(sb, level - 1, left, ref x, ref y, ref rotation);

        }

    }
}
