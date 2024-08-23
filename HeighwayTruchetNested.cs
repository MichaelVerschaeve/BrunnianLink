using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    public static class HeighwayTruchetNested
    {
        public static void Generate(StringBuilder sb, int level)
        {
            MetaData.StartSubModel(sb, $"HeighwayDragonII{level}");
            // List<string> someColorNames = new() { "Bright_Light_Orange", "Light_Bluish_Grey", "Coral", "Dark_Turquoise" };
            List<string> someColorNames = new() { "Coral", "Lime", "Dark_Turquoise", "Bright_Light_Orange" };
            var colorIds = someColorNames.Select(x => ColorMap.Get(x).id).ToList();
            Tile t = new Tile(1, 1);
            sb.AppendLine(t.Print(0.5, -0.5, 0, colorIds[0]));
            sb.AppendLine(t.Print(0.5, 0.5, 0, colorIds[1]));
            sb.AppendLine(t.Print(-0.5, 0.5, 0, colorIds[2]));
            sb.AppendLine(t.Print(-0.5, -0.5, 0, colorIds[3]));


            List<char> turns = new List<char>() { '+' };
            for (int i = 0; i < level; i++)
                turns = turns.Append('+').Concat(turns.Select(c => c == '+' ? '-' : '+').Reverse()).ToList();

            Dictionary<(int x, int y), List<int>> board = new();

            for (int i = 0; i < 4; i++)
            {
                int dir = i;
                int leftId = colorIds[(i + 1) % 4];
                int rightId = colorIds[i];
                (int x, int y) pos = (0, 0);
                foreach (char turn in turns)
                {
                    switch (dir)
                    {
                        case 0: pos.x++; break;
                        case 1: pos.y++; break;
                        case 2: pos.x--; break;
                        case 3: pos.y--; break;
                    }
                    if (!board.TryGetValue(pos, out List<int>? cell))
                    {
                        cell = new List<int>() { -1, -1, -1, -1, -1 };
                        board[pos] = cell;
                    }
                    if (turn == '+')
                    {
                        cell[4] = leftId;
                        cell[(dir + 2) % 4] = rightId;
                        dir = (dir + 3) % 4;
                    }
                    else
                    {
                        cell[4] = rightId;
                        dir = (dir + 1) % 4;
                        cell[dir] = leftId;
                    }
                }
            }

            foreach (var pair in board)
            {
                (int x, int y) pos = pair.Key;
                Shape cutout = new() { PartID = "3396" };
                if (pair.Value[1] != -1 || pair.Value[3] != -1)
                    cutout = cutout.Rotate();
                sb.AppendLine(cutout.Print(2 * pos.x, 2 * pos.y, 0, pair.Value[4]));
                Shape q = new() { PartID = "25269" };
                if (pair.Value[0] != -1)
                    sb.AppendLine(q.Rotate(270).Print(2 * pos.x + 0.5, 2 * pos.y + 0.5, 0, pair.Value[0]));
                if (pair.Value[1] != -1)
                    sb.AppendLine(q.Print(2 * pos.x - 0.5, 2 * pos.y + 0.5, 0, pair.Value[1]));
                if (pair.Value[2] != -1)
                    sb.AppendLine(q.Rotate(90).Print(2 * pos.x - 0.5, 2 * pos.y - 0.5, 0, pair.Value[2]));
                if (pair.Value[3] != -1)
                    sb.AppendLine(q.Rotate(180).Print(2 * pos.x + 0.5, 2 * pos.y - 0.5, 0, pair.Value[3]));
            }
        }
    }
}
