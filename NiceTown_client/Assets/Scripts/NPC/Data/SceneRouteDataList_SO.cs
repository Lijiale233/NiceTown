using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SceneRouteDataList_SO", menuName = "NPC Schedule/SceneRouteDataList")]
public class SceneRouteDataList_SO : ScriptableObject
{
    public List<SceneRoute> sceneRouteList;
}