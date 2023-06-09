﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    //used for both wheel tiling and socolar itself
    public class SocolarRule : SubstitutionRule
    {
        private readonly bool m_isWheelTiling;

        private static readonly string[] m_colorsSocolar = new[] { "Magenta", "Dark_Turquoise", "Orange", "Orange" };
        private static readonly string[] m_colorsWheel = new[] { "Violet", "Aqua", "Bright_Light_Orange", "Sand_Green", "Tan", "White", "Lime", "Medium_Azure"};
        public override string[] Colors => m_isWheelTiling?m_colorsWheel: m_colorsSocolar;

        public override bool ColorByState => !m_isWheelTiling;

        public override string MainName => m_isWheelTiling ? "Wheel_Tiling" : "Socolar";

        private readonly string folder;
        public SocolarRule(bool isWheeling = false)
        {
            m_isWheelTiling = isWheeling;
            string dir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.FullName!;
            dir = Directory.GetParent(dir)?.FullName!;
            dir = Directory.GetParent(dir)?.FullName!;
            dir = Directory.GetParent(dir)?.FullName!;
            if (!isWheeling)
                dir = Path.Combine(dir, "Socolar");
            else
                dir = Path.Combine(dir, "SpectrePolies");
            folder = dir;
        }

        public override int StateCount => 4;

        private static readonly double m_scaleFactor = 2.0 + Math.Sqrt(3.0);
        public override double ScaleFactor => m_scaleFactor;

        public override double InitialScale => m_isWheelTiling ? 6*Math.Sqrt(2.0) : 4;

        private static (double x, double y, double rotation, int state) MirrorX((double x, double y, double rotation, int state) t)
        {
            int newstate = t.state switch { 2 => 3, 3 => 2, _ => t.state };
            double newrot = t.state switch { 1 => -90 - t.rotation, _ => -t.rotation }; ;
            return (-t.x, t.y, newrot, newstate);
        }

        //state 0: rhombus, long diagonal horizontal, origin at center, nonchiral
        //state 1: square, centered, mirror image is rotate -90 degrees
        //state 2: hexagon, centered
        //state 3: hexagon mirror
        // state 7-x, mirror of x
        public override List<(double x, double y, double rotation, int state)> Rule(int state)
        {
            List<(double x, double y, double rotation, int state)> res = new();
            double c15 = Math.Cos(Math.PI / 12);
            double s15 = Math.Sin(Math.PI / 12);
            switch (state)
            {
                case 0:
                    res.Add((0, 0, 90, 0));
                    res.Add((c15 + s15, 0, 0, 0));
                    res.Add((0.5 * (s15 + c15), 0.5 * (c15 + s15), 180 + 15, 1));
                    res.Add((0.5 * (s15 + c15), -0.5 * (c15 + s15), -90 - 15, 1));
                    res.Add((2 * c15 + s15 + 0.5 * Math.Sqrt(2.0), 0, 45, 1));
                    res = res.Concat(res.Skip(1).Select(t => MirrorX(t))).ToList();
                    break;
                case 1:
                    res.Add((0, 0, 0, 1));
                    double t = (1 + Math.Sqrt(2) * c15) * 0.5;
                    res.Add((t, t, 45, 0));
                    res.Add((-t, -t, 45, 0));
                    res.Add((t, -t, -45, 0));
                    res.Add((-t, t, -45, 0));
                    t = (1 + Math.Sqrt(3)) * 0.5;
                    res.Add((-t, 0, 30, 2));
                    res.Add((t, 0, -30, 3));
                    res.Add((0, -t, 120, 2));
                    res.Add((0, t, 60, 3));
                    break;
                case 2:
                    double hexToSquareDist = (1 + Math.Sqrt(3.0)) * 0.5;
                    double hexToSquareDist_s60 = (3 + Math.Sqrt(3.0)) * 0.25;
                    Poly rhombus = new() { Points = new List<(double x, double y)>() { (c15, 0.0), (0.0, s15), (-c15, 0.0), (0.0, -s15) } };
                    Poly square = new() { Points = new List<(double x, double y)>() { (0.5, -0.5), (0.5, 0.5), (-0.5, 0.5), (-0.5, -0.5) } };
                    Poly hexagon = new() { Points = Enumerable.Range(0, 6).Select(i => (Math.Cos(i * Math.PI / 3.0), Math.Sin(i * Math.PI / 3.0))) };
                    res = Rule(1).Select(t => (t.x, t.y - hexToSquareDist, t.rotation, t.state)).ToList();
                    //mirror next 9
                    List<(double x, double y, double rotation, int state)> toCopyAndMirror = new();
                    Poly sq2 = square.Fit(hexagon.Fit(square.Fit(hexagon, 4, 1), 2, 5), 3, 1);
                    toCopyAndMirror.Add((sq2.OffsetX, sq2.OffsetY, sq2.RotationDeg, 1));                  
                    sq2 = square.Fit(hexagon, 2, 1);
                    toCopyAndMirror.Add((sq2.OffsetX, sq2.OffsetY, sq2.RotationDeg, 1));
                    Poly hex2 = hexagon.Fit(sq2, 3, 5);
                    toCopyAndMirror.Add((hex2.OffsetX, hex2.OffsetY, hex2.RotationDeg, 2));
                    Poly rhomb2 = rhombus.Fit(hex2, 4, 0);
                    toCopyAndMirror.Add((rhomb2.OffsetX, rhomb2.OffsetY, rhomb2.RotationDeg, 0));
                    sq2 = square.Fit(rhomb2, 3, 0);
                    toCopyAndMirror.Add((sq2.OffsetX, sq2.OffsetY, sq2.RotationDeg, 1));
                    sq2 = square.Fit(rhomb2, 2, 3);
                    toCopyAndMirror.Add((sq2.OffsetX, sq2.OffsetY, sq2.RotationDeg, 1));
                    rhomb2 = rhombus.Fit(sq2, 2, 2);
                    toCopyAndMirror.Add((rhomb2.OffsetX, rhomb2.OffsetY, rhomb2.RotationDeg, 0));
                    rhomb2 = rhombus.Fit(hex2, 0, 3);
                    toCopyAndMirror.Add((rhomb2.OffsetX, rhomb2.OffsetY, rhomb2.RotationDeg, 0));
                    sq2 = square.Fit(rhomb2, 1, 0);
                    toCopyAndMirror.Add((sq2.OffsetX, sq2.OffsetY, sq2.RotationDeg, 1));
                    res.AddRange(toCopyAndMirror);
                    // mirror last 9

                    res.AddRange(toCopyAndMirror.Select(t => MirrorX(t)));
                    //add 4 more at the top
                    rhomb2 = rhombus.Fit(hexagon, 1, 3);
                    res.Add((rhomb2.OffsetX, rhomb2.OffsetY, rhomb2.RotationDeg, 0));
                    hex2 = hexagon.Fit(rhomb2, 0, 2);
                    res.Add((hex2.OffsetX, hex2.OffsetY, hex2.RotationDeg, 3)); //is a mirror
                    sq2 = square.Fit(rhomb2, 1, 0);
                    res.Add((sq2.OffsetX, sq2.OffsetY, sq2.RotationDeg, 1));
                    rhomb2 = rhombus.Fit(sq2, 2, 2);
                    res.Add((rhomb2.OffsetX, rhomb2.OffsetY, rhomb2.RotationDeg, 0));
                    break;
                case 3:
                    res = Rule(2).Select(t => MirrorX(t)).ToList(); break;
            }
            return res;
        }

        private readonly string[] WheelingBaseParts = new[] { "Bow", "", "Flower", "Flower" };
        private readonly string[] SocolarBaseParts = new[] { "", Plate.XYPartID(4, 4) + ".dat", "Hexagon", "HexagonMirror" };
        public override string BasePart(int state, int color) {

            if (m_isWheelTiling)
            {
                string p = WheelingBaseParts[state];
                return string.IsNullOrEmpty(p)?"":(p + "_" + color.ToString());
            }
            else
                return SocolarBaseParts[state];
        }
        public override bool Level0IsComposite => true;
        public override void DefineCompositeBasePart(StringBuilder sb, int state, int color)
        {
            if (!m_isWheelTiling)
                DefineCompositeBasePartSocolar(sb, state);
            else
                DefineCompositeBasePartWheelTiling(sb, state, color);
        }

        private void DefineCompositeBasePartSocolar(StringBuilder sb, int state)
        {
            if (state < 2) return; //empty and simple plate
            if (state == 2)
            {
                sb.AppendLine(new Shape() { PartID = "hexBase", SubModel = true }.Rotate(-60).Print(0, 0, -3.5, 16));
                return;
            }
            sb.AppendLine(new Shape() { PartID = "hexBase", SubModel = true }.Rotate(60).Print(0, 0, -3.5, 16));
            MetaData.StartSubModel(sb, "hexBase");
            string file = Path.Combine(folder, "hexagon.ldr");
            string[] fileLines = File.ReadAllLines(file);
            string specialLine = fileLines.First(line => line.EndsWith("3003.dat"));
            var parts = specialLine.Split(' ');
            double ldux_offset = double.Parse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture);
            double lduy_offset = double.Parse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture);
            double lduz_offset = double.Parse(parts[4], NumberStyles.Float, CultureInfo.InvariantCulture);
            foreach (string line in fileLines.SkipWhile(line => line.StartsWith('0')))
            {
                parts = line.Split(' ');
                double ldux = double.Parse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture);
                double lduy = double.Parse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture);
                double lduz = double.Parse(parts[4], NumberStyles.Float, CultureInfo.InvariantCulture);
                sb.Append($"1 {parts[1]} {ldux - ldux_offset} {lduy - lduy_offset} {lduz - lduz_offset} ");
                sb.AppendLine(string.Join(" ", parts.Skip(5)));
            }
        }

        private void DefineCompositeBasePartWheelTiling(StringBuilder sb, int state, int color)
        {
            if (state == 1)
            {
                return; //squares dissappear
            }
            //MetaData.StartSubModel(sb,BasePart(state, colorId));
            string[] partNames = { "Bow_base_", "Flower_base_" };
            int i = state == 0 ? 0 : 1;
            string baseName = partNames[i] + color.ToString();
            Shape shape = new() { PartID = baseName, SubModel = true };
            if (state == 0)
            {
                sb.AppendLine(shape.Rotate(60).Print(3 * Math.Sqrt(3.0), -3 * Math.Sqrt(3.0), 0, 16));
            }
            else
            {
                Shape bow = new() { PartID = baseName, SubModel = true };
                sb.AppendLine(shape.Rotate(45-180).Print(0, 3  * Math.Sqrt(2.0) * (Math.Sqrt(3.0) + 1), 0, 16));
            }

            //copy .ldr contents...
            string[] templateFiles = { "bow.ldr", "flower.ldr" };

            string[] fileLines = File.ReadAllLines(Path.Combine(folder, templateFiles[i]));
            string specialLine = fileLines.First(line => line.EndsWith("6141.dat"));
            var parts = specialLine.Split(' ');
            double ldux_offset = double.Parse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture) - 10;
            double lduy_offset = double.Parse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture);
            double lduz_offset = double.Parse(parts[4], NumberStyles.Float, CultureInfo.InvariantCulture) - 10;

            int colorId = ColorMap.Get(Colors[color]).id;

            MetaData.StartSubModel(sb, baseName);
            foreach (string line in fileLines.SkipWhile(line => line.StartsWith('0')))
            {
                parts = line.Split(' ');
                int pcolor = int.Parse(parts[1]);
                if (pcolor == 4) //the red dot,
                    continue;
                else if (pcolor == 15)
                    pcolor = colorId;
                double ldux = double.Parse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture);
                double lduy = double.Parse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture);
                double lduz = double.Parse(parts[4], NumberStyles.Float, CultureInfo.InvariantCulture);
                sb.Append($"1 {pcolor} {ldux - ldux_offset} {lduy - lduy_offset} {lduz - lduz_offset} ");
                sb.AppendLine(string.Join(" ", parts.Skip(5)));
            }
        }

        public override (double x, double y, double rotation, int state)[] StartStates => new[]{(0.0,0.0,0.0,3)};
    }
}
