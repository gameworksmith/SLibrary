using UnityEngine;

namespace SLibrary.Extension
{
    public static class EngineBaseExtension
    {
        public static Color GetColorFromHex(string hexStr)
        {
            Color ret = Color.white;
            if (ColorUtility.TryParseHtmlString(hexStr, out ret))
            {
                return ret;
            }
            return Color.white;
        }

        public static void DestroyAllChildren(this Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(parent.GetChild(i).gameObject);
            }
        }
        public static void DestroyAllChildren(this GameObject go)
        {
            var parent = go.transform;
            
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(parent.GetChild(i).gameObject);
            }
        }


        
        public static void SetAllChildrenToLayer(this GameObject go, string layerName)
        {
            int layer = LayerMask.NameToLayer(layerName);
            var transforms = go.GetComponentsInChildren<Transform>(true);
            foreach (Transform transform in transforms)
            {
                transform.gameObject.layer = layer;
            }

        }
    }
}