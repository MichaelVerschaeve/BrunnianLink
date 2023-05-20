using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.AxHost;

namespace BrunnianLink
{
    public struct RhombData : IComparable<RhombData>
    {
        public double x;
        public double y;
        public int rotation;
        public bool thin;

        public int CompareTo(RhombData other)
        {
            int res = (rotation,thin).CompareTo((other.rotation,other.thin));
            if (res != 0) return res;
            if (Math.Abs(x-other.x) > 1.0e-8)
                return x.CompareTo(other.x);
            if (Math.Abs(y - other.y) > 1.0e-8)
                return y.CompareTo(other.y);
            return 0; //consider equal;
        }


    }

    public class VertexData : IComparable<VertexData>
    {
        public double x;
        public double y;
        public int flags;

        public int CompareTo(VertexData? other)
        {
            if (other == null) return -1;
            if (Math.Abs(x - other.x) > 1.0e-8)
                return x.CompareTo(other.x);
            if (Math.Abs(y - other.y) > 1.0e-8)
                return y.CompareTo(other.y);
            return 0; //consider equal;
        }
    }

    public static class PenroseRhomb 
    {
        private static readonly string[] m_colors = { "Yellow", "Red" };
        public static string[] Colors => m_colors;

        public static int StateCount => 2;

        static readonly double phi = (1+Math.Sqrt(5.0))*0.5;

        public static double ScaleFactor => phi;

        public static double InitialScale => 16;

        public static string MainName => "PenroseRhomb";

        public static string BasePart(bool thin)
        {
            return thin ? "Thin" : "Thick";
        }

        static readonly Dictionary<int, double> c = new();
        static readonly Dictionary<int, double> s = new();


        public static void Generate(StringBuilder sb, int level)
        {
            MetaData.StartSubModel(sb, MainName);

            SortedSet<RhombData> oldSet = new() { new RhombData() { x= 0.0, y=0.0, rotation=0, thin=false } };
            SortedSet<VertexData> vSet = new();
            for (int i = 0; i < level; i++)
            {
                SortedSet<RhombData> newSet = new();

                foreach (RhombData olddata in oldSet)
                {
                    foreach (RhombData newdata in Recurse(olddata))
                        newSet.Add(newdata);
                }
                oldSet = newSet;
            }
            var scaledSet = oldSet.Select((RhombData d) =>
            {
                RhombData r = d;
                r.x *= InitialScale;
                r.y *= InitialScale;
                return r;
            });

            foreach (RhombData data in scaledSet)
            {
                Shape s = new() { PartID = BasePart(data.thin), SubModel = true };
                sb.AppendLine(s.Rotate(data.rotation).Print(data.x, data.y, 0, ColorMap.Get(Colors[data.thin ? 1 : 0]).id));
                foreach (VertexData vdata in VerticesFromRhomb(data))
                {
                    if (vSet.TryGetValue(vdata, out VertexData vold))
                    {
                        vold.flags |= vdata.flags;
                    }
                    else
                        vSet.Add(vdata);
                }
            }
            SortedSet<int> usedVertices = new();
            foreach (VertexData data in vSet)
            {
                Shape vertex = new() { PartID = $"Vertex_{data.flags}", SubModel = true };
                usedVertices.Add(data.flags);
                sb.AppendLine(vertex.Print(data.x, data.y, -2.5, LBGid));
            }

            foreach (int i in usedVertices)
                DefineVertex(sb, i);
            
            MetaData.StartSubModel(sb,BasePart(false));
            DefineRhomb(sb, false);
            MetaData.StartSubModel(sb,  BasePart(true));
            DefineRhomb(sb, true);
        }

        static int EncodeFlag(int rotation)=> 1 <<  (((360 + rotation)% 360-18)/36);
        

        static IEnumerable<int> DecodeFlag(int flag)
        {
            int mask = 1;
            for (int i = 0; i < 5; i++)
            {
                if ((flag & mask) == mask)
                    yield return 18 + i * 36;
                mask <<= 1;
            }
            for (int i = 5; i < 10; i++)
            {
                if ((flag & mask) == mask)
                    yield return  (18 + i * 36)-360;
                mask <<= 1;
            }
        }


        static IEnumerable<VertexData> VerticesFromRhomb(RhombData data)
        {
            int angle = (data.thin) ? 18 : 54;

            int sangle = angle + data.rotation;
            if (sangle < 180) sangle += 360;
            if (sangle > 180) sangle -= 360;
            double dx1 = c[sangle] * InitialScale;
            double dy1 = s[sangle] * InitialScale;
            sangle = 180 -angle + data.rotation;
            if (sangle < 180) sangle += 360;
            if (sangle > 180) sangle -= 360;
            double dx2 = c[sangle] * InitialScale;
            double dy2 = s[sangle] * InitialScale;
            yield return new VertexData() { x = data.x, y = data.y, flags = (EncodeFlag(data.rotation + angle) | EncodeFlag(data.rotation + 180 - angle)) };
            yield return new VertexData() { x = data.x + dx1, y = data.y + dy1, flags = (EncodeFlag(data.rotation + 180 - angle) | EncodeFlag(data.rotation + 180 + angle))};
            yield return new VertexData() { x = data.x + dx2, y = data.y + dy2, flags =(EncodeFlag(data.rotation + angle) | EncodeFlag(data.rotation - angle) )};
            yield return new VertexData() { x = data.x +dx1+dx2, y = data.y +dy1+ dy2, flags =( EncodeFlag(data.rotation - angle) | EncodeFlag(data.rotation + 180 + angle))};
        }



