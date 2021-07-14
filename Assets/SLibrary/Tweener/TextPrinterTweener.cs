using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SLibrary.Tweener
{
    public class TextPrinterTweener : BaseTweener
    {
        [Header("文本")]
        public string TextContent;

        private Text _text;
        private TextMeshProUGUI _textMesh;
        private int _totalCount;
        
        protected override void Init()
        {
            base.Init();
            _text = Target.GetComponent<Text>();
            _textMesh = Target.GetComponent<TextMeshProUGUI>();
            _totalCount = TextContent.Length;
        }

#if USE_DOTWEEN
        protected override void ProgressSetter(float pnewvalue)
        {
            int len = (int) (_totalCount * pnewvalue);
            string text = TextContent.Substring(0, len);
            if (_text != null)
            {
                _text.text = text;
            }
            else if (_textMesh != null)
            {
                _textMesh.text = text;
            }
        }
#endif
    }
}