using Dialogs.Rename_Parabola_Dialog;
using Math_View;
using UnityEngine;
using UnityEngine.UI;

namespace Panels.Shape_Panel
{
    /// <summary>
    /// The control panel for a parabola
    /// </summary>
    [RequireComponent(typeof(LayoutElement))]
    public class ParabolaPanel : MonoBehaviour
    {
        /// <summary>
        /// The parabola object that belongs to this panel
        /// </summary>
        public Parabola Parabola { get; set; }
        /// <summary>
        /// Height of the Parabola Panel when contracted, in pixels
        /// </summary>
        [SerializeField, Tooltip("Height of the Parabola Panel when contracted, in pixels")]
        private float contractedHeight;
        /// <summary>
        /// Height of the Parabola Panel when expanded, in pixels
        /// </summary>
        [SerializeField, Tooltip("Height of the Parabola Panel when expanded, in pixels")]
        private float expandedHeight;
        /// <summary>
        /// Transform of the name text
        /// </summary>
        [SerializeField, Tooltip("Transform of the name text")]
        private Transform nameTextTransform;
        /// <summary>
        /// Transform of the formula text
        /// </summary>
        [SerializeField, Tooltip("Transform of the formula text")]
        private Transform formulaTextTransform;
        /// <summary>
        /// Transform of the color swatch
        /// </summary>
        [SerializeField, Tooltip("Transform of the color swatch")]
        private Transform colorSwatchTransform;
        /// <summary>
        /// Transform of the expand container
        /// </summary>
        [SerializeField, Tooltip("Transform of the expand container")]
        private Transform expandContainerTransform;
        /// <summary>
        /// Transform of the input field for parameter A"
        /// </summary>
        [SerializeField, Tooltip("Transform of the input field for parameter A")] 
        private Transform parameterAInputFieldTransform;
        /// <summary>
        /// Transform of the input field for parameter B
        /// </summary>
        [SerializeField, Tooltip("Transform of the input field for parameter B")]
        private Transform parameterBInputFieldTransform;
        /// <summary>
        /// Transform of the input field for parameter C
        /// </summary>
        [SerializeField, Tooltip("Transform of the input field for parameter C")]
        private Transform parameterCInputFieldTransform;
        /// <summary>
        /// Prefab for a dialog to rename the parabola
        /// </summary>
        [SerializeField, Tooltip("Prefab for a dialog to rename the parabola")]
        private GameObject renameParabolaDialogPrefab;
        /// <summary>
        /// Game object of the expand button, used for locking it
        /// </summary>
        [SerializeField, Tooltip("Game object of the expand button, used for locking it")] 
        private GameObject expandButtonGameObject;
        /// <summary>
        /// Game object of the delete button, used for locking it
        /// </summary>
        [SerializeField, Tooltip("Game object of the delete button, used for locking it")]
        private GameObject deleteButtonGameObject;
        /// <summary>
        /// Game object of the snap toggle, used for locking it
        /// </summary>
        [SerializeField, Tooltip("Game object of the snap toggle, used for locking it")]
        private GameObject snapToggleGameObject;
        /// <summary>
        /// Game object of the infinite toggle, used for locking it
        /// </summary>
        [SerializeField, Tooltip("Game object of the infinite toggle, used for locking it")]
        private GameObject infiniteToggleGameObject;
        /// <summary>
        /// Container where dialogs are displayed
        /// </summary>
        private GameObject _dialogContainer;
        /// <summary>
        /// LayoutElement of the panel, useful for resizing
        /// </summary>
        private LayoutElement _layoutElement;
        /// <summary>
        /// Container where shapes are displayed
        /// </summary>
        private ShapePanelContainer _shapePanelContainer;
        /// <summary>
        /// Text object that displays the parabola's name
        /// </summary>
        private Text _nameText;
        /// <summary>
        /// Text object that displays the parabola's formula
        /// </summary>
        private Text _formulaText;
        /// <summary>
        /// Color swatch for the color of the parabola
        /// </summary>
        private Image _colorSwatchImage;

        /// <summary>
        /// Input field for parameter A
        /// </summary>
        private InputField _parameterAInputField;
        /// <summary>
        /// Input field for parameter B
        /// </summary>
        private InputField _parameterBInputField;
        /// <summary>
        /// Input field for parameter C
        /// </summary>
        private InputField _parameterCInputField;
        /// <summary>
        /// Whether or not this panel is expanded
        /// </summary>
        private bool _isExpanded;

