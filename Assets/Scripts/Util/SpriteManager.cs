using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    // Start is called before the first frame update
    [System.Serializable]
    class AspectSprite{
        [SerializeField] public AspectAlignment Aspect;
        [SerializeField] public Sprite Sprite;
    }

    [SerializeField] private List<AspectSprite> aspectSprites;
    [SerializeField] private  Sprite dummySprite;
    [SerializeField] private static Dictionary<AspectAlignment, Sprite> aspectSpritesDict = new();

    [SerializeField] private static Sprite notFoundSprite;


    private void Awake()
    {
        for (int i = 0; i < aspectSprites.Count; i++)
        {               
            aspectSpritesDict[aspectSprites[i].Aspect] = aspectSprites[i].Sprite;
        }
        notFoundSprite = dummySprite;
    }

    // Update is called once per frame

    public static Sprite GetSpriteByAspect(AspectAlignment aspect)
    {
        if (!aspectSpritesDict.TryGetValue(aspect, out Sprite sprite))
        {
            return notFoundSprite;
        }
        return sprite;
    }
}
