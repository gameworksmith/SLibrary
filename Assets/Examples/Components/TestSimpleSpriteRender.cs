using System;
using SLibrary.Components;
using UnityEngine;

namespace Examples.Components
{
    public class TestSimpleSpriteRender : MonoBehaviour
    {
        public float Width;
        public float Height;

        public SimpleSliceSpriteRender Render;

        private void OnGUI()
        {
            if (GUI.Button(new Rect(0, 0, 120, 40), "同步x"))
            {
                Render.Width = Width;
            }
            
            if (GUI.Button(new Rect(0, 50, 120, 40), "同步y"))
            {
                Render.Height = Height;
            }
            
            if (GUI.Button(new Rect(0, 90, 120, 40), "同步xy"))
            {
                Render.SetSize(Width, Height);
            }
        }
    }
}