using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Stage/Stage Data Catalog")]
public class StageDataCatalog : ScriptableObject
{
    [SerializeField]
    private StageData[] stages;

    public IReadOnlyList<StageData> Stages => stages;
}