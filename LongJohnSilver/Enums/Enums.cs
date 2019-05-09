using System;
using System.Collections.Generic;
using System.Text;

namespace LongJohnSilver.Enums
{
    public enum KnockoutStatus
    {
        NoKnockout = 1,
        KnockoutInProgress = 2,
        KnockoutFinished = 3,
        KnockoutUnderConstruction = 4
    }

    public enum DraftStatus
    {
        NoDraft = 1,
        DraftInConstruction = 2,
        DraftInProgress = 3,
        DraftVoting = 4,
        DraftFinished = 5,
    }
}