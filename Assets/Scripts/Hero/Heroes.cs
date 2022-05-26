using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Hero")]
public class Heroes : ScriptableObject
{
    public string HeroName;
    public GameObject Hero;
    [TextArea(1, 10)] public string HeroDescription;

    public Sprite AbilitySprite;
    [TextArea(1, 10)] public string AbilityName;
    [TextArea(1, 10)] public string AbilityDescription;
}
