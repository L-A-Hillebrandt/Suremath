using UnityEngine;
using UnityEngine.UI;

namespace Panels.Exercise_Prompt_Panel
{
    /// <summary>
    /// Prompt panel that asks a single numeric value from the user
    /// </summary>
    [RequireComponent(typeof(LayoutElement))]
    public class ValuePromptPanel : MonoBehaviour
    {
        /// <summary>
        /// The text of the prompt
        /// </summary>
        [SerializeField, Tooltip("The text of the prompt")]
        private Text text;
        /// <summary>
        /// Input field for solution input
        /// </summary>
        [SerializeField, Tooltip("Input field for solution input")]
        private InputField valueInputField;
        /// <summary>
        /// Overlay that locks the inputs after validation
        /// </summary>
        [SerializeField, Tooltip("Overlay that locks the inputs after validation")]
        private RawImage validationOverlay;
        /// <summary>
        /// LayoutElement of the prompt panel, useful for resizing
        /// </summary>
        private LayoutElement _layoutElement;
        /// <summary>
        /// Correct solution of the prompt
        /// </summary>
        private float _solution;

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
            // and the input field underneath and the 5px border over, in the middle and under it
            _layoutElement.minHeight = Mathf.Abs(text.preferredHeight) + 24f + 15f;
        }

        /// <summary>
        /// Sets the initial values of the prompt panel, parsed from exercise data
        /// </summary>
        /// <param name="promptText">The text of the prompt</param>
        /// <param name="solution">The solution of the prompt</param>
        public void SetInitialValues(string promptText, float solution)
        {
            text.text = promptText;
            _solution = solution;

            validationOverlay.color = new Color(1f, 1f, 1f, 0f);
        }

        /// <summary>
        /// Decides whether or not the solution the user has given is correct and updates UI accordingly
        /// </summary>
        public void Validate()
        {
            if (Mathf.Abs(float.Parse(valueInputField.text) - _solution) <= Mathf.Epsilon)
            {
                validationOverlay.color = new Color(0f, 1f, 0f, 0.1f);
            }
            else
            {
                validationOverlay.color = new Color(1f, 0f, 0f, 0.1f);
            }
        }
    }
}
