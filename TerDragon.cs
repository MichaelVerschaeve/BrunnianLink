using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BrunnianLink.TruchetFractals;

namespace BrunnianLink
{
    public static class TerDragon
    {

        public static void Generate(StringBuilder sb, int level)
        {
            MetaData.StartSubModel(sb, "TerDragon");
            List<bool> moves = new();

            for (int i = 0; i < level; i++)
                moves = moves.Append(false).Concat(moves).Append(true).Concat(moves).ToList();
            int idGreen = ColorMap.Get("Green").id;
            Shape leftTurn = new() { PartID = "left_turn", SubModel = true };
            Shape rightTurn = new() { PartID = "right_turn", SubModel = true };
            double s60 = Math.Sqrt(3.0) * 0.5;
            List<string> someColorNames = new() { "Red","Blue","Green","Orange","Purple","Black" };
            var colorIds = someColorNames.Select(x => ColorMap.Get(x).id).ToList();
            for (int rotation = 0; rotation < 6; rotation++)
                sb.AppendLine(new Shape() { PartID = $"rot{rotation}", SubModel=true }.Print(0, 0, 0, colorIds[rotation]));
            double d = Math.Sqrt(3.0) + 2;

            for (int rotation = 0; rotation < 6; rotation++)
            {
                MetaData.StartSubModel(sb, $"rot{rotation}");
                double x = d;
                double y = 0;

                double[] c = new[] { d, 0.5 * d, -0.5 * d, -d, -0.5 * d, 0.5 * d };
                double[] s = new[] { 0, s60 * d, s60 * d, 0, -s60 * d, -s60 * d, 0 };

                bool top = false;

                foreach (bool left in moves)
                {
                    sb.AppendLine((left ? leftTurn : rightTurn).Rotate(rotation * 60).Print(x, y, top ? 0.0 : -1.0, 16));
                    if (left)
                    {
                        rotation += 2;
                        if (rotation >= 6)
                            rotation -= 6;
                    }
                    else
                    {
                        rotation -= 2;
                        if (rotation < 0)
                            rotation += 6;
                    }
                    x += c[rotation];
                    y += s[rotation];
                    top = !top;
                }
            }
            MetaData.StartSubModel(sb, "left_turn");
            sb.AppendLine(rightTurn.Rotate(-60).Print(0, 0,0,16));
            MetaData.StartSubModel(sb, "right_turn");
            sb.AppendLine(new Shape() { PartID = "2429" }.Print(-s60,-0.5,0,16));
            sb.AppendLine(new Shape() { PartID = "2430" }.Rotate(-120).Print(-s60, -0.5, 0, 16));
        }
    }
}