using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.SpecExplorer
{
    internal enum TaskTypes
    {
        Exploring,
        GeneratingTestCode,
        RunningPostProcessors,
        OnTheFlyTesting,
        OnTheFlyReplayTest,
    }
}