        public static IEnumerable<RhombData> Recurse(RhombData old)
        {
            foreach (var (x, y, rotation, thin) in Rule(old.thin))
            {
                int newRotation = (int) rotation + old.rotation;
                if (newRotation < -180) newRotation += 360;
                if (newRotation > 180) newRotation -= 360;
                yield return new RhombData()
                {
                    x = c[old.rotation] * x - s[old.rotation] * y + old.x * ScaleFactor,
                    y = s[old.rotation] * x + c[old.rotation] * y + old.y * ScaleFactor,
                    rotation = newRotation,
                    thin = thin
                };
            }
        }


        private static readonly List<(double x, double y, int rotation, bool thin)>[] m_rules;
        static PenroseRhomb()
        {
            for (int i = -10; i <= 10; i++)
            {
                c.Add(i * 18, Math.Cos(i * Math.PI / 10.0));
                s.Add(i * 18, Math.Sin(i * Math.PI / 10.0));

            }
            m_rules = new[]
            {
                new List<(double x, double y, int rotation, bool thin)> //thick longest side vertical, origin at bottom
                {
                    (0, 1 + 2 * c[36], 180, false),
                    (c[18], 1 + s[18], 144, false),
                    (-c[18], 1 + s[18], -144, false),
                    (c[18], 1 + s[18], 36, true),
                    (-c[18], 1 + s[18], -36, true)
                },
                new List<(double x, double y, int rotation, bool thin)> //thin longest side horizontal, origin at bottom
                {
                    (0, 1, -108, true),
                    (0, 1, 108, true),
                    (c[54] +c[18] , s[54]-s[18], 108, false),
                    (-c[54] -c[18] , s[54]-s[18], -108, false)
                }
            };
        } 

        public  static List<(double x, double y, int rotation, bool thin)> Rule(bool thin)
        {
            return m_rules[thin?1:0];
        }

        public static void DefineRhomb(StringBuilder sb, bool thin)
        {
            // Shape liftarm = new() { PartID = "40490" };
            Shape liftarm = new() { PartID = "32525" };
            int angle = thin ? 18 : 54;
            int color = ColorMap.Get(m_colors[thin ? 1 : 0]).id;
            double l = InitialScale;
            double x = 0.5 * l * c[angle] - 0.5 * s[angle];
            double y = 0.5 * l * s[angle] - 0.5 * c[angle];
            double dy = l * s[angle];
            double cangle = 90 - angle;
            sb.AppendLine(liftarm.Rotate(cangle).Print(x, y + dy, 0, color));
            sb.AppendLine(liftarm.Rotate(-cangle).Print(-x, y + dy, 0, color));
            sb.AppendLine(liftarm.Rotate(-cangle).Print(x, -y + dy, 0, color));
            sb.AppendLine(liftarm.Rotate(cangle).Print(-x, -y + dy, 0, color));
            double dpx = 3*c[angle];
            double dpy = 3*s[angle];
            Shape pin = new() { PartID = "2780", SwitchXZ = true };
            sb.AppendLine(pin.Print(x - dpx, y + dpy + dy, -1.25, BlackId));
            sb.AppendLine(pin.Print(x + dpx, y - dpy + dy, -1.25, BlackId));
            sb.AppendLine(pin.Print(-x - dpx, y - dpy + dy, -1.25, BlackId));
            sb.AppendLine(pin.Print(-x + dpx, y + dpy + dy, -1.25, BlackId));
            sb.AppendLine(pin.Print(x - dpx, -y - dpy + dy, -1.25, BlackId));
            sb.AppendLine(pin.Print(x + dpx, -y + dpy + dy, -1.25, BlackId));
            sb.AppendLine(pin.Print(-x - dpx,- y + dpy + dy, -1.25, BlackId));
            sb.AppendLine(pin.Print(-x + dpx, -y - dpy + dy, -1.25, BlackId));

        }

        static readonly int LBGid = ColorMap.Get("Light_Bluish_Grey").id;
        static readonly int BlackId = ColorMap.Get("Black").id;
        static readonly int RedId = ColorMap.Get("Red").id;

        public static void DefineVertex(StringBuilder sb, int flags)
        {
            Shape rotor = new() { PartID = "80273" };
            MetaData.StartSubModel(sb, $"Vertex_{flags}");
            sb.AppendLine(rotor.Print(0, 0, -2.5, LBGid));
            sb.AppendLine(rotor.Rotate(180).Print(0, 0, -5, LBGid));
            Shape pinAxleConnector3L = new() { PartID = "42003", SwitchYZ = true };
            Shape pin = new() { PartID = "2780", SwitchXZ = true };
            Shape pinDouble = new() { PartID = "65098", SwitchXZ = true };
            Shape doubleSplit = new(){ PartID = "41678" };
            Shape redAxle = new(){ PartID = "32062" };
            foreach (int rotation in DecodeFlag(flags))
            {
                sb.AppendLine(pinAxleConnector3L.Rotate(rotation + 90).Print(3 * c[rotation], 3 * s[rotation], 0, LBGid));
                sb.AppendLine(doubleSplit.Rotate(rotation + 90).Print(4 * c[rotation], 4 * s[rotation], 0, LBGid));
                sb.AppendLine(redAxle.Rotate(rotation + 90).Print(4 * c[rotation], 4 * s[rotation], 0, RedId));
                if ((rotation - 18 + 360) % 72 == 0) // downer
                {
                    sb.AppendLine(pinDouble.Rotate(rotation).Print(2.5 * c[rotation], 2.5 * s[rotation], -2.5, LBGid));
                }
                else
                {
                    sb.AppendLine(pin.Print(2 * c[rotation], 2 * s[rotation], -1.25, BlackId));
                    sb.AppendLine(pin.Print(3 * c[rotation], 3 * s[rotation], -1.25, BlackId));
                }
            }

        }
    }
}
