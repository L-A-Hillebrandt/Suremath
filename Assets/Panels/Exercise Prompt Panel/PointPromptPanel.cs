using UnityEngine;
using UnityEngine.UI;

namespace Panels.Exercise_Prompt_Panel
{
    /// <summary>
    /// Prompt panel that asks the coordinates of a point from the user
    /// </summary>
    [RequireComponent(typeof(LayoutElement))]
    public class PointPromptPanel : MonoBehaviour
    {
        /// <summary>
        /// Text object that displays the prompts text
        /// </summary>
        [SerializeField, Tooltip("Text object that displays the prompts text")]
        private Text text;
        /// <summary>
        /// Input field for the x coordinate of the solution
        /// </summary>
        [SerializeField, Tooltip("Input field for the x coordinate of the solution")] 
        private InputField xInputField;
        /// <summary>
        /// Input field for the y coordinate of the solution
        /// </summary>
        [SerializeField,Tooltip("Input field for the x coordinate of the solution")]
        private InputField yInputField;
        /// <summary>
        /// Overlay that locks the inputs after validation
        /// </summary>
        [SerializeField, Tooltip("Overlay that locks the inputs after validation")]
        private RawImage validationOverlay;
        /// <summary>
        /// The layout element of the prompt panel, used for resizing
        /// </summary>
        private LayoutElement _layoutElement;
        /// <summary>
        /// The correct solution of the prompt
        /// </summary>
        private Vector2 _solution;

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
        /// Sets the text and solution of the prompt, parsed from exercise data
        /// </summary>
        /// <param name="promptText">The text of the prompt</param>
        /// <param name="solution">The solution of the prompt</param>
        public void SetInitialValues(string promptText, Vector2 solution)
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
            var input = new Vector2(float.Parse(xInputField.text), float.Parse(yInputField.text));
            if (Vector2.Distance(input, _solution) <= Mathf.Epsilon)
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
