using System.Text;

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
            List<string> someColorNames = new() { "Red", "Blue", "Green", "Orange", "Purple", "Black" };
            var colorIds = someColorNames.Select(x => ColorMap.Get(x).id).ToList();
            for (int rotation = 0; rotation < 6; rotation++)
                sb.AppendLine(new Shape() { PartID = $"rot{rotation}", SubModel = true }.Print(0, 0, 0, 16));
            double d = Math.Sqrt(3.0) + 2;
            double[] c = new[] { d, 0.5 * d, -0.5 * d, -d, -0.5 * d, 0.5 * d };
            double[] s = new[] { 0, s60 * d, s60 * d, 0, -s60 * d, -s60 * d, 0 };

            List<Shape> leftTurnsTop = someColorNames.Select(x => new Shape() { PartID = $"left_turn_top_{x}", SubModel = true }).ToList();
            List<Shape> rightTurnsTop = someColorNames.Select(x => new Shape() { PartID = $"right_turn_top_{x}", SubModel = true }).ToList();

            List<Shape> leftTurnsBottom = someColorNames.Select(x => new Shape() { PartID = $"left_turn_bottom_{x}", SubModel = true }).ToList();
            List<Shape> rightTurnsBottom = someColorNames.Select(x => new Shape() { PartID = $"right_turn_bottom_{x}", SubModel = true }).ToList(); ;

            for (int initialrotation = 0; initialrotation < 6; initialrotation++)
            {
                int rotation = initialrotation;
                MetaData.StartSubModel(sb, $"rot{rotation}");

                Shape leftTurnTop = leftTurnsTop[initialrotation];
                Shape rightTurnTop = rightTurnsTop[initialrotation];
                Shape leftTurnBottom = leftTurnsBottom[initialrotation];
                Shape rightTurnBottom = rightTurnsBottom[initialrotation];

                double x = c[rotation];
                double y = s[rotation];

                bool top = false;

                foreach (bool left in moves)
                {
                    sb.AppendLine((top ? (left ? leftTurnTop : rightTurnTop) : (left ? leftTurnBottom : rightTurnBottom)).Rotate(rotation * 60).Print(x, y, 0.0, 16));
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
            int colorId_base1 = ColorMap.Get("Light_Bluish_Grey").id;
            int colorId_base2 = ColorMap.Get("White").id;
            foreach (string color in someColorNames)
            {
                int colorId = ColorMap.Get(color).id;
                foreach (string top in new string[] { "top", "bottom" })
                {
                    MetaData.StartSubModel(sb, $"left_turn_{top}_{color}");
                    string rightShapeId = $"right_turn_{top}_{color}";
                    sb.AppendLine(new Shape() { PartID = rightShapeId, SubModel = true }.Rotate(-60).Print(0, 0, 0, colorId_base1));
                    MetaData.StartSubModel(sb, rightShapeId);
                    sb.AppendLine(new Shape() { PartID = "2429" }.Print(-s60, -0.5, top == "top" ? 0 : -1, colorId_base1));
                    sb.AppendLine(new Shape() { PartID = "2430" }.Rotate(-120).Print(-s60, -0.5, top == "top" ? 0 : -1, colorId_base1));
                    sb.AppendLine(new Shape() { PartID = "25269" }.Rotate(90).Print(-s60 - 0.5, 0.0, 1, colorId));
                    sb.AppendLine(new Shape() { PartID = "25269" }.Rotate(60).Print(-s60*0.5 - 0.25, -0.5*s60- 0.75, 1, colorId));
                }
                (colorId_base1, colorId_base2) = (colorId_base2, colorId_base1);
            }
        }
    }
}