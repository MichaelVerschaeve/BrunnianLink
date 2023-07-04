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
}