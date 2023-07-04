using System.Text;

namespace BrunnianLink
{
    public static class TruchetTiling
    {
        public static void Generate(StringBuilder sb, int level)
        {
            MetaData.StartSubModel(sb, "TruchetTiling");
            Shape[] shapes = new[] { new Shape(){ PartID = "SubSquare_0", SubModel = true }, new Shape() { PartID = "SubSquare_1", SubModel = true } };
            var rand = new Random();
            int blackID = ColorMap.Get("Black").id;
            int orangeID = ColorMap.Get("Bright_Light_Orange").id;

            for (int iy = 0; iy < level; iy++)
            {
                for (int ix = 0; ix < level; ix++)
                {
                    bool rotate = rand.Next(2) == 1;
                    bool odd = ((ix + iy) & 1) == 1;
                    sb.AppendLine(shapes[rotate==odd?0:1].Rotate(rotate?90.0:0).Print(2.0 * ix - level+1, 2.0 * iy - level+1, 0, 16));

                }
            }

            for (int i=0; i < 2; i++)
            {
                MetaData.StartSubModel(sb, $"SubSquare_{i}");
                sb.AppendLine(new Shape() { PartID = "3396" }.Print(0.0, 0.0, 0.0, i==0?blackID:orangeID));
                sb.AppendLine(new Shape() { PartID = "25269" }.Rotate(-90).Print(0.5, 0.5, 0.0, i == 1 ? blackID : orangeID));
                sb.AppendLine(new Shape() { PartID = "25269" }.Rotate(90).Print(-0.5, -0.5, 0.0, i == 1 ? blackID : orangeID));
            }
        }
    }

    public static class TruchetTwinHeighwayDragon
    {
        public static void Generate(StringBuilder sb, int level)
        {
            MetaData.StartSubModel(sb, "TruchetTwinDragon");
            List<bool> moves = new () { true};
            for (int i = 0; i < level; i++)
                moves = moves.Concat(moves.Select(b=>!b).Reverse().Prepend(true)).ToList();
            moves = moves.Concat(moves.Prepend(true)).Append(true).ToList(); //twin dragon, just close the loop
            Dictionary<(int x, int y), int> board = new();
            (int x, int y) position = (0,0);
            (int dx, int dy) direction = (0,1);
            foreach (bool right in moves)
            {
                if (!board.TryGetValue(position, out int cornerstatus))
                    board.Add(position, cornerstatus = 0);
                if (right)
                {
                    cornerstatus |= direction switch
                    {
                        (0, 1) => 1,
                        (-1, 0) => 2,
                        (0, -1) => 4,
                        (1, 0) => 8,
                        _ => throw new Exception("illegal direction")
                    };
                    direction = (direction.dy, -direction.dx);
                }
                else
                {
                    cornerstatus = direction switch
                    {
                        (0, 1) => -1,
                        (-1, 0) => -2,
                        (0, -1) => -1,
                        (1, 0) => -2,
                        _ => throw new Exception("illegal direction")
                    };
                    direction = (-direction.dy, direction.dx);
                }
                board[position] = cornerstatus;
                position.x += direction.dx;
                position.y += direction.dy;
            }
            System.Diagnostics.Debug.Assert(position == (0, 0));
            System.Diagnostics.Debug.Assert(direction == (0,1));
            int xfrom = board.Keys.Select(t => t.x).Min();
            int xto = board.Keys.Select(t => t.x).Max();
            int yfrom = board.Keys.Select(t => t.y).Min();
            int yto = board.Keys.Select(t => t.y).Max();

            int outID = ColorMap.Get("Black").id;
            int InID = ColorMap.Get("Bright_Light_Orange").id;

            for (double x = 2.0*xfrom; x <= 2.0*xto; x += 2.0 )
            {
                for (double y = 2.0 * yfrom; y <= 2.0 * yto; x += 2.0)
                {
                    if (!board.TryGetValue(position, out int cornerstatus))
                    {
                        sb.AppendLine(new Tile(2, 2).Print(x, y, 0.0, outID));
                        continue;
                    }
                    bool doRot = false;
                    int topId = outID;
                    int centerId = outID;
                    int bottomId = outID;
                    switch (cornerstatus)
                    {
                        case -1:
                            centerId = InID; 
                            break;
                        case -2:
                            centerId = InID;
                            doRot = true;
                            break;



                        default: throw new Exception("illegal board status");
                    }

                    if (doRot)
                    {
                        sb.AppendLine(new Shape() { PartID = "3396" }.Print(0.0, 0.0, 0.0, centerId));
                        sb.AppendLine(new Shape() { PartID = "25269" }.Print(-0.5, 0.5, 0.0, topId));
                        sb.AppendLine(new Shape() { PartID = "25269" }.Rotate(180).Print(0.5, -0.5, 0.0, bottomId));
                    }
                    else
                    {
                        sb.AppendLine(new Shape() { PartID = "3396" }.Print(0.0, 0.0, 0.0, centerId));
                        sb.AppendLine(new Shape() { PartID = "25269" }.Rotate(-90).Print(0.5, 0.5, 0.0, topId));
                        sb.AppendLine(new Shape() { PartID = "25269" }.Rotate(90).Print(-0.5, -0.5, 0.0, bottomId));
                    }
                }

            }


        }

    }
}