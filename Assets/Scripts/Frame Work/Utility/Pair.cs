using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrameWork.Utility
{
    public class Pair<KT, OT>
    {
        public KT first { get; set; }

        public OT second { get; set; }

        public Pair(KT first, OT second)
        {
            this.first = first;
            this.second = second;
        }
    }
}