        /// <summary>
        /// Sets how the formula of the parabola will be displayed.
        /// </summary>
        enum DetailDisplayMode
        {
            /// <summary>
            /// y = ax² + bx + c
            /// </summary>
            StandardForm,
            /// <summary>
            /// y = a * (x - b)² + c
            /// </summary>
            VertexForm,
            /// <summary>
            /// Displays values a, b, and c
            /// </summary>
            Parameters,
            /// <summary>
            /// Displays coordinates of the origin handle and curve handle
            /// </summary>
            Points
        }

        private DetailDisplayMode _detailDisplayMode;

        void Start()
        {
            var parent = transform.parent;
            _shapePanelContainer = parent.GetComponent<ShapePanelContainer>();
            _nameText = nameTextTransform.GetComponent<Text>();
            _formulaText = formulaTextTransform.GetComponent<Text>();
            _parameterAInputField = parameterAInputFieldTransform.GetComponent<InputField>();
            _parameterBInputField = parameterBInputFieldTransform.GetComponent<InputField>();
            _parameterCInputField = parameterCInputFieldTransform.GetComponent<InputField>();
		
            _dialogContainer = GameObject.Find("Dialog Container");
		
            _detailDisplayMode = DetailDisplayMode.StandardForm;

            // TODO: dirty hack but seems necessary for resolution-independent layout
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.localScale = new Vector2(1f, 1f);

            _layoutElement = GetComponent<LayoutElement>();
            _layoutElement.minHeight = contractedHeight;
            expandContainerTransform.gameObject.SetActive(false);
            _isExpanded = false;

            deleteButtonGameObject.GetComponent<Button>().interactable = !Parabola.Lock.DeleteLocked;
            expandButtonGameObject.GetComponent<Button>().interactable = !Parabola.Lock.ExpandLocked;
            snapToggleGameObject.GetComponent<Toggle>().interactable = !Parabola.Lock.SnapLocked;
            infiniteToggleGameObject.GetComponent<Toggle>().interactable = !Parabola.Lock.InfiniteLocked;

            _nameText.gameObject.GetComponent<Button>().interactable = !Parabola.Lock.NameLocked;
            _parameterAInputField.interactable = !Parabola.Lock.ALocked;
            _parameterBInputField.interactable = !Parabola.Lock.BLocked;
            _parameterCInputField.interactable = !Parabola.Lock.CLocked;
        }

