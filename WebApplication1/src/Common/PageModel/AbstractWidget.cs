﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.PageModel
{
    public abstract class AbstractWidget : AbstractItem
    {
        public string ZoneName { get; set; }

        public override bool IsPage
        {
            get
            {
                return false;
            }
        }
    }
}
