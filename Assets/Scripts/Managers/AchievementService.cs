using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface IAchievementService
{
    IReadOnlyList<AchievementData> Achievements { get; }


}


public sealed class AchievementService { }





