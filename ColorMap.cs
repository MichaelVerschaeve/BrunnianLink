using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    static class ColorMap
    {
        private static readonly Dictionary<string, (int id, Color c)> colorMap = new()
        {
            {"Black",(0,ColorTranslator.FromHtml("#1B2A34"))},
            {"Blue",(1,ColorTranslator.FromHtml("#1E5AA8"))},
            {"Green",(2,ColorTranslator.FromHtml("#00852B"))},
            {"Dark_Turquoise",(3,ColorTranslator.FromHtml("#069D9F"))},
            {"Red",(4,ColorTranslator.FromHtml("#B40000"))},
            {"Dark_Pink",(5,ColorTranslator.FromHtml("#D3359D"))},
            {"Brown",(6,ColorTranslator.FromHtml("#543324"))},
            {"Light_Grey",(7,ColorTranslator.FromHtml("#8A928D"))},
            {"Dark_Grey",(8,ColorTranslator.FromHtml("#545955"))},
            {"Light_Blue",(9,ColorTranslator.FromHtml("#97CBD9"))},
            {"Bright_Green",(10,ColorTranslator.FromHtml("#58AB41"))},
            {"Light_Turquoise",(11,ColorTranslator.FromHtml("#00AAA4"))},
            {"Salmon",(12,ColorTranslator.FromHtml("#F06D61"))},
            {"Pink",(13,ColorTranslator.FromHtml("#F6A9BB"))},
            {"Yellow",(14,ColorTranslator.FromHtml("#FAC80A"))},
            {"White",(15,ColorTranslator.FromHtml("#F4F4F4"))},
            {"Light_Green",(17,ColorTranslator.FromHtml("#ADD9A8"))},
            {"Light_Yellow",(18,ColorTranslator.FromHtml("#FFD67F"))},
            {"Tan",(19,ColorTranslator.FromHtml("#B0A06F"))},
            {"Light_Violet",(20,ColorTranslator.FromHtml("#AFBED6"))},
            {"Purple",(22,ColorTranslator.FromHtml("#671F81"))},
            {"Dark_Blue_Violet",(23,ColorTranslator.FromHtml("#0E3E9A"))},
            {"Orange",(25,ColorTranslator.FromHtml("#D67923"))},
            {"Magenta",(26,ColorTranslator.FromHtml("#901F76"))},
            {"Lime",(27,ColorTranslator.FromHtml("#A5CA18"))},
            {"Dark_Tan",(28,ColorTranslator.FromHtml("#897D62"))},
            {"Bright_Pink",(29,ColorTranslator.FromHtml("#FF9ECD"))},
            {"Medium_Lavender",(30,ColorTranslator.FromHtml("#A06EB9"))},
            {"Lavender",(31,ColorTranslator.FromHtml("#CDA4DE"))},
            {"Very_Light_Orange",(68,ColorTranslator.FromHtml("#FDC383"))},
            {"Bright_Reddish_Lilac",(69,ColorTranslator.FromHtml("#8A12A8"))},
            {"Reddish_Brown",(70,ColorTranslator.FromHtml("#5F3109"))},
            {"Light_Bluish_Grey",(71,ColorTranslator.FromHtml("#969696"))},
            {"Dark_Bluish_Grey",(72,ColorTranslator.FromHtml("#646464"))},
            {"Medium_Blue",(73,ColorTranslator.FromHtml("#7396C8"))},
            {"Medium_Green",(74,ColorTranslator.FromHtml("#7FC475"))},
            {"Light_Pink",(77,ColorTranslator.FromHtml("#FECCCF"))},
            {"Light_Nougat",(78,ColorTranslator.FromHtml("#FFC995"))},
            {"Medium_Nougat",(84,ColorTranslator.FromHtml("#AA7D55"))},
            {"Medium_Lilac",(85,ColorTranslator.FromHtml("#441A91"))},
            {"Medium_Brown",(86,ColorTranslator.FromHtml("#7B5D41"))},
            {"Blue_Violet",(89,ColorTranslator.FromHtml("#1C58A7"))},
            {"Nougat",(92,ColorTranslator.FromHtml("#BB805A"))},
            {"Light_Salmon",(100,ColorTranslator.FromHtml("#F9B7A5"))},
            {"Violet",(110,ColorTranslator.FromHtml("#26469A"))},
            {"Medium_Violet",(112,ColorTranslator.FromHtml("#4861AC"))},
            {"Medium_Lime",(115,ColorTranslator.FromHtml("#B7D425"))},
            {"Aqua",(118,ColorTranslator.FromHtml("#9CD6CC"))},
            {"Light_Lime",(120,ColorTranslator.FromHtml("#DEEA92"))},
            {"Light_Orange",(125,ColorTranslator.FromHtml("#F9A777"))},
            {"Dark_Nougat",(128,ColorTranslator.FromHtml("#AD6140"))},
            {"Very_Light_Bluish_Grey",(151,ColorTranslator.FromHtml("#C8C8C8"))},
            {"Bright_Light_Orange",(191,ColorTranslator.FromHtml("#FCAC00"))},
            {"Bright_Light_Blue",(212,ColorTranslator.FromHtml("#9DC3F7"))},
            {"Rust",(216,ColorTranslator.FromHtml("#872B17"))},
            {"Reddish_Lilac",(218,ColorTranslator.FromHtml("#8E5597"))},
            {"Lilac",(219,ColorTranslator.FromHtml("#564E9D"))},
            {"Bright_Light_Yellow",(226,ColorTranslator.FromHtml("#FFEC6C"))},
            {"Sky_Blue",(232,ColorTranslator.FromHtml("#77C9D8"))},
            {"Dark_Blue",(272,ColorTranslator.FromHtml("#19325A"))},
            {"Dark_Green",(288,ColorTranslator.FromHtml("#00451A"))},
            {"Flamingo_Pink",(295,ColorTranslator.FromHtml("#FF94C2"))},
            {"Dark_Brown",(308,ColorTranslator.FromHtml("#352100"))},
            {"Maersk_Blue",(313,ColorTranslator.FromHtml("#ABD9FF"))},
            {"Dark_Red",(320,ColorTranslator.FromHtml("#720012"))},
            {"Dark_Azure",(321,ColorTranslator.FromHtml("#469BC3"))},
            {"Medium_Azure",(322,ColorTranslator.FromHtml("#68C3E2"))},
            {"Light_Aqua",(323,ColorTranslator.FromHtml("#D3F2EA"))},
            {"Yellowish_Green",(326,ColorTranslator.FromHtml("#E2F99A"))},
            {"Olive_Green",(330,ColorTranslator.FromHtml("#77774E"))},
            {"Sand_Red",(335,ColorTranslator.FromHtml("#88605E"))},
            {"Medium_Dark_Pink",(351,ColorTranslator.FromHtml("#F785B1"))},
            {"Coral",(353,ColorTranslator.FromHtml("#FF6D77"))},
            {"Earth_Orange",(366,ColorTranslator.FromHtml("#D86D2C"))},
            {"Sand_Purple",(373,ColorTranslator.FromHtml("#75657D"))},
            {"Sand_Green",(378,ColorTranslator.FromHtml("#708E7C"))},
            {"Sand_Blue",(379,ColorTranslator.FromHtml("#70819A"))},
            {"Fabuland_Brown",(450,ColorTranslator.FromHtml("#D27744"))},
            {"Medium_Orange",(462,ColorTranslator.FromHtml("#F58624"))},
            {"Dark_Orange",(484,ColorTranslator.FromHtml("#91501C"))},
            {"Very_Light_Grey",(503,ColorTranslator.FromHtml("#BCB4A5"))},
            {"Light_Orange_Brown",(507,ColorTranslator.FromHtml("#FA9C1C"))},
            {"Chrome_Antique_Brass",(60,ColorTranslator.FromHtml("#645A4C"))},
            {"Chrome_Blue",(61,ColorTranslator.FromHtml("#6C96BF"))},
            {"Chrome_Green",(62,ColorTranslator.FromHtml("#3CB371"))},
            {"Chrome_Pink",(63,ColorTranslator.FromHtml("#AA4D8E"))},
            {"Chrome_Black",(64,ColorTranslator.FromHtml("#1B2A34"))},
            {"Chrome_Gold",(334,ColorTranslator.FromHtml("#DFC176"))},
            {"Chrome_Silver",(383,ColorTranslator.FromHtml("#CECECE"))},
            {"Pearl_Black",(83,ColorTranslator.FromHtml("#0A1327"))},
            {"Copper",(134,ColorTranslator.FromHtml("#764D3B"))},
            {"Pearl_Light_Grey",(135,ColorTranslator.FromHtml("#A0A0A0"))},
            {"Metallic_Blue",(137,ColorTranslator.FromHtml("#5B7590"))},
            {"Pearl_Light_Gold",(142,ColorTranslator.FromHtml("#DEAC66"))},
            {"Pearl_Dark_Gold",(147,ColorTranslator.FromHtml("#83724F"))},
            {"Pearl_Dark_Grey",(148,ColorTranslator.FromHtml("#484D48"))},
            {"Pearl_Very_Light_Grey",(150,ColorTranslator.FromHtml(" #989B99"))},
            {"Pearl_Red",(176,ColorTranslator.FromHtml("#945148"))},
            {"Pearl_Yellow",(178,ColorTranslator.FromHtml("#AB673A"))},
            {"Pearl_Silver",(179,ColorTranslator.FromHtml("#898788"))},
            {"Pearl_White",(183,ColorTranslator.FromHtml("#F6F2DF"))},
            {"Metallic_Bright_Red",(184,ColorTranslator.FromHtml("#D60026"))},
            {"Metallic_Bright_Blue",(185,ColorTranslator.FromHtml("#0059A3"))},
            {"Metallic_Dark_Green",(186,ColorTranslator.FromHtml("#008E3C"))},
            {"Reddish_Gold",(189,ColorTranslator.FromHtml("#AC8247"))},
            {"Lemon_Metallic",(200,ColorTranslator.FromHtml("#708224"))},
            {"Pearl_Gold",(297,ColorTranslator.FromHtml("#AA7F2E"))},
            {"Metallic_Silver",(80,ColorTranslator.FromHtml("#767676"))},
            {"Metallic_Green",(81,ColorTranslator.FromHtml("#C2C06F"))},
            {"Metallic_Gold",(82,ColorTranslator.FromHtml("#DBAC34"))},
            {"Metallic_Dark_Grey",(87,ColorTranslator.FromHtml("#3E3C39"))},
            {"Metallic_Copper",(300,ColorTranslator.FromHtml("#C27F53"))},
            {"Metallic_Light_Blue",(10045, ColorTranslator.FromHtml("#97CBD9"))},
            {"Metallic_Pink",(10046,ColorTranslator.FromHtml("#AD659A"))},
            {"Metallic_Light_Pink",(10049,ColorTranslator.FromHtml("#FECCCF"))},
            {"Milky_White",(79,ColorTranslator.FromHtml("#EEEEEE"))},
            {"Glow_In_Dark_Opaque",(21,ColorTranslator.FromHtml("#E0FFB0"))},
            {"Glow_In_Dark_Trans",(294,ColorTranslator.FromHtml("#BDC6AD"))},
            {"Glow_In_Dark_White",(329,ColorTranslator.FromHtml("#F5F3D7"))},
            {"Glitter_Trans_Dark_Pink",(114,ColorTranslator.FromHtml("#DF6695"))},
            {"Glitter_Trans_Clear",(117,ColorTranslator.FromHtml("#EEEEEE"))},
            {"Glitter_Trans_Purple",(129,ColorTranslator.FromHtml("#640061"))},
            {"Glitter_Trans_Light_Blue",(302,ColorTranslator.FromHtml("#AEE9EF"))},
            {"Glitter_Trans_Neon_Green",(339,ColorTranslator.FromHtml("#C0FF00"))},
            {"Glitter_Trans_Orange",(341,ColorTranslator.FromHtml("#F08F1C"))}
        };

        public static (int id, Color c) Get(string color)
        {
            return colorMap[color];
        }

        // distance in RGB space
        private static double ColorDiff(Color c1, Color c2)
        {
            return Math.Sqrt((c1.R - c2.R) * (c1.R - c2.R)
                                 + (c1.G - c2.G) * (c1.G - c2.G)
                                 + (c1.B - c2.B) * (c1.B - c2.B));
        }

        public static (int id, Color c) Closest(Color target)
        {
            return colorMap.TakeWhile(kvp => !kvp.Key.StartsWith("Chrome")).MinBy(kvp => ColorDiff(kvp.Value.c,target)).Value;

        }

    }
}
