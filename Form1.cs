using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Forms;
using System.Linq;
using System.Drawing.Imaging;
using Microsoft.VisualBasic.Devices;

namespace BrunnianLink
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            tbPath.Text = Properties.Settings.Default.OutputPath;

            cbModelChoice.SelectedIndex = Properties.Settings.Default.SelectedModel;
            nudLevel.Value = Properties.Settings.Default.SelectedLevel;
        }

        private void BtOpen_Click(object sender, EventArgs e)
        {
            //if (!File.Exists(tbPath.Text))
            BtGenerate_Click(sender, e);
            using Process fileopener = new();

            //fileopener.StartInfo.FileName = "explorer";
            fileopener.StartInfo.FileName = @"C:\Program Files\Studio 2.0\Studio.exe";
            fileopener.StartInfo.Arguments = "\"" + tbPath.Text + "\"";
            fileopener.Start();
        }

        private static readonly TilingGenerator[] tileGens = {
            new() { Rule = new PinWheelRules() },
            new() { Rule = new ChairRule() },
            new() { Rule = new TennisRule() },
            new() { Rule = new WandererReflectionsRule() },
            new() { Rule = new WandererRotationsRule() },
            new() { Rule = new AmmannBeenkerRule()},
            new() { Rule = new ShieldRule() },
            new() {Rule = new LabyrinthRule() }
        };
        private void BtGenerate_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new();
            /*
            sb.Append("0 Test\r\n0 Name:  Test\r\n0 Author:  Michael Verschaeve\r\n0 CustomBrick\r\n");
            Shape bp = new BasePlate(32);
            sb.AppendLine(bp.Print(0, 0, 0, ColorMap.Get("Light_Bluish_Grey").id));
            Shape plate = new Plate(4, 2);
            sb.AppendLine(plate.Print(0, 0, 1, ColorMap.Get("Tan").id));
            Shape wedgePlate1 = new() { Depth = 2, Width = 4, Height = 1, PartID = "65426" }; //left
            sb.AppendLine(wedgePlate1.Print(0, 0, 2, ColorMap.Get("White").id));
            Shape wedgePlate2 = new() { Depth = 2, Width = 4, Height = 1, PartID = "65429" }; //right
            sb.AppendLine(wedgePlate2.Print(0, 0, 3, ColorMap.Get("Black").id));
            */
            int level = (int)nudLevel.Value;
            int modelChoice = cbModelChoice.SelectedIndex;
            string file = tbPath.Text;
            string parentDir = Directory.GetParent(file)?.FullName!;
            if (!Directory.Exists(parentDir))
                Directory.CreateDirectory(parentDir);
            Properties.Settings.Default.SelectedLevel = level;
            Properties.Settings.Default.SelectedModel = modelChoice;
            Properties.Settings.Default.OutputPath = file;
            Properties.Settings.Default.Save();

            switch (modelChoice)
            {
                case 0:
                    BrunnianGenerator.Generate(sb);
                    break;
                case 1:
                    Gosper.Generate(sb, level);
                    break;
                case 2:
                    //sb.AppendLine(new Plate(2,2).Print(0,1,0,ColorMap.Get("Red").id));
                    //sb.AppendLine(new Plate(2,2).Print(0, -1, 0, ColorMap.Get("Blue").id));
                    EinsteinHat.Generate(sb, level);
                    break;
                case 3:
                    //sb.AppendLine(new Plate(2,2).Print(0,1,0,ColorMap.Get("Red").id));
                    //sb.AppendLine(new Plate(2,2).Print(0, -1, 0, ColorMap.Get("Blue").id));
                    EinsteinHat2.Generate(sb, level);
                    break;
                case 4:
                    Hilbert3D.Generate(sb, level);
                    break;
                case 5:

                    for (int p1 = -5; p1 <= 5; p1++)
                        for (int p2 = -5; p2 <= 5; p2++)
                        {
                            int x = p1 * 4 + p2 * 2;
                            int y = p2 * 4 - p1 * 2;
                            /*if (x - 3.5 < -24) continue;
                            if (x + 3.5 > 24) continue;
                            if (y - 3.5 < -24) continue;
                            if (y + 3.5 > 24) continue;*/
                            sb.AppendLine(new Shape() { PartID = "22385" }.Print(x, y + 1.5, 0, ColorMap.Get("Red").id));
                            sb.AppendLine(new Shape() { PartID = "22385" }.Rotate(180).Print(x, y - 1.5, 0, ColorMap.Get("Blue").id));
                            sb.AppendLine(new Shape() { PartID = "22385" }.Rotate(90).Print(x - 1.5, y, 0, ColorMap.Get("Lime").id));
                            sb.AppendLine(new Shape() { PartID = "22385" }.Rotate(-90).Print(x + 1.5, y, 0, ColorMap.Get("White").id));

                        }
                    break;
                case 6:
                    OctaFlake.Generate(sb, level);
                    break;
                case 7:
                    PenroseRhomb.Generate(sb, level);
                    break;
                default:
                    tileGens[cbModelChoice.SelectedIndex - 8].Generate(sb, level);
                    break;
            }
            var utf8WithoutBom = new System.Text.UTF8Encoding(false);
            File.WriteAllText(file, sb.ToString(), utf8WithoutBom);
        }

        private void BtViewText_Click(object sender, EventArgs e)
        {
            BtGenerate_Click(sender, e);
            using Process fileopener = new();

            fileopener.StartInfo.FileName = "notepad";
            fileopener.StartInfo.Arguments = "\"" + tbPath.Text + "\"";
            fileopener.Start();
        }
    }
}