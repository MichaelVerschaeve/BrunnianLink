using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    public class BoerdijkCoxeter
    {
        public static void Generate(StringBuilder sb, int level)
        {
            MetaData.StartSubModel(sb, $"Tetrahelix_{level}");
            string dir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.FullName!;
            dir = Directory.GetParent(dir)?.FullName!;
            dir = Directory.GetParent(dir)?.FullName!;
            dir = Directory.GetParent(dir)?.FullName!;
            string file = Path.Combine(dir, "TetraHedronTiled.ldr");
            string[] fileLines = File.ReadAllLines(file);
            var partsGrouped = fileLines.Where(s => s.EndsWith("15535.dat")).Select(l => l.Split(' ')).GroupBy(parts => parts[1]);
            var fx = partsGrouped.Select(g => g.Average(parts => double.Parse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture)));
            var fy = partsGrouped.Select(g => g.Average(parts => double.Parse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture)));
            var fz = partsGrouped.Select(g => g.Average(parts => double.Parse(parts[4], NumberStyles.Float, CultureInfo.InvariantCulture)));
            double sumpx = fx.Sum();
            double sumpy = fy.Sum();
            double sumpz = fz.Sum();
            double[] px = fx.Select(fi => sumpx - 3.0 * fi).ToArray();
            double[] py = fy.Select(fi => sumpy - 3.0 * fi).ToArray();
            double[] pz = fz.Select(fi => sumpz - 3.0 * fi).ToArray();
            double dx = (px[1] - px[0]);
            double dy = (py[1] - py[0]);
            double dz = (pz[1] - pz[0]);
            double scale = Math.Sqrt(dx * dx + dy * dy + dz * dz);

            /*for (int i = 0; i < px.Length; i++)
                for (int j = i + 1; j < px.Length; j++)
                {
                     dx = (px[j] - px[i]);
                     dy = (py[j] - py[i]);
                     dz = (pz[j] - pz[i]);
                     double s = Math.Sqrt(dx * dx + dy * dy + dz * dz);
                    System.Diagnostics.Debug.WriteLine($"{i}-{j} {s}");
                }
            System.Diagnostics.Debug.WriteLine(scale);
            */
            double[][] PInv = MatInverse(new double[][]{
                new double[] { px[1]- px[0], px[2] - px[0], px[3] - px[0] },
                new double[] { py[1]- py[0], py[2] - py[0], py[3] - py[0] },
                new double[] { pz[1]- pz[0], pz[2] - pz[0], pz[3] - pz[0] } }
            );
            foreach ((double[] pxt, double[] pyt, double[] pzt) in SpiralPoints(level, scale))
            {
                double[][] M = MatMatMul(new double[][] {
                    new double[] { pxt[1] - pxt[0], pxt[2] - pxt[0], pxt[3] - pxt[0] },
                    new double[] { pyt[1] - pyt[0], pyt[2] - pyt[0], pyt[3] - pyt[0] },
                    new double[] { pzt[1] - pzt[0], pzt[2] - pzt[0], pzt[3] - pzt[0] }
                }, PInv);
                double[] t = MathVecMul(M, new double[] { px[0], py[0], pz[0] });
                double[] C = new double[] { pxt[0] - t[0], pyt[0] - t[1], pzt[0] - t[2] };
                string pos = String.Join(" ", C.Select(d => d.ToString(CultureInfo.InvariantCulture)));
                string mat = String.Join(" ", M.Select(row => String.Join(" ", row.Select(d => d.ToString(CultureInfo.InvariantCulture)))));
                sb.AppendLine($"1 16 {pos} {mat} tetrahedron");
            }
            MetaData.StartSubModel(sb, "tetrahedron");
            foreach (string line in fileLines.Where(line => line.StartsWith("1")))
                sb.AppendLine(line);
        }


        public static IEnumerable<(double[] pxt, double[] pyt, double[] pzt)> SpiralPoints(int n, double scale)
        {
            n += 3;
            double r = scale * 3 * Math.Sqrt(3.0) / 10.0;
            double h = scale / Math.Sqrt(10.0);
            List<double> px = new List<double>();
            List<double> py = new List<double>();
            List<double> pz = new List<double>();
            double alpha = Math.Acos(-2.0 / 3.0);
            for (int i = 0; i < n; i++)
            {
                px.Add(r * Math.Cos(i * alpha));
                py.Add(r * Math.Sin(i * alpha));
                pz.Add(i * h);
                if (px.Count >= 4)
                {
                    yield return (px.ToArray(), py.ToArray(), pz.ToArray());
                    px.RemoveAt(0);
                    py.RemoveAt(0);
                    pz.RemoveAt(0);
                }
            }
        }

        public static double[][] MatInverse(double[][] mat)
        {
            double det = mat[0][0] * (mat[1][1] * mat[2][2] - mat[2][1] * mat[1][2]) -
              mat[0][1] * (mat[1][0] * mat[2][2] - mat[1][2] * mat[2][0]) +
              mat[0][2] * (mat[1][0] * mat[2][1] - mat[1][1] * mat[2][0]);

            double invdet = 1 / det;

            return new double[][]
            {
                new double[]
                {
                    (mat[1][1] * mat[2][2] - mat[2][1] * mat[1][2]) * invdet,
                    (mat[0][2] * mat[2][1] - mat[0][1] * mat[2][2]) * invdet,
                    (mat[0][1] * mat[1][2] - mat[0][2] * mat[1][1]) * invdet
                },
                new double[]
                {
                    (mat[1][2] * mat[2][0] - mat[1][0] * mat[2][2]) * invdet,
                    (mat[0][0] * mat[2][2] - mat[0][2] * mat[2][0]) * invdet,
                    (mat[1][0] * mat[0][2] - mat[0][0] * mat[1][2]) * invdet
                },
                new double[] {
                    (mat[1][0] * mat[2][1] - mat[2][0] * mat[1][1]) * invdet,
                    (mat[2][0] * mat[0][1] - mat[0][0] * mat[2][1]) * invdet,
                    (mat[0][0] * mat[1][1] - mat[1][0] * mat[0][1]) * invdet
                }
            };
        }

        public static double[][] MatMatMul(double[][] matA, double[][] matB)
        {
            double[][] res = new double[][] { new double[] { 0.0, 0.0, 0.0 }, new double[] { 0.0, 0.0, 0.0 }, new double[] { 0.0, 0.0, 0.0 } };
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    for (int k = 0; k < 3; k++)
                        res[i][j] += matA[i][k] * matB[k][j];
            return res;
        }

        private static double[] MathVecMul(double[][] mat, double[] v)
        {
            double[] res = new double[] { 0.0, 0.0, 0.0 };
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    res[i] += mat[i][j] * v[j];
            return res;
        }

    }
}