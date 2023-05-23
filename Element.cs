using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    static public class MetaData
    {
        public static void StartSubModel(StringBuilder sb, string modelName)
        {
            sb.Append($"0 FILE {modelName}\r\n0 {modelName}\r\n0 Name: {modelName}\r\n0 Author:  Michael Verschaeve\r\n0 CustomBrick\r\n");
        }

        public static void BuildStepFinished(StringBuilder sb)
        {
            sb.AppendLine($"0 STEP");
        }



    }




    public class Shape : ICloneable
    {
        //these values are in LDU
        protected int sx;
        protected int sy;
        protected int sz;
        protected double ax;
        protected double az;
        protected string partID = "";


        public object Clone()
        {
            Shape clone = new()
            {
                sx = (int)sx,
                sy = (int)sy,
                sz = (int)sz,
                ax = (double)ax,
                az = (double)az,
                partID = partID,
                rotationAngleDegrees = rotationAngleDegrees,
                SubModel = SubModel,
                SwitchXZ = SwitchXZ,
                SwitchYZ = SwitchYZ,
                SwitchYZ2 = SwitchYZ2
            };
            return clone;
        }

        public int Height { get => sy / 8; set => sy = 8 * value; }
        public int Width { get => sx / 20; set => sx = 20 * value; }
        public int Depth { get => sz / 20; set => sz = 20 * value; }
        public string PartID { get => partID; set => partID = value; }

        public bool SwitchXZ { get; set; }
        public bool SwitchYZ { get; set; }
        public bool SwitchYZ2 { get; set; }
        public bool SubModel { get; set; }

        protected double rotationAngleDegrees=0.0;

        public string RotMat()
        {
            double c = Math.Cos(rotationAngleDegrees*Math.PI/180.0);
            double s = Math.Sin(rotationAngleDegrees * Math.PI / 180.0);
            if (SwitchYZ) //x,yswitch cols, other axle minus to not go chiral
            {
                return $"{c} {-s} 0 0 0 -1 {s} {c} 0";
            }
            else if (SwitchYZ2) //x,yswitch cols, other axle minus to not go chiral
            {
                return $"{c} {s} 0 0 0 1 {s} {-c} 0";
            }
            else if (SwitchXZ) {
                return $"0 {c} {-s} -1 0 0 0 {s} {c}";
            }
            return $"{c} 0 {-s} 0 1 0 {s} 0 {c}";
        }

        public virtual Shape Rotate(double angle = 90, bool degrees = true)
        {
            Shape res = (this.Clone() as Shape)!;
            res?.RotateThis(angle, degrees);
            return res!;
        }

        public virtual void RotateThis(double angle = 90, bool degrees = true)
        {
            if (!degrees) angle *= 180 * angle/Math.PI;
            rotationAngleDegrees += angle;
            while (rotationAngleDegrees > 180.000001) rotationAngleDegrees -= 360;
            while (rotationAngleDegrees < -180.000001) rotationAngleDegrees += 360;

            bool rot90 = Math.Abs(Math.Abs(angle)-90) < 1.0e-6;

            //keep original sizes if not multiples of 90, not much point in adapting them
            if (rot90)
                (sx, sz) = (sz, sx);

            double c = Math.Cos(rotationAngleDegrees * Math.PI / 180.0);
            double s = Math.Sin(rotationAngleDegrees * Math.PI / 180.0);

            (ax,az) = (c*ax-s*az,s*ax+c*az);
        }

        public string Print(double centerXStuds, double centerYStuds, double topZPlates, int color)
        {
            StringBuilder sb = new();
            sb.Append($"1 {color} ");
            double dy = -topZPlates * 8;
            double dx = centerXStuds * 20 + ax;
            double dz = centerYStuds * 20 + az;
            sb.Append($" {dx} {dy} {dz} ");
            sb.Append(RotMat());
            sb.Append(' ');
            sb.Append(partID);
            if (!SubModel) //reference to orignal part
                sb.Append(".dat");
            return sb.ToString();
        }

    }

    public class BasePlate : Shape
    {
        public BasePlate(int size)
        {
            sx = sz = size * 20;
            sy = 0; //don't know, don't care, baseplates are always at bottom
            ax = az = 0;
            partID = size switch
            {
                32 => "3811",
                48 => "4186",
                _ => throw new ArgumentException("unsupported size"),
            };
        }

        public override Shape Rotate(double _, bool __)
        {
            return this;
        }

        public override void RotateThis(double _ , bool __ )
        {

        }
    }

    public class Bow : Shape
    {
        public Bow(int size)
        {
            sx = sz = size * 20;
            sy = 8; //tile

            if (size > 2)
            {
                ax = -sx / 2;
                az = sz / 2;
            }
            else if (size == 2)
            {
                ax =-10;
                az =10;
            }
            else
                ax = az = 0;
            partID = size switch
            {
                1 => "25269",
                2 => "27925",
                3 => "79393",
                4 => "27507",
                _ => throw new ArgumentException("unsupported size"),
            };
        }
    }

    public class Tile : Shape
    {
        static readonly Dictionary<(int, int), string> tileIDs = new()
        {
            {(1,1), "3070b" },
            {(1,2), "3069b" },
            {(1,3), "63864" },
            {(1,4), "2431" },
            {(1,6), "6636" },
            {(1,8), "4162" },
            {(2,2), "3068b" },
            {(2,3), "26603" },
            {(2,4), "87079" },
            {(2,6), "69729" },
            {(3,6), "6934a" }, //scala
            {(4,4), "1751" },
            {(6,6), "10202" },
            {(8,16), "90498" }
        };

        public static string XYPartID(int sizeY,int sizeX) => tileIDs[(sizeY, sizeX)];

        public Tile(int sizeX, int sizeY = 1)
        {
            if (sizeX < sizeY)
            {
                (sizeX, sizeY) = (sizeY, sizeX);
                rotationAngleDegrees = 90;
            }

            sz = 20 * sizeY;
            sx = 20 * sizeX;
            sy = 8;
            ax = az = 0;
            partID = XYPartID(sizeY, sizeX);
        }
    }

    public class Plate : Shape
    {
        static readonly Dictionary<(int, int), string> plateIDs = new()
        {
            {(1,1), "3024" },
            {(1,2), "3023" },
            {(1,3), "3623" },
            {(1,4), "3710" },
            {(1,5), "78329" },
            {(1,6), "3666" },
            {(1,8), "3460" },
            {(1,10), "4477" },
            {(1,12), "60479" },

            {(2,2), "3022" },
            {(2,3), "2021" },
            {(2,4), "3020" },
            {(2,6), "3795" },
            {(2,8), "3034" },
            {(2,10), "3832" },
            {(2,12), "2445" },
            {(2,14), "91988" },
            {(2,16), "4282" },

            {(3,3), "11212" },

            {(4,4), "3031" },
            {(4,6), "3032" }, 
            {(4,8), "3035" },
            {(4,10), "3030" },
            {(4,12), "3029" },

            {(6,6), "3958" },
            {(6,8), "3036" },
            {(6,10), "3033" },
            {(6,12), "3028" },
            {(6,14), "3456" },
            {(6,16), "3027" },
            {(6,24), "3026" },

            {(8,8), "41539" },
            {(8,11), "728" },
            {(8,16), "92438" },

            {(16,16), "91405" }
        };

        public static string XYPartID(int sizeY, int sizeX) => plateIDs[(sizeY, sizeX)];

        public Plate(int sizeX, int sizeY = 1)
        {
            if (sizeX < sizeY)
            {
                (sizeX, sizeY) = (sizeY, sizeX);
                rotationAngleDegrees = 90;
            }

            sz = 20 * sizeY;
            sx = 20 * sizeX;
            sy = 8;
            ax = az = 0;
            partID = XYPartID(sizeY, sizeX);
        }
    }
}
