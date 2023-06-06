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
            int count = 0;
            //store original order
            foreach (var item in cbModelChoice.Items)
                TextToIndex.Add(item?.ToString()??"", count++);
            cbModelChoice.Sorted = true;
        }

        private Dictionary<String, int> TextToIndex = new();

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
            new() { Rule = new DominoRule() },
            new() { Rule = new Domino9Rule() },
            new() { Rule = new SemiHouseRule() },
            new() { Rule = new WandererReflectionsRule() },
            new() { Rule = new WandererRotationsRule() },
            new() { Rule = new AmmannBeenkerRule()},
            new() { Rule = new ShieldRule() },
            new() {Rule = new LabyrinthRule() },
            new() {Rule = new HofStetterArrowedRule() },
            new() {Rule = new Wunderlich(){CurveType=0} },
            new() {Rule = new Wunderlich(){CurveType=1} },
            new() {Rule = new Wunderlich(){CurveType=2} },
            new() {Rule = new MiniTangramRule()},
            new() {Rule = new SocolarRule(false)},
            new() {Rule = new SocolarRule(true)},
        };
        private void BtGenerate_Click(object sender, EventArgs e)
        {
            if (cbModelChoice.SelectedIndex == -1) return;

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
            int modelChoice = TextToIndex[cbModelChoice.SelectedItem.ToString()];
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
                    ChiralSpectre.Generate(sb, level);
                    break;
                case 5:
                    Hilbert3D.Generate(sb, level);
                    break;
                case 6:

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
                case 7:
                    OctaFlake.Generate(sb, level);
                    break;
                case 8:
                    PenroseRhomb.Generate(sb, level);
                    break;
                case 9:
                    new MazeGenerator(level).Generate(sb);
                    break;
                default:
                    tileGens[modelChoice - 10].Generate(sb, level);
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