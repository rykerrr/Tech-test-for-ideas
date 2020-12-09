using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649
public class AIGameSceneSettings : InheritableSingleton<AIGameSceneSettings>
{
    [SerializeField] private int historyDepth;

    public int HistoryDepth => historyDepth;
}
#pragma warning restore 0649