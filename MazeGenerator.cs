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

        public class Cell
        {
            public Wall[] Walls = new Wall[4];
            public bool AssignedToMaze = false;
            public bool Visited = false;

            internal Direction FindCell(Cell newCell)
            {
                for (int i = 0; i < 4; i++)
                    if (Walls[i].OtherSide(this) == newCell)
                        return (Direction)(i);
                return 0;
            }
        }

        public class Wall
        {
            public bool Open = false;
            Cell[] Sides;

            public Wall(Cell c1, Cell c2)
            {
                Sides = new Cell[2] { c1, c2 };
            }

            public Cell OtherSide(Cell arg) => arg == Sides[0] ? Sides[1] : Sides[0];
        }

        public Cell[,] maze;

        private readonly int n;

        public MazeGenerator(int level)
        {
            n = (1 << (level + 1)) - 2;
            maze = new Cell[n, n];
            List<Cell> unassigned = new List<Cell>();

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; i++)
                {
                    unassigned.Add(maze[i, j] = new Cell());
                }

            //set up walls;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Wall w = new Wall(maze[j, i], maze[j, i + 1]);
                    maze[j, i].Walls[(int)Direction.Up] = w;
                    maze[j, i + 1].Walls[(int)Direction.Down] = w;
                    w = new Wall(maze[i, j], maze[i + 1, j]);
                    maze[i, j].Walls[(int)Direction.Right] = w;
                    maze[i + 1, j].Walls[(int)Direction.Left] = w;
                }
            }

            Random rand = new Random();

            Cell firstPick = unassigned[rand.Next(unassigned.Count)];
            firstPick.AssignedToMaze = true;
            unassigned.Remove(firstPick);

            while (unassigned.Count > 0)
            {
                Cell lastVisited = unassigned[rand.Next(unassigned.Count)];
                List<Cell> path = new List<Cell>() { lastVisited };
                lastVisited.Visited = true;
                Direction lastDirection = (Direction)rand.Next(4);
                while (!lastVisited.AssignedToMaze)
                {
                    Direction newDirection = (Direction)((int)(lastDirection + rand.Next(3)) % 4);
                    if (newDirection == Opposite(lastDirection)) newDirection = lastDirection; //don't go back
                    Cell newCell = lastVisited.Walls[(int)newDirection].OtherSide(lastVisited);
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
                            lastDirection = path[path.Count - 2].FindCell(newCell);
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
            Shape borderTop = new Shape() { PartID = "4806b" };
            Shape borderLeft = borderTop.Rotate();
            Shape borderBottom = borderLeft.Rotate();
            Shape borderRight = borderBottom.Rotate();
            Tile horzTile = new(2);
            Tile vertTile = new(1, 2);
            int tanID = ColorMap.Get("Tan").id;
            //border
            for (int i = 0; i < n; i++)
            {
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

            Shape borderTopRight = new Shape() { PartID = "4806b" }.Rotate(-90);

            Tile two = new(2, 2);

            for (int x = 0; x < n; x++)
                for (int y = 0; y < n; y++)
                {
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
    }
}
