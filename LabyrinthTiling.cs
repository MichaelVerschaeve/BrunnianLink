using System.Text;

namespace BrunnianLink
{
    public static class LabyrinthTiling
    {
        static readonly int cornerId = ColorMap.Get("Dark_Blue").id;
        static readonly int trapId = ColorMap.Get("Tan").id;
        static readonly int squareId = ColorMap.Get("Orange").id;

        public enum Part { Square, Corner, Trapezium}

        public static void Generate(StringBuilder sb, int level)
        {
            MetaData.StartSubModel(sb, $"LabyrinthII_{level}");

            GenerateRec(sb, Part.Square, new Rot45Coords(), 0, level);

            //sb.AppendLine(new Tile(2, 2).Print(0, 0, -1, 4)); //mark center

            Shape wedgeLeft = new() { PartID = "65426" };  //   \|
            Shape wedgeRight = new() { PartID = "65429" }; //   |/
            MetaData.StartSubModel(sb, "Corner");
            sb.AppendLine(new Plate(4, 4).Print(2, 2, 0, cornerId));
            sb.AppendLine(wedgeLeft.Rotate(180).Print(5, 2, 0, cornerId));
            sb.AppendLine(wedgeRight.Rotate(90).Print(2, 5, 0, cornerId));
            MetaData.StartSubModel(sb, "Trapezium");
            sb.AppendLine(new Plate(2, 4).Print(3, 2, 0, trapId));
            sb.AppendLine(wedgeLeft.Rotate(180).Print(5, 2, 0, trapId));
            sb.AppendLine(wedgeRight.Rotate(180).Print(1, 2, 0, trapId));

        }

        //square is at zero degrees parallel to x and y axises and origin is in bottom corner
        public static void GenerateRec(StringBuilder sb, Part p, Rot45Coords c, int rotation, int level)
        {
            if (level == 0)
            {
                switch (p)
                {
                    case Part.Square:
                        Rot45Coords otherPoint = c + (X + Y).Rotate(rotation);
                        double midx = (c.Cx + otherPoint.Cx) * 0.5;
                        double midy = (c.Cy + otherPoint.Cy) * 0.5;
                        sb.AppendLine(new Plate(6, 6).Print(midx, midy, 0, squareId));
                        return;
                    case Part.Corner:
                        sb.AppendLine(new Shape() { PartID = "Corner", SubModel = true }.Rotate(rotation).Print(c.Cx, c.Cy, 0, 16));
                        return;
                    case Part.Trapezium:
                        sb.AppendLine(new Shape() { PartID = "Trapezium", SubModel = true }.Rotate(rotation).Print(c.Cx, c.Cy, 0, 16));
                        return;
                }
            }
            c += c.Rotate(45) + c.Rotate(360 - 45); //enlarge
            level--;
            GenerateRec(sb, Part.Square, c + V.Rotate(rotation), rotation, level);
            if (p != Part.Trapezium)    
                c += V.Rotate(rotation) + Y.Rotate(rotation) - U.Rotate(rotation);
            for (int i = (p==Part.Trapezium?0:-1); i < 3; i++)
            {
                int subrotation = (360 + rotation + i * 90) % 360;
                Rot45Coords Xr = X.Rotate(subrotation);
                Rot45Coords Ur = U.Rotate(subrotation);
                Rot45Coords Vr = V.Rotate(subrotation);
                GenerateRec(sb, Part.Corner, c, subrotation, level);
                if (p != Part.Square && i == 1)
                    break;
                GenerateRec(sb, Part.Trapezium, c+Xr+Vr, (subrotation + 180) % 360, level);
                c += Ur + Xr + Vr;
            }
        }

        private static Rot45Coords X = new() { x = 1 };
        private static Rot45Coords U = new() { u = 1 };
        private static Rot45Coords V = new() { v = 1 };
        private static Rot45Coords Y = new() { y = 1 };

        public struct Rot45Coords
        {
            public int x; //0 degrees
            public int u; //-45 degrees
            public int v;//45 degrees
            public int y; //90 degrees
            public readonly int Cx => 4 * (u + v) + 6 * x;
            public readonly int Cy => 4 * (v - u) + 6 * y;

            public readonly Rot45Coords Rotate(int rot)
            {
                Rot45Coords res = this;
                for (int r = 0; r < rot; r += 45)
                    (res.x, res.u, res.v, res.y) = (res.u, -res.y, res.x, res.v);
                return res;
            }
            public static Rot45Coords operator +(Rot45Coords a, Rot45Coords b)
            => new() { x = a.x + b.x, u = a.u + b.u, v = a.v + b.v, y = a.y + b.y };
            public static Rot45Coords operator -(Rot45Coords a, Rot45Coords b)
            => new() { x = a.x - b.x, u = a.u - b.u, v = a.v - b.v, y = a.y - b.y };
            public static Rot45Coords operator -(Rot45Coords a)
            => new() { x = -a.x, u = -a.u, v = -a.v, y = -a.y };
            public static Rot45Coords operator *(int s, Rot45Coords a)
            => new() { x = s * a.x, u = s * a.u, v = s * a.v, y = s * a.y };

            public Rot45Coords(int x, int u, int v, int y)
            {
                this.x = x; this.u = u; this.v = v; this.y = y;
            }
        }
    }
}
