﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manufaktura.Controls.Rendering
{
    public abstract class ScoreRendererBase
    {
        public ScoreRendererState State { get; private set; }
        protected ScoreRendererBase()
        {
            State = new ScoreRendererState();
        }
    }
}
