using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Banner")]
public class BannerSO : ScriptableObject
{
    [Header("Banner")]
    public GameObject _bannerGO;

    [Header("Sprite")]
    public Sprite _bannerSprite;

}
