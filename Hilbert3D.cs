using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BrunnianLink.LabyrinthTiling;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BrunnianLink
{
    public class PointComparer : IComparer<int[]>
    {
        public int Compare(int[]? x, int[]? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x == null) return 1;
            if (y == null) return -1;
            foreach ((int c1,int c2) in x.Zip(y))
            {
                int t= c1.CompareTo(c2);
                if (t != 0) return t;
            }
            return 0;   
        }
    }

    static public class Hilbert3D
    {
        static IEnumerable<int> FormVector(List<int[]> source, int i)
        {
            for (int j = 0; j < 3; j++)
                yield return source[(source.Count + i + 1) % source.Count][j] - source[(source.Count + i) % source.Count][j];
        }

        static bool ShapeCondition(List<int[]> source)
        {
            var ortho = (int i, int j) => FormVector(source,i).Zip(FormVector(source, j)).Select(p=>p.First*p.Second).Sum()==0;
            //last segment and first two pairwise perpendicular
            if (!ortho(-1,0) || !ortho(0,1) || !ortho(-1,1))
                return false;
            //second to last segment = - first segment
            return FormVector(source,-2).SequenceEqual(FormVector(source,0).Select(t => -t));
        }

        static int[] MatMul(int[][] columnOrderFirst, int[] v)
        {
            int[] res = [0, 0, 0];
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    res[i] += columnOrderFirst[j][i] * v[j];
            return res;
        }

        static int[][] Transpose(int[][] columnOrderFirst) => Enumerable.Range(0, 3).Select(r => Enumerable.Range(0, 3).Select(c => columnOrderFirst[c][r]).ToArray()).ToArray();

        static List<int[]> Transform(List<int[]> source, int[] targetOrigin, int[] targetX, int[] targetY, int[] targetZ)
        {
            if (!ShapeCondition(source))
            {
                source = new(source);
                source.Reverse();
                if (!ShapeCondition(source))
                    throw new NotImplementedException(); //not expected
            }
            int[][] transpose = Transpose([FormVector(source,0).ToArray(), FormVector(source, 1).ToArray(), FormVector(source, -1).Select(t=>-t).ToArray()]);
            int[][] targetMat = [targetX,targetY, targetZ];
            return source.Select(point => MatMul(targetMat, MatMul(transpose, point.Select((v, index) => v - source[0][index]).ToArray())).Select((v, index) => v  + targetOrigin[index]).ToArray()).ToList();
        }


        static List<int[]> PointCollection(int level)
        {
            if (level == 1) return [[0, 0, 0], [1, 0, 0], [1, 1, 0], [0, 1, 0], [0, 1, 1], [1, 1, 1], [1, 0, 1], [0, 0, 1]];
            int[] X = [1, 0, 0], Y = [0, 1, 0], Z = [0, 0, 1], minX = [-1, 0, 0], minY = [0, -1, 0], minZ = [0, 0, -1];
            List<int[]> points = PointCollection(level - 1);
            List<int[]> t1 = Transform(points, [0, 0, 1], minY, Z, minX);
            List<int[]> t2 = Transform(points, [0, 0, 0], minY, minZ, minX); t2.Reverse();
            List<int[]> t3 = Transform(points, [1, 0, 0], minZ, minY, X);
            List<int[]> t4 = Transform(points, [1, 1, 0], minZ, Y, X); t4.Reverse();
            List<int[]> t5 = Transform(points, [0, 1, 0], Y, minZ, minX);
            List<int[]> t6 = Transform(points, [0, 1, 1], Y, Z, minX); t6.Reverse();
            List<int[]> t7 = Transform(points, [1, 1, 1], Z, Y, X);
            List<int[]> t8 = Transform(points, [1, 0, 1], Z, minY, X); t8.Reverse();
            List<int[]> result = new();
            List<int[]>[] subs = [t1, t2, t3, t4, t5, t6, t7, t8];
            foreach (var range in subs)
                result.AddRange(range);
            //return result;
            
            int[]? minPoint = result.Min(new PointComparer());
            int index = minPoint == null ? 0 : result.IndexOf(minPoint);
            result = result.Skip(index).Concat(result.Take(index)).ToList();
            return Transform(result, [0, 0, 0], X, Y, Z);
        }

        static public void GenerateCyclic(StringBuilder sb, int level)
        {
            level = Math.Max(level, 1);
            MetaData.StartSubModel(sb, $"Hilbert3D_{level}");
            var points = PointCollection(level);
            var connects = points.Zip(points.Skip(1).Append(points[0])).Select(t => new double[] { 2.0*(t.First[0] + t.Second[0]), 2.0*(t.First[1] + t.Second[1]), 5*(t.First[2] + t.Second[2]) } ).ToList();
            var all = connects.Concat(points.Select<int[],double[]>(a => [4.0 * a[0], 4.0 * a[1],10.0*a[2]]));

            int colorId = ColorMap.Get("Red").id; //trans clear//
            Brick brick = new (2, 2);
            Tile squareTile = new(2, 2);
            Shape squareInvertedTile = new() { PartID = "11203" };
            foreach ( var point in all)
            {
                sb.AppendLine(squareInvertedTile.Print(point[0], point[1], point[2], colorId));
                sb.AppendLine(brick.Print(point[0], point[1], point[2]+3, colorId));
                sb.AppendLine(squareTile.Print(point[0], point[1], point[2] + 4, colorId));
            }
        }


        static public void Generate(StringBuilder sb, int level)
        {
            level = Math.Max(level, 1);
            MetaData.StartSubModel(sb, $"Hilbert3D_{level}");

            string commands = "X";
            for (int i = 0; i < level; i++)
                commands = commands.Replace("X", "^<XF^<XFX-F^>>XFX&F+>>XFX-F>X->");

            List<int[]> points = new() { new int[] { 0, 0, 0 } };
            bool? currentTube = null;
            List<Rotation> rotations = new() { new Rotation() };


            Rotation rotation = new();

            foreach (char command in commands)
            {
                switch (command)
                {
                    case 'F':
                        points.Add(rotation.Forward(points.Last()));
                        System.Diagnostics.Debug.WriteLine(string.Join(' ',points.Last()));
                        rotations.Add((rotation.Clone() as Rotation)!);
                        break;
                    case '+':
                        rotation.Yaw(false); break;
                    case '-':
                        rotation.Yaw(true); break;
                    case '^':
                        rotation.Pitch(true); break;
                    case '&':
                        rotation.Pitch(false); break;
                    case '<':
                        rotation.Roll(true); break;
                    case '>':
                        rotation.Roll(false); break;

                }
                if (points.Count == 3)
                {
                    bool? prevTube = currentTube;
                    currentTube = AddCilindersAndElbows(sb, points, rotations);
                   /* if (prevTube.HasValue)
                    {
                        //int color = ColorMap.Get("Blue").id;
                        string partID = "43093.dat"; //pin-axle
                        if (prevTube.Value == currentTube.Value)
                        {
                            sb.Append($"1 {(prevTube.Value ? ColorMap.Get("Red").id : ColorMap.Get("Black").id)} ");
                            partID = prevTube.Value ? "32062.dat" : "2780.dat";
                        }
                        else
                            sb.Append($"1 {ColorMap.Get("Blue").id} ");
                        points[0] = points[0].Zip(points[1], (x, y) => x + y).ToArray();
                        sb.Append($" {points[0][0] * 15} {-points[0][1] * 10} {points[0][2] * 10} ");
                        sb.Append(((prevTube.Value && !currentTube.Value) ? rotations[1]?.PreRotate() :rotations[1])?.ToString());
                        sb.Append(' ');
                        sb.AppendLine(partID);
                    }*/
                    points.RemoveAt(0);
                    rotations.RemoveAt(0);
                }
            }
        }

        static bool[,,] holes = new bool[0,0,0];

        static public void GenerateTrans(StringBuilder sb, int level)
        {
            level = Math.Max(level, 1);
            MetaData.StartSubModel(sb, $"Hilbert3D_hollow_{level}");
            int size = (1 << (level + 1)) + 1;
            holes = new bool[size, size, size + 1];
            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                    holes[x, y, size] = true;
            var pointCollection = PointCollection(level);
            int[] prevPoint = pointCollection[pointCollection.Count-1];
            foreach (var point in pointCollection)
            { 
                holes[point[0] * 2 + 1, point[1] * 2 + 1, point[2] * 2 + 1] = true;
                holes[point[0] + prevPoint[0] + 1, point[1] + prevPoint[1] + 1, point[2] + prevPoint[2] + 1] = true;
                prevPoint = point;
            }
            holes[1, 0, 1] = true; //the peeper;
            bool regular = true;
            int colorId = ColorMap.Get("Bright_Pink").id; //trans clear//
            Shape[] square2x2Shapes = { new Plate(2, 2), new Brick(2, 2), new Plate(2, 2) };
            Tile SquareTile = new(2, 2);
            Shape SquareInvertedTile = new() { PartID = "11203" };
            Shape[] square1x1Shapes = { new Plate(1, 1), new Brick(1, 1), new Plate(1, 1) };
            Shape[] rect1x2Shapes = { new Plate(1, 2), new Brick(1, 2), new Plate(1, 2) };
            Shape[] rect2x1Shapes = { new Plate(2, 1), new Brick(2, 1), new Plate(2, 1) };
            for (int i = 0; i < size; i++)
            {
                int[] z = { 5 * i + 1, 5 * i + 4, 5 * i + 5 };
                for (int j = 0; j < 3; j++)
                {
                    if (regular)
                    {
                        for (int x = 0; x < size; x++)
                            for (int y = 0; y < size; y++)
                                if (!holes[x, y, i])
                                {
                                    Shape s = square2x2Shapes[j];
                                    if (j == 2 && (i == size-1 || holes[x, y, i + 1]))
                                        s = SquareTile;
                                    if (j == 0 && (i == 0 || holes[x, y, i - 1]))
                                        s = SquareInvertedTile;
                                    sb.AppendLine(s.Print(2 * x + 1, 2 * y + 1, z[j], colorId));
                                }
                    }
                    else //offsetted layer
                    {
                        for (int x = 0; x <= size; x++)
                            for (int y = 0; y <= size; y++)
                            {
                                bool holeBottomLeft = x == 0 || y == 0 || holes[x - 1, y - 1, i];
                                bool holeTopLeft = x == 0 || y == size || holes[x - 1, y, i];
                                bool holeBottomRight = x == size || y == 0 || holes[x, y - 1, i];
                                bool holeTopRight = x == size || y == size || holes[x, y, i];
                                if (!holeBottomLeft && !holeTopLeft && !holeBottomRight && !holeTopRight)
                                    sb.AppendLine(square2x2Shapes[j].Print(2 * x, 2 * y, z[j], colorId));
                                else if (!holeBottomLeft && !holeTopLeft && holeBottomRight && holeTopRight)
                                    sb.AppendLine(rect1x2Shapes[j].Print(2 * x - 0.5, 2 * y, z[j], colorId));
                                else if (holeBottomLeft && holeTopLeft && !holeBottomRight && !holeTopRight)
                                    sb.AppendLine(rect1x2Shapes[j].Print(2 * x + 0.5, 2 * y, z[j], colorId));
                                else if (!holeBottomLeft && holeTopLeft && !holeBottomRight && holeTopRight)
                                    sb.AppendLine(rect2x1Shapes[j].Print(2 * x, 2 * y - 0.5, z[j], colorId));
                                else if (holeBottomLeft && !holeTopLeft && holeBottomRight && !holeTopRight)
                                    sb.AppendLine(rect2x1Shapes[j].Print(2 * x, 2 * y + 0.5, z[j], colorId));
                                else
                                {
                                    Shape s = square1x1Shapes[j];
                                    double[] xoffset = { -0.5, -0.5, 0.5, 0.5 };
                                    double[] yoffset = { -0.5, 0.5, -0.5, 0.5 };
                                    bool[] skipcondition = { holeBottomLeft, holeTopLeft, holeBottomRight, holeTopRight };
                                    for (int k = 0; k < 4; k++)
                                        if (!skipcondition[k])
                                            sb.AppendLine(s.Print(2 * x + xoffset[k], 2 * y + yoffset[k], z[j], colorId));
                                }
                            }
                    }
                    regular = !regular;
                    MetaData.BuildStepFinished(sb);
                }
            }
        }

        static bool AddCilindersAndElbows(StringBuilder sb, List<int[]> points, List<Rotation> rotations)
        {
            int[] vX_In = rotations[1].X();
            int[] vX_Out = rotations[2].X();

            if (vX_In.SequenceEqual(vX_Out))
            {
                sb.Append($"1 {ColorMap.Get("Metallic_Silver").id} ");
                for (int i = 0; i < 3; i++)
                {
                    sb.Append($"{10 * points[0][i] + 50 * points[1][i]} ");
                }
                sb.Append(rotations[1].Transpose().ToString());
                sb.AppendLine(" 62462.dat");

                sb.Append($"1 {ColorMap.Get("Metallic_Silver").id} ");
                for (int i = 0; i < 3; i++)
                {
                     sb.Append($"{40 * points[1][i] + 20 * points[2][i]} ");
                 }
                 rotations[1].Yaw(true);
                 sb.Append(rotations[1].Transpose().ToString());
                 sb.AppendLine(" 18654.dat");
                 
                return false;
            }

            sb.Append($"1 {ColorMap.Get("Metallic_Silver").id} ");
            for (int i = 0; i < 3; i++)
            {
                sb.Append($"{60 * points[1][i]} ");
            }
            sb.Append( new Rotation(new int[][] { vX_In, CrossProduct(vX_Out, vX_In), vX_Out }));
            sb.AppendLine(" 25214.dat"); //elbow
            return true;
        }

        private static int[] CrossProduct(int[] a, int[] b)
        {
            return new int[] {

                a[1] * b[2] - a[2] * b[1],
                a[2] * b[0] - a[0] * b[2],
                a[0] * b[1] - a[1] * b[0]
            };

        }
    }

    public class Rotation : ICloneable
    {


        int[] mat = new int[] { 1, 0, 0, 0, 1, 0, 0, 0, 1 }; //row storage

        public Rotation(int[][] columns)
        {
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++) 
                    mat[3 * r + c] = columns[c][r];
        }

        public Rotation()
        {
        }

        public void Yaw(bool positive)
        {
            int s = positive ? 1 : -1;
            for (int i = 0; i < 3; i++)
                (mat[i], mat[i + 3]) = (-s * mat[i + 3], s * mat[i]);

        }

        public void Pitch(bool positive)
        {
            int s = positive ? 1 : -1;
            for (int i = 0; i < 3; i++)
                (mat[i], mat[i + 6]) = (s * mat[i + 6], -s * mat[i]);

        }

        public void Roll(bool positive)
        {
            int s = positive ? 1 : -1;
            for (int i = 0; i < 3; i++)
                (mat[i+3], mat[i + 6]) = (-s * mat[i + 6], s * mat[i+3]);
        }



        public int[] Forward(int[] pos) 
        { 
            int[] res = new int[3];
            for (int i = 0; i < 3; i++)
                res[i] = pos[i] + mat[i];
            return res;
        }

        public override string ToString()
        {
            return string.Join(" ", mat);
        }

        public object Clone()
        {
            return new Rotation() { mat = (mat.Clone() as int[])! };
        }

        public Rotation Transpose()
        {
            Rotation res = new(){ mat = (mat.Clone() as int[])! };
            for (int i = 0; i < 3; i++ )
            {
                for (int r = 0; r < 3; r++)
                    for (int c = 0; c < 3; c++)
                        res.mat[3 * r + c] = mat[3*c+r];
            }
            return res;
        }


        public int[] X()
        {
            return mat.Take(3).ToArray();
        }
    }
}
