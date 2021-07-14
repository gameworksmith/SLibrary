using UnityEngine;
using UnityEngine.UI;

namespace SLibrary.Components
{
    public class NoDrawGraphic : MaskableGraphic
    {
        public override void SetMaterialDirty()
        {
        }
 
        public override void SetVerticesDirty()
        {
        }
 
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }
    }
}