using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BrunnianLink
{

    static public class Hilbert3D
    {
        static bool ShapeCondition(List<int[]> source)
        {
            int dxm2 = source[~1][0] - source[~2][0];
            int dym2 = source[~1][1] - source[~2][1];
            int dzm2 = source[~1][2] - source[~2][2];
            int dxm1 = source[0][0] - source[~1][0];
            int dym1 = source[0][1] - source[~1][1];
            int dzm1 = source[0][2] - source[~1][2];
            int dx0 = source[1][0]- source[0][0];
            int dy0 = source[1][1] - source[0][1];
            int dz0 = source[1][2] - source[0][2];
            int dx1 = source[2][0] - source[1][0];
            int dy1 = source[2][1] - source[1][1];
            int dz1 = source[2][2] - source[1][2];

            //last segment perpendicular to first
            if (dxm1 * dx0 + dym1 * dy0 + dzm1 * dz0 != 0)
                return false;
            //2nd segment perpendicular to first
            if (dx1 * dx0 + dy1 * dy0 + dz1 * dz0 != 0)
                return false;
            //2nd segment perpendicual to last
            if (dx1 * dxm1 + dy1 * dym1 + dz1 * dzm1 != 0)
                return false;
            //second to last segment = - first segment
            if (dxm2 != dx0 || dym2 != dy0 || dzm2 != dz0)
                return false;
            return true;
        }

        static int[] MatMul(int[][] columnOrderFirst, int[] v)
        {
            int[] res = [0, 0, 0];
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    res[i] += columnOrderFirst[j][i] * v[j];
            return res;
        }

        static List<int[]> Transform(List<int[]> source, int[] targetOrigin, int[] targetX, int[] targetY, int[] targetZ)
        {
            if (!ShapeCondition(source))
            {
                source = new(source);
                source.Reverse();
            }
            if (!ShapeCondition(source))
                throw new NotImplementedException(); //not expected
            int[][] transpose = 
        }


        static List<int[]> PointCollection(int level)
        {
            if (level == 1) return [[0, 0, 0], [1, 0, 0], [1, 1, 0], [0, 1, 0], [0, 1, 1], [1, 1, 1], [1, 0, 1], [0, 0, 1]];

            List<int[]> points = PointCollection(level-1);

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
            bool? prevTube = null;
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
                    prevTube = currentTube;
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
