using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathMaker {
    interface ShadowForm {
        Shadow GetShadow();
        void RedoFormPromptIdsIfNecessary(string promptIdFormat);
    }

}
