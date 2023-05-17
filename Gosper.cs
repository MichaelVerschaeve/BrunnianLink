using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    public static class Gosper
    {
        //some consts...
        private const string A_Rule = "A-B--B+A++AA+B-";
        private const string B_Rule = "+A-BB--B-A++A+B";
        private static readonly int[] x_inc ={ 8, 4, -4, -8, -4, 4 };
        private static readonly int[] y_inc ={ 0, 6, 6, 0, -6, -6 };
        private static readonly int[] x_off = { 4, 4, -4, -4, 0, 0 };
        private static readonly int[] y_off = { 2, 2, 2, 2, -4, -4 };
        private static readonly int redId = ColorMap.Get("Red").id;
        private static readonly int blueId = ColorMap.Get("Blue").id;
        public static void Generate(StringBuilder sb, int level)
        {
            //write Header
            MetaData.StartSubModel(sb, "Gosper");
            string commands = "A";
            int rotation = 0;
            for (int i = 0; i < level; i++)
                commands = commands.ToLower().Replace("a", A_Rule).Replace("b", B_Rule);
            int x = 0;
            int y = 0;

            foreach (char c in commands)
            {
                switch (c)
                {
                    case '-':
                        rotation++;
                        if (rotation == 6)
                            rotation = 0;
                        break;
                    case '+':
                        rotation--;
                        if (rotation < 0)
                            rotation = 5;
                        break;
                    case 'A': goto case 'B';
                    case 'B':
                        System.Diagnostics.Debug.Write(rotation);
                        Shape subShape = new() { PartID = $"Forward{rotation}", SubModel = true };
                        sb.AppendLine(subShape.Print(x + x_off[rotation], y + y_off[rotation], 1, 16));
                        x += x_inc[rotation];
                        y += y_inc[rotation];
                        break;
                }
            }

            //write subElements
            Shape wedgeLeft = new() { PartID = "65429" };
            Shape wedgeRight = new() { PartID = "65426" };
            Shape triangle = new() { PartID = "35787" };
            Shape plate = new Plate(2, 4);
            Shape tile = new Tile(2, 4);
            for (char c = '0'; c < '6'; c++)
            {
                //bottom and top wedges
                MetaData.StartSubModel(sb, $"Forward{c}");
                sb.AppendLine(wedgeLeft.Rotate(-90).Print(-2, -3, 1, "015".Contains(c)?redId:blueId));
                sb.AppendLine(wedgeLeft.Rotate(90).Print(2, 3, 1, "123".Contains(c) ? redId : blueId));
                sb.AppendLine(wedgeRight.Rotate(90).Print(2, -3, 1, "015".Contains(c) ? redId : blueId));
                sb.AppendLine(wedgeRight.Rotate(-90).Print(-2, 3, 1, "345".Contains(c) ? redId : blueId));
                //center plates                
                sb.AppendLine(plate.Print(-1, 0, 1, "135".Contains(c) ? redId : blueId));
                sb.AppendLine(plate.Print(1, 0, 1, "135".Contains(c) ? redId : blueId));
                if ("14".Contains(c)) // left wedges
                {
                    sb.AppendLine(wedgeLeft.Print(-3, 0, 1, c== '4'? redId : blueId));
                    sb.AppendLine(wedgeLeft.Rotate(180).Print(-3, 0, 1, c=='1' ? redId : blueId));
                }
                else //left tile
                    sb.AppendLine(tile.Print(-3, 0, 1, "35".Contains(c) ? redId : blueId));
                //center tiles
                sb.AppendLine(tile.Print(-1, 0, 2, "135".Contains(c) ? redId : blueId));
                sb.AppendLine(tile.Print(1, 0, 2, "135".Contains(c) ? redId : blueId));
                if ("25".Contains(c)) // right wedges
                {
                    sb.AppendLine(wedgeRight.Print(3, 0, 1, c == '2' ? redId : blueId));
                    sb.AppendLine(wedgeRight.Rotate(180).Print(3, 0, 1, c == '5' ? redId : blueId));
                }
                else //right tiles
                    sb.AppendLine(tile.Print(3, 0, 1, "13".Contains(c) ? redId : blueId));
                //triangles
                sb.AppendLine(triangle.Rotate(180).Print(-1, -3, 2, "015".Contains(c) ? redId : blueId));
                sb.AppendLine(triangle.Rotate(-90).Print(1, -3, 2, "015".Contains(c) ? redId : blueId));
                sb.AppendLine(triangle.Rotate(90).Print(-1, 3, 2, "135".Contains(c) ? redId : blueId));
                sb.AppendLine(triangle.Print(1, 3, 2, "135".Contains(c) ? redId : blueId));
            }
        }
    }
}
