using UnityEngine;
using UnityEngine.UI;

namespace Panels.Exercise_Prompt_Panel
{
    /// <summary>
    /// Prompt panel that just displays some text
    /// </summary>
    [RequireComponent(typeof(LayoutElement))]
    public class TextPromptPanel : MonoBehaviour
    {
        /// <summary>
        /// The text of the prompt
        /// </summary>
        [SerializeField, Tooltip("The text of the prompt")]
        private Text promptText;

        /// <summary>
        /// LayoutElement of the prompt panel, useful for resizing
        /// </summary>
        private LayoutElement _layoutElement;

        void Start()
        {
            _layoutElement = GetComponent<LayoutElement>();

            // TODO: dirty hack but seems necessary for resolution-independent layout
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.localScale = new Vector2(1f, 1f);
        }

        void Update()
        {
            // make sure the prompt panel is always large enough for the text
            // and the 5px border over and under it
            _layoutElement.minHeight = Mathf.Abs(promptText.preferredHeight) + 10f;
        }
    }
}
