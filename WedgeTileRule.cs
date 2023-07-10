﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrunnianLink
{
    public class WedgeTileRule : SubstitutionRule
    {
        static readonly string[] m_colors = new string[] { "Lime", "Medium_Azure", "Tan", "Dark_Blue" };

        public override string[] Colors => m_colors;

        public override int StateCount => throw new NotImplementedException();

        public override double ScaleFactor => 2;

        public override double InitialScale => 1;

        public override string MainName => "WedgeTileTiling";

        public override string BasePart(int state, int color) => state == 0 ? "29119" : "29120";

        public override bool ColorByState => false;
        public override bool Level0IsComposite => false;

        List<(double x, double y, double rotation, int state)> m_rule = new()
        { (-0.5,0,0,0),
          (0.5,-1,0,0),
          (0,1.5,90,0),
          (0.5,1,180,1)
        };

        public override List<(double x, double y, double rotation, int state)> Rule(int state) => state==0? m_rule : m_rule.Select(t=>(-t.x,t.y,-t.rotation,1-t.state)).ToList();
      
    }
}
