using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    public class LavesGraphGenerator
    {
        static readonly int blueId = ColorMap.Get("Dark_Azure").id; //only black has currently all parts...
        static readonly int transBlueId = 41;
        static readonly int yellowId = ColorMap.Get("Yellow").id; //only black has currently all parts...
        static readonly int transYellowId = 46;
        public static void Generate(StringBuilder sb, int level)
        {
            MetaData.StartSubModel(sb, $"Laves{level}");
            List<(int x, int y, int z)> basePoints = new()
            { (0,0,0),(1,2,3),(2,3,1),(3,1,2),
              (2,2,2),(3,0,1),(0,1,3),(1,3,0)
            };
            GenerateLaves(sb, level, basePoints, true);
            basePoints = new(basePoints.Select(t=>(3-t.x,3-t.y,3-t.z)));
            GenerateLaves(sb, level, basePoints, false);
        }

        private static void GenerateLaves(StringBuilder sb, int level, List<(int x, int y, int z)> basePoints, bool color1)
        {
            List<(int x, int y, int z)> nodes = new(basePoints.SelectMany(basePoint => Enumerable.Range(0, level).Select(i => (basePoint.x + 4 * i, basePoint.y, basePoint.z))));
            nodes = new(nodes.SelectMany(basePoint => Enumerable.Range(0, level).Select(i => (basePoint.x, basePoint.y + 4 * i, basePoint.z))));
            nodes = new(nodes.SelectMany(basePoint => Enumerable.Range(0, level).Select(i => (basePoint.x, basePoint.y, basePoint.z + 4 * i))));

            var DistanceSquared = ((int x, int y, int z) p1, (int x, int y, int z) p2) =>
            {
                int dx = p1.x - p2.x;
                int dy = p1.y - p2.y;
                int dz = p1.z - p2.z;
                return dx * dx + dy * dy + dz * dz;
            };
            HashSet<((int x, int y, int z), (int x, int y, int z))> linksDone = new();
            //bool first = true;
            foreach (var node in nodes)
            {
                var neighbours = nodes.Where(node2 => DistanceSquared(node, node2) == 2);
                if (neighbours.Count() != 3) continue;
                var vnow = PrintNode(sb, node, neighbours, color1);
                foreach (var neighbor in neighbours)
                {
                    if (!linksDone.Contains((neighbor, node)))
                    {
                        PrintLink(sb, node, neighbor, color1);
                        linksDone.Add((node, neighbor));
                    }
                    //else if (first)
                    //{
                    //    double angle = 180*Math.Acos(ScalarProduct(vnow, PrintNode(null, neighbor, nodes.Where(node3 => DistanceSquared(neighbor, node3) == 2), false)))/double.Pi;
                    //    first = false;
                    //}
                }
            }
        }

        private static (double x, double y, double z) PrintNode(StringBuilder? sb, (int x, int y, int z) node, IEnumerable<(int x, int y, int z)> neighbours, bool bColor1)
        {
            double SF1 = 0.5 * Math.Sqrt(2.0);
            double SF2 = 5.0 * SF1;
            (double x, double y, double z) c = VectorScale(SF2,node);
            List<(double x, double y, double z)> star = neighbours.Select(p=>VectorDiff(p,node)).ToList();
            (double x, double y, double z) vz = VectorNormalize(star[0]);
            (double x, double y, double z) vx = VectorNormalize(VectorDiff(star[1], star[2]));
            (double x, double y, double z) vy = VectorProduct(vz, vx);
            c = VectorSum(c, VectorScale(0.5,vy));
           // if (sb!= null) PrintPart(sb, c, vx, vy, vz, bColor1 ? redId : blueId, "10288.dat");
            if (sb!= null) PrintPart(sb, c, vx, vy, vz, bColor1 ? transBlueId : transYellowId, "42409.dat");
            return vy;
        }

        private static void PrintLink(StringBuilder sb, (int x, int y, int z) p1, (int x, int y, int z) p2, bool bColor1)
        {
            double SF1 = 0.5 * Math.Sqrt(2.0);
            double SF2 = 5.0 * SF1 ;
            (double x, double y, double z) c = VectorScale(0.5 * SF2, VectorSum(p1, p2));
            (double x, double y, double z) vx = VectorScale(SF1, VectorDiff(p2, p1));
            (double x, double y, double z) vy = (0,1,0);
            double p = ScalarProduct(vx, vy);
            vy = VectorNormalize(VectorDiff(vy, VectorScale(p, vx)));
            (double x, double y, double z) vz = VectorProduct(vx, vy); 
            //PrintPart(sb, c, vx, vy, vz, bColor1?orangeId:yellowId, "62462.dat");
            PrintPart(sb, c, vz, vy, vx, bColor1?blueId:yellowId, "59443.dat");
        }

        private static void PrintPart(StringBuilder sb, (double x, double y, double z) c, (double x, double y, double z) vx, (double x, double y, double z) vy, (double x, double y, double z) vz, int colorId, string part)
        {
            c.x *= 20;
            c.y *= 20;
            c.z *= 20;
            string mat = "";
            string pos = c.x.ToString(CultureInfo.InvariantCulture) + " " + c.y.ToString(CultureInfo.InvariantCulture) + " " + c.z.ToString(CultureInfo.InvariantCulture);
            mat += vx.x.ToString(CultureInfo.InvariantCulture) + " " + vy.x.ToString(CultureInfo.InvariantCulture) + " " + vz.x.ToString(CultureInfo.InvariantCulture) + " ";
            mat += vx.y.ToString(CultureInfo.InvariantCulture) + " " + vy.y.ToString(CultureInfo.InvariantCulture) + " " + vz.y.ToString(CultureInfo.InvariantCulture) + " ";
            mat += vx.z.ToString(CultureInfo.InvariantCulture) + " " + vy.z.ToString(CultureInfo.InvariantCulture) + " " + vz.z.ToString(CultureInfo.InvariantCulture);
            sb.AppendLine($"1 {colorId} {pos} {mat} {part}");
        }

        private static (double x, double y, double z) VectorProduct((double x, double y, double z) v1, (double x, double y, double z) v2)
        {
            return(v1.y*v2.z-v1.z*v2.y,v1.z*v2.x-v1.x*v2.z,v1.x*v2.y-v1.y*v2.x);
        }

        private static (double x, double y, double z) VectorNormalize((double x, double y, double z) v)
        {
            return VectorScale(1.0 / Math.Sqrt(ScalarProduct(v, v)), v);
        }

        private static (double x, double y, double z) VectorDiff((double x, double y, double z) v1, (double x, double y, double z) v2)
        {
            return (v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }

        private static (double x, double y, double z) VectorSum((double x, double y, double z) v1, (double x, double y, double z) v2)
        {
            return (v2.x + v1.x, v2.y + v1.y, v2.z + v1.z);
        }

        private static (double x, double y, double z) VectorScale(double s, (double x, double y, double z) v)
        {
            return (s * v.x, s * v.y, s * v.z);
        }

        private static double ScalarProduct((double x, double y, double z) v1, (double x, double y, double z) v2)
        {
            return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
        }
    }
}
