using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace BrunnianLink
{
    public class MazeGenerator
    {
        public enum Direction { Left, Up, Down, Right };

        public static Direction Opposite(Direction direction) => (Direction)(3 - direction);

        public class Cell : IEquatable<Cell>
        {
            public Wall[] Walls = new Wall[4];
            public bool AssignedToMaze = false;
            public bool Visited = false;

            private static int idCounter;
            private readonly int id;
            public Cell() { id = idCounter++; }
            public bool Equals(Cell? other)
            {
               if (other== null) return false;
               return other.id == id;
            }

            internal Direction FindCell(Cell newCell)
            {
                for (int i = 0; i < 4; i++)
                    if (Walls[i].OtherSide(this) == newCell)
                        return (Direction)(i);
                return 0;
            }

            public override bool Equals(object? obj) => Equals(obj as Cell);

            public override int GetHashCode()
            {
                return id.GetHashCode();
            }
        }

        public class Wall
        {
            public bool Open = false;
            readonly Cell?[] Sides;

            public Wall(Cell? c1, Cell? c2)
            {
                Sides = new Cell?[2] { c1, c2 };
            }

            public Cell? OtherSide(Cell arg) => arg == Sides[0] ? Sides[1] : Sides[0];
        }

        public Cell[,] maze;

        private readonly int n;

        public MazeGenerator(int level)
        {
            int size = 1 << ((level >> 1) + 2);
            if ((level & 1) == 1)
                size += (size >> 1);
            n = size / 2 - 1;

            maze = new Cell[n, n];
            HashSet<Cell> unassigned = new();

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                {
                    unassigned.Add(maze[i, j] = new Cell());
                }

            //set up walls;

            for (int j = 0; j < n; j++)
            {
                //inner walls...
                for (int i = 0; i < n - 1; i++)
                {
                    Wall w = new(maze[j, i], maze[j, i + 1]);
                    maze[j, i].Walls[(int)Direction.Up] = w;
                    maze[j, i + 1].Walls[(int)Direction.Down] = w;
                    w = new Wall(maze[i, j], maze[i + 1, j]);
                    maze[i, j].Walls[(int)Direction.Right] = w;
                    maze[i + 1, j].Walls[(int)Direction.Left] = w;
                }
                //sides
                maze[j, n - 1].Walls[(int)Direction.Up] = new Wall(maze[j, n - 1], null);
                maze[j, 0].Walls[(int)Direction.Down] = new Wall(null, maze[j, 0]);
                maze[n-1, j].Walls[(int)Direction.Right] = new Wall(maze[n-1, j],null);
                maze[0,j].Walls[(int)Direction.Left] = new Wall(null, maze[0, j] );
            }

            Random rand = new();

            Cell firstPick = unassigned.Skip(rand.Next(unassigned.Count)).First();
            firstPick.AssignedToMaze = true;
            unassigned.Remove(firstPick);

            while (unassigned.Count > 0)
            {
                Cell lastVisited = unassigned.Skip(rand.Next(unassigned.Count)).First();
                List<Cell> path = new() { lastVisited };
                lastVisited.Visited = true;
                Direction lastDirection = (Direction)rand.Next(4);
                while (!lastVisited.AssignedToMaze)
                {
                    Direction newDirection = (Direction)((int)(lastDirection + 1 + rand.Next(3)) % 4);
                    if (newDirection == Opposite(lastDirection)) newDirection = lastDirection; //don't go back
                    Cell? newCell = lastVisited.Walls[(int)newDirection].OtherSide(lastVisited);
                    if (newCell == null) continue;
                    if (newCell.Visited) //loopback
                    {
                        while (path.Last() != newCell)
                        {
                            path.Last().Visited = false;
                            path.RemoveAt(path.Count - 1);
                        }
                        lastVisited = newCell;
                        if (path.Count > 1)
                        { //get last direction again from last two cells
                            lastDirection = path[^2].FindCell(newCell);
                        }
                        else //reset,
                            lastDirection = (Direction)rand.Next(4);
                    }
                    else
                    {
                        newCell.Visited = true;
                        lastDirection = newDirection;
                        lastVisited = newCell;
                        path.Add(lastVisited);
                    }
                }
                //add path to maze
                Cell? prevCell = null;
                foreach (Cell cell in path)
                {
                    cell.Visited = false;
                    cell.AssignedToMaze = true;
                    unassigned.Remove(cell);
                    if (prevCell != null)
                    {
                        prevCell.Walls[(int)prevCell.FindCell(cell)].Open = true;
                    }
                    prevCell = cell;
                }
            }
        }

        public void Generate(StringBuilder sb)
        {
            MetaData.StartSubModel(sb, $"Maze_{n}x{n}");
            Shape borderTop = new() { PartID = "4865b" };
            Shape borderLeft = borderTop.Rotate();
            Shape borderBottom = borderLeft.Rotate();
            Shape borderRight = borderBottom.Rotate();
            Tile horzTile = new(2);
            Tile vertTile = new(1, 2);
            int tanID = ColorMap.Get("Tan").id;
            //border

            int grayID = ColorMap.Get("Light_Bluish_Grey").id;

            if (n == 31)
            {
                BasePlate bp = new(32);

                for (int x = 0; x < 2; x++)
                    for (int y = 0; y < 2; y++)
                        sb.AppendLine(bp.Print(15 + 32 * x, 15 + 32 * y, -1, grayID));
            }
            else if (n == 23)
            {
                BasePlate bp = new(48);
                sb.AppendLine(bp.Print(23, 23, -1, grayID));
            }
            for (int i = 0; i < n; i++)
            {
                if ((i & 7) == 0)
                    MetaData.BuildStepFinished(sb);
                sb.AppendLine(borderRight.Print(-0.5, 1 + 2 * i, 2, tanID));
                sb.AppendLine(vertTile.Print(2 * n + 0.5, 1 + 2 * i, 0, tanID));
                sb.AppendLine(borderTop.Print(1 + 2 * i, -0.5, 2, tanID));
                sb.AppendLine(horzTile.Print(1 + 2 * i, 2 * n + 0.5, 0, tanID));
            }
            Tile one = new(1);
            sb.AppendLine(one.Print(-0.5, -0.5, 0, tanID));
            sb.AppendLine(one.Print(-0.5, 2 * n + 0.5, 0, tanID));
            sb.AppendLine(one.Print(2 * n + 0.5, -0.5, 0, tanID));
            sb.AppendLine(one.Print(2 * n + 0.5, 2 * n + 0.5, 0, tanID));

            Shape borderTopRight = new Shape() { PartID = "91501" }.Rotate(-90);

            Tile two = new(2, 2);

            for (int x = 0; x < n; x++)
                for (int y = 0; y < n; y++)
                {
                    if ((y & 7) == 0)
                        MetaData.BuildStepFinished(sb);
                    double centerX = 1.0 + 2 * x;
                    double centerY = 1.0 + 2 * y;
                    if (maze[x, y].Walls[(int) Direction.Up].Open && maze[x, y].Walls[(int)Direction.Right].Open)
                    {
                        sb.AppendLine(two.Print(centerX, centerY, 0.0, tanID));
                    }
                    else if (maze[x, y].Walls[(int)Direction.Up].Open)
                    {
                        sb.AppendLine(vertTile.Print(centerX-0.5, centerY, 0.0, tanID));
                        sb.AppendLine(borderRight.Print(centerX + 0.5, centerY, 2.0, tanID));
                    }
                    else if (maze[x, y].Walls[(int)Direction.Right].Open)
                    {
                        sb.AppendLine(horzTile.Print(centerX, centerY-0.5, 0.0, tanID));
                        sb.AppendLine(borderTop.Print(centerX, centerY+0.5, 2.0, tanID));
                    }
                    else
                    {
                        sb.AppendLine(borderTopRight.Print(centerX +0.5, centerY + 0.5, 2.0, tanID));
                        sb.AppendLine(one.Print(centerX - 0.5, centerY - 0.5, 0.0, tanID));
                    }
                }
        }

        public void GenerateSloped(StringBuilder sb)
        {
            MetaData.StartSubModel(sb, $"SlopedMaze_{n}x{n}");
            Shape HorzStraight = new() { PartID = "HorizontalStraight", SubModel=true };
            Shape VertStraight = HorzStraight.Rotate();
            Shape CornerDtoR= new() { PartID = "Corner", SubModel = true };
            Shape CornerRtoU = CornerDtoR.Rotate();
            Shape CornerUtoL = CornerDtoR.Rotate(180);
            Shape CornerLtoD = CornerDtoR.Rotate(-90);
            Shape TshapeDown = new() { PartID = "TShapeDown", SubModel = true };
            Shape TshapeRight = TshapeDown.Rotate();
            Shape TshapeUp = TshapeDown.Rotate(180);
            Shape TshapeLeft = TshapeDown.Rotate(-90);
            Shape Cross = new() { PartID = "Cross", SubModel = true };
            Shape TEndUp = new() { PartID = "TEnd", SubModel = true };
            Shape TEndRight = TEndUp.Rotate(-90);
            Shape TEndDown = TEndUp.Rotate(180);
            Shape TEndLeft = TEndUp.Rotate();


            int redId = ColorMap.Get("Red").id;
            int grayID = ColorMap.Get("Light_Bluish_Grey").id;

            if (n == 15)
            {
                BasePlate bp = new(32);

                for (int x = 0; x < 2; x++)
                    for (int y = 0; y < 2; y++)
                        sb.AppendLine(bp.Print(14+ 32 * x, 14 + 32 * y,-3, grayID));
            }
            else if (n == 11)
            {
                BasePlate bp = new(48);
                sb.AppendLine(bp.Print(22, 22,-3, grayID));
            }

            //corners
            sb.AppendLine(CornerDtoR.Print(0,4*n,0,redId));
            sb.AppendLine(CornerRtoU.Print(0,0, 0, redId));
            sb.AppendLine(CornerUtoL.Print(4*n,0, 0, redId));
            sb.AppendLine(CornerLtoD.Print(4 * n, 4*n, 0, redId));
            //sides
            for (int k = 1; k < n; k++)
            {
                if (maze[k, 0].Walls[(int)Direction.Left].Open)
                    sb.AppendLine(HorzStraight.Print(4 * k, 0, 0, redId));
                else
                    sb.AppendLine(TshapeUp.Print(4*k,0,0,redId));

                if (maze[k, n-1].Walls[(int)Direction.Left].Open)
                    sb.AppendLine(HorzStraight.Print(4 * k, 4*n, 0, redId));
                else
                    sb.AppendLine(TshapeDown.Print(4 * k, 4*n, 0, redId));

                if (maze[0, k].Walls[(int)Direction.Down].Open)
                    sb.AppendLine(VertStraight.Print(0,4 * k, 0, redId));
                else
                    sb.AppendLine(TshapeRight.Print(0,4 * k, 0, redId));

                if (maze[n-1, k].Walls[(int)Direction.Down].Open)
                    sb.AppendLine(VertStraight.Print(4 * n, 4 * k, 0, redId));
                else
                    sb.AppendLine(TshapeLeft.Print(4 * n, 4 * k, 0, redId));
            }
            //mids
            for (int x = 1; x < n; x++)
            {
                for (int y = 1; y < n; y++)
                {
                    Cell ur = maze[x, y];
                    Cell dl = maze[x - 1, y - 1];
                    HashSet<Direction> links = new HashSet<Direction>();
                    if (ur.Walls[(int)Direction.Left].Open)
                        links.Add(Direction.Up);
                    if (ur.Walls[(int)Direction.Down].Open)
                        links.Add(Direction.Right);
                    if (dl.Walls[(int)Direction.Right].Open)
                        links.Add(Direction.Down);
                    if (dl.Walls[(int)Direction.Up].Open)
                        links.Add(Direction.Left);

                    Shape? s = Cross;
                    if (links.Count == 1) // t-shape
                    {
                        s = links.First() switch
                        {
                            Direction.Left => TshapeRight,
                            Direction.Right => TshapeLeft,
                            Direction.Down => TshapeUp,
                            _ => TshapeDown
                        };
                    }
                    else if (links.Count == 3) //end piece
                    {
                        if (!links.Contains(Direction.Left)) s = TEndLeft;
                        else if (!links.Contains(Direction.Right)) s = TEndRight;
                        else if (!links.Contains(Direction.Up)) s = TEndUp;
                        else s = TEndDown;
                    }
                    else if (links.Count == 2) //straight or corner
                    {
                        if (!links.Contains(Direction.Left) && !links.Contains(Direction.Right)) s = HorzStraight;
                        else if (!links.Contains(Direction.Left) && !links.Contains(Direction.Up)) s = CornerUtoL;
                        else if (!links.Contains(Direction.Left) && !links.Contains(Direction.Down)) s = CornerLtoD;
                        else if (!links.Contains(Direction.Up) && !links.Contains(Direction.Down)) s = VertStraight;
                        else if (!links.Contains(Direction.Right) && !links.Contains(Direction.Up)) s = CornerRtoU;
                        else if (!links.Contains(Direction.Right) && !links.Contains(Direction.Down)) s = CornerDtoR;
                    }
                    if (s!= null) sb.AppendLine(s.Print(4 * x, 4 * y, 0, redId));
                }
            }
            //define subshapes...

            Shape Slope2x2 = new() { PartID = "3039" };
            Shape Slope2x4 = new() { PartID = "3037" };
            Shape Slope2x2Convex = new() { PartID = "3046" };
            Shape Slope2x2Concave = new() { PartID = "3045" };
            Shape TopSlope2x2 = new() { PartID = "3043" };
            Shape TopSlope2x4 = new() { PartID = "3041" };
            Shape TopSlopeOverLap = new() { PartID = "3049c" };
            Shape TopSlopeHalfPyramid = new() { PartID = "15571" };
            MetaData.StartSubModel(sb, "HorizontalStraight");
            sb.AppendLine(Slope2x4.Print(0, -0.5, 0, redId));
            sb.AppendLine(Slope2x4.Rotate(180).Print(0, 0.5, 0, redId));
            sb.AppendLine(TopSlope2x4.Print(0,0,3, redId));
            MetaData.StartSubModel(sb, "Corner");
            sb.AppendLine(Slope2x2Convex.Print(0.5, -0.5, 0, redId));
            sb.AppendLine(Slope2x2.Rotate(-90).Print(-0.5, -1, 0, redId));
            sb.AppendLine(Slope2x2.Rotate(180).Print(1, 0.5, 0, redId));
            sb.AppendLine(Slope2x2Concave.Rotate(180).Print(-0.5, 0.5, 0, redId));
            sb.AppendLine(TopSlope2x2.Print(1, 0, 3, redId));
            sb.AppendLine(TopSlopeOverLap.Print(0, -1.5, 3, redId));
            sb.AppendLine(TopSlopeHalfPyramid.Rotate(-90).Print(-0.5, 0, 3, redId));
            MetaData.StartSubModel(sb, "TShapeDown");
            sb.AppendLine(Slope2x2Convex.Rotate(-90).Print(-0.5, -0.5, 0, redId));
            sb.AppendLine(Slope2x2Convex.Print(0.5, -0.5, 0, redId));
            sb.AppendLine(Slope2x4.Rotate(180).Print(0, 0.5, 0, redId));
            sb.AppendLine(TopSlope2x4.Print(0, 0, 3, redId));
            sb.AppendLine(TopSlopeOverLap.Print(0, -1.5, 3, redId));
            MetaData.StartSubModel(sb, "Cross");
            sb.AppendLine(Slope2x2Convex.Rotate(180).Print(-0.5, 0.5, 0, redId));
            sb.AppendLine(Slope2x2Convex.Rotate(90).Print(0.5, 0.5, 0, redId));
            sb.AppendLine(Slope2x2Convex.Rotate(-90).Print(-0.5, -0.5, 0, redId));
            sb.AppendLine(Slope2x2Convex.Print(0.5, -0.5, 0, redId));
            sb.AppendLine(TopSlope2x4.Print(0, 0, 3, redId));
            sb.AppendLine(TopSlopeOverLap.Print(0, -1.5, 3, redId));
            sb.AppendLine(TopSlopeOverLap.Rotate(180).Print(0, 1.5, 3, redId));
            MetaData.StartSubModel(sb, "TEnd");
            sb.AppendLine(Slope2x2.Rotate(-90).Print(-0.5, 1, 0, redId));
            sb.AppendLine(Slope2x2.Rotate(90).Print(0.5, 1, 0, redId));
            sb.AppendLine(Slope2x2Concave.Rotate(-90).Print(-0.5, -0.5, 0, redId));
            sb.AppendLine(Slope2x2Concave.Print(0.5, -0.5, 0, redId));
            sb.AppendLine(TopSlope2x2.Rotate(90).Print(0,1, 3, redId));
            sb.AppendLine(TopSlopeHalfPyramid.Print(0, -0.5, 3, redId));
        }

    }
}
