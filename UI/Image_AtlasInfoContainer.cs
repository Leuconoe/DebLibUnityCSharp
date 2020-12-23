using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;

#endif
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Image_AtlasInfoContainer : MonoBehaviour
{ 
    [SerializeField] public string m_AtlasName;
    [SerializeField] public string m_spriteName;

#if UNITY_EDITOR
    [ExecuteInEditMode]
    private void Reset()
    {
        if (this.GetComponent<Image>().sprite == null)
        {
            return;
        }
        m_spriteName = this.GetComponent<Image>().sprite.name;
        //Debug.Log(m_Image.sprite.texture);
        string[] guids = AssetDatabase.FindAssets("", new string[] { "Assets/Game/Resources1/SpriteAtlas" });
        for (int i = 0; i < guids.Length; i++)
        {
            string guid = guids[i];
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            SpriteAtlas sa = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(assetPath);
            if (sa != null)
            {
                Sprite sp = sa.GetSprite(m_spriteName);
                if (sp != null && sp.name.Contains(m_spriteName))
                {
                    if (string.IsNullOrEmpty(m_AtlasName))
                    {
                        m_AtlasName = sa.name;
                    }
                    else
                    {
                        Debug.Log(m_spriteName + " conflict Atlas Name : " + m_AtlasName + " , " + sa.name);
                    }

                    //return;
                }
            }
        }
    }
#endif

    // Start is called before the first frame update
    void Start()
    {
		return;
        if (!string.IsNullOrEmpty(m_AtlasName))
        {
            //this.GetComponent<Image>().sprite = ResourcesLoader.LoadSprite(m_AtlasName, m_spriteName);
        }
    }
}