        void Update()
        {
            //lockOverlayTransform.gameObject.SetActive(Parabola.Locked);
            _nameText.text = Parabola.Name;
            _formulaText.text = GetFormulaString(Parabola.FormulaValues.x, Parabola.FormulaValues.y, Parabola.FormulaValues.z);

            if (!_parameterAInputField.isFocused)
            {
                _parameterAInputField.text = Parabola.FormulaValues.x.ToString("0.##");
            }

            if (!_parameterBInputField.isFocused)
            {
                _parameterBInputField.text = Parabola.FormulaValues.y.ToString("0.##");
            }

            if (!_parameterCInputField.isFocused)
            {
                _parameterCInputField.text = Parabola.FormulaValues.z.ToString("0.##");
            }
        }
        /// <summary>
        /// Returns the formula string of the parabola depending on the current detail display mode.
        /// The parameters come from the form y = ax² + bx + c.
        /// </summary>
        /// <param name="a">y = Ax² + bx + c</param>
        /// <param name="b">y = ax² + Bx + c</param>
        /// <param name="c">y = ax² + bx + C</param>
        /// <returns>The formula string of the parabola, depending on the current detail display mode</returns>
        private string GetFormulaString(float a, float b, float c)
        {
            var output = "";

            switch (_detailDisplayMode)
            {
                case DetailDisplayMode.StandardForm when Parabola.Lock.StandardLocked:
                    output += "?";
                    break;
                case DetailDisplayMode.StandardForm:
                    output += "y = " + (a.ToString("0.##")) + "x²";
                    output += ((b < 0f) ? " - " : " + ");
                    output += Mathf.Abs(b).ToString("0.##") + "x";
                    output += ((c < 0f) ? " - " : " + ");
                    output += Mathf.Abs(c).ToString("0.##");
                    break;
                case DetailDisplayMode.VertexForm when Parabola.Lock.VertexLocked:
                    output += "?";
                    break;
                case DetailDisplayMode.VertexForm:
                    output += "y = " + (a.ToString("0.##")) + " * (x";
                    output += ((Parabola.OriginHandle.x < 0f) ? " + " : " - ");
                    output += Mathf.Abs(Parabola.OriginHandle.x).ToString("0.##") + ")²";
                    output += ((Parabola.OriginHandle.y < 0f) ? " - " : " + ");
                    output += Mathf.Abs(Parabola.OriginHandle.y).ToString("0.##");
                    break;
                case DetailDisplayMode.Parameters when Parabola.Lock.ParametersLocked:
                    output += "?";
                    break;
                case DetailDisplayMode.Parameters:
                    output += ("a = " + a.ToString("0.##") + " b = " + b.ToString("0.##") + " c = " + c.ToString("0.##"));
                    break;
                case DetailDisplayMode.Points when Parabola.Lock.PointsLocked:
                    output += "?";
                    break;
                case DetailDisplayMode.Points:
                    output += ("S(" + Parabola.OriginHandle.x.ToString("0.##") + ", " + Parabola.OriginHandle.y.ToString("0.##") + ") ");
                    output += ("P(" + Parabola.CurveHandle.x.ToString("0.##") + ", " + Parabola.CurveHandle.y.ToString("0.##") + ")");
                    break;
            }

            return output;
        }
        /// <summary>
        /// Updates the parabola panel's color swatch
        /// </summary>
        public void UpdateColorSwatch()
        {
            if (!_colorSwatchImage)
            {
                _colorSwatchImage = colorSwatchTransform.GetComponent<Image>();
            }

            _colorSwatchImage.color = Parabola.Color;
        }
        /// <summary>
        /// Called when the color swatch is clicked, randomizes the parabola's color
        /// </summary>
        public void ColorSwatchButtonClicked()
        {
            Parabola.RandomizeColor();
            UpdateColorSwatch();
        }
        /// <summary>
        /// Called when the name button is clicked, opens the name dialog
        /// </summary>
        public void NameButtonClicked()
        {
            var renameParabolaDialogGameObject = Instantiate(renameParabolaDialogPrefab, _dialogContainer.transform);
            var renameParabolaDialogRectTransform = renameParabolaDialogGameObject.GetComponent<RectTransform>();
            renameParabolaDialogRectTransform.offsetMin = new Vector2(0f, 0f);
            renameParabolaDialogRectTransform.offsetMax = new Vector2(0f, 0f);
            var renameParabolaDialog = renameParabolaDialogGameObject.GetComponent<RenameParabolaDialog>();
            renameParabolaDialog.SetInitialValues(Parabola, Parabola.Name);
        }
        /// <summary>
        /// Called when the formula button is clicked, cycles through the available detail display modes
        /// </summary>
        public void FormulaButtonClicked()
        {
            _detailDisplayMode++;

            if (_detailDisplayMode > DetailDisplayMode.Points)
                _detailDisplayMode = DetailDisplayMode.StandardForm;
        }
        /// <summary>
        /// Called when the expand button is clicked, toggles whether or not the panel is expanded
        /// </summary>
        public void ExpandButtonClicked()
        {
            _isExpanded = !_isExpanded;

            _layoutElement.minHeight = _isExpanded ? expandedHeight : contractedHeight;

            expandContainerTransform.gameObject.SetActive(_isExpanded);
        }
        /// <summary>
        /// Called when the delete button is clicked, removes and deletes its own parabola panel
        /// </summary>
        public void DeleteButtonClicked()
        {
            _shapePanelContainer.RemovePanel(gameObject);
        }
        /// <summary>
        /// Called when the value in the parameter A input field changes
        /// </summary>
        /// <param name="value">The value the input field changed to</param>
        public void ParameterAInputFieldChanged(string value)
        {
            var a = float.Parse(value);
            Parabola.FormulaValues = new Vector3(a, Parabola.FormulaValues.y, Parabola.FormulaValues.z);
        }
        /// <summary>
        /// Called when the value in the parameter B input field changes
        /// </summary>
        /// <param name="value">The value the input field changed to</param>
        public void ParameterBInputFieldChanged(string value)
        {
            var b = float.Parse(value);
            Parabola.FormulaValues = new Vector3(Parabola.FormulaValues.x, b, Parabola.FormulaValues.z);
        }
        /// <summary>
        /// Called when the value in the parameter C input field changes
        /// </summary>
        /// <param name="value">The value the input field changed to</param>
        public void ParameterCInputFieldChanged(string value)
        {
            var c = float.Parse(value);
            Parabola.FormulaValues = new Vector3(Parabola.FormulaValues.x, Parabola.FormulaValues.y, c);
        }
        /// <summary>
        /// Called when the value in the snap to grid toggle changes
        /// </summary>
        /// <param name="value">The value the toggle changed to</param>
        public void SnapToGridToggleChanged(bool value)
        {
            Parabola.SnapToGrid = value;
        }
        /// <summary>
        /// Called when the value in the infinite toggle changes
        /// </summary>
        /// <param name="value">The value the toggle changed to</param>
        public void InfiniteToggleChanged(bool value)
        {
            Parabola.Infinite = value;
        }
    }
}
