using System;
using System.Collections.Generic;
using System.Text;

public enum ModificationResult
 {
     IllegalCharacter,
     ValueIsEmpty,
     Duplicate,
     Missing,
     Success
 }

public enum KnockoutStatus
{
    NoKnockout = 1,
    KnockoutInProgress = 2,
    KnockoutFinished = 3,
    KnockoutUnderConstruction = 4
}