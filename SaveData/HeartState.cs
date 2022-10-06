using System;
using System.Collections.Generic;
using System.Text;

namespace RedHeartsFirst
{
    public enum HeartOrder
    {
        Off, // Same as BlackBlueRed, which is the default
        BlackRedBlue,
        RedBlackBlue,
        BlueRedBlack,

        /* I'm not including: 
         * RedBlueBlack,
         * BlueBlackRed,
         * ... Because why would anyone want those?? */
    }
}
