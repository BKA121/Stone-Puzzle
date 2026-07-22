using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchGroup 
{
    public MatchType matchType;
    public HashSet<Stone> matchGroup;
    

    public MatchGroup(IEnumerable<Stone> matchGroup, MatchType type)
    {
        this.matchGroup = new HashSet<Stone>(matchGroup);
        this.matchType = type;
    }
}
