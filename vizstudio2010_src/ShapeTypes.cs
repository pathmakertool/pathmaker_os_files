using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathMaker {
    /**
     * Enum for use as ShapeTypes.  The numeric values of these are used to store in the shapes shape type cells.
     */
    public enum ShapeTypes {
         None=0,
         DocTitle=1,
         ChangeLog=2,
         Start=3,
         Interaction=4,
         Play=5,
         Decision=6,
         Data=7,
         Transfer=8,
         HangUp=9,
         SubDialog=10,
         Return=11,
         CallSubDialog=12,
         Placeholder=13,
         OffPageRef=14,
         Connector=15,
         Comment=16,
         OnPageRefIn=17,
         OnPageRefOut=18,
         Page=19
    };
}
