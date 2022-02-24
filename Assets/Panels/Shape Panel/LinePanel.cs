using Dialogs.Angle_From_Line_Dialog;
using Dialogs.Rename_Line_Dialog;
using Math_View;
using UnityEngine;
using UnityEngine.UI;

namespace Panels.Shape_Panel
{
    /// <summary>
    /// The control panel for a line
    /// </summary>
    [RequireComponent(typeof(LayoutElement))]
    public class LinePanel : MonoBehaviour
    {
        /// <summary>
        /// The line object this panel belongs to
        /// </summary>
        public Line Line { get; set; }
        /// <summary>
        /// Contracted height of this panel in pixels
        /// </summary>
        [SerializeField, Tooltip("Contracted height of this panel in pixels")]
        private float contractedHeight;
        /// <summary>
        /// Expanded height of this panel in pixels
        /// </summary>
        [SerializeField, Tooltip("Expanded height of this panel in pixels")]
        private float expandedHeight;


        /// <summary>
        /// Transform of the name Text
        /// </summary>
        [SerializeField, Tooltip("Transform of the name Text")]
        private Transform nameTextTransform;
        /// <summary>
        /// Transform of the formula Text
        /// </summary>
        [SerializeField, Tooltip("Transform of the formula Text")]
        private Transform formulaTextTransform;
        /// <summary>
        /// Transform of the color swatch
        /// </summary>
        [SerializeField, Tooltip("Transform of the color swatch")]
        private Transform colorSwatchTransform;
        /// <summary>
        /// Transform of the length Text
        /// </summary>
        [SerializeField, Tooltip("Transform of the length Text")]
        private Transform lengthTextTransform;
        /// <summary>
        /// Transform of the expand container
        /// </summary>
        [SerializeField, Tooltip("Transform of the expand container")]
        private Transform expandContainerTransform;
        /// <summary>
        /// Transform of the input field for parameter M
        /// </summary>
        [SerializeField, Tooltip("Transform of the input field for parameter M")]
        private Transform parameterMInputFieldTransform;
        /// <summary>
        /// Transform of the input field for parameter B
        /// </summary>
        [SerializeField,Tooltip("Transform of the input field for parameter B")]
        private Transform parameterBInputFieldTransform;
        /// <summary>
        /// Prefab for the dialog to rename a line
        /// </summary>
        [SerializeField, Tooltip("Prefab for the dialog to rename a line")]
        private GameObject renameLineDialogPrefab;
        /// <summary>
        /// Prefab for the dialog to create an angle by entering a value
        /// </summary>
        [SerializeField, Tooltip("Prefab for the dialog to create an angle by entering a value")]
        private GameObject angleFromLineDialogPrefab;
        /// <summary>
        /// Game Object of the angle button
        /// </summary>
        [SerializeField] 
        private GameObject angleButton;
        /// <summary>
        /// Game Object of the delete button
        /// </summary>
        [SerializeField]
        private GameObject deleteButton;
        /// <summary>
        /// Game Object of the expand button
        /// </summary>
        [SerializeField]
        private GameObject expandButton;
        /// <summary>
        /// Game Object of the infinite toggle
        /// </summary>
        [SerializeField]
        private GameObject infiniteCheckbox;
        /// <summary>
        /// Game Object of the snap to grid toggle
        /// </summary>
        [SerializeField]
        private GameObject snapToGridCheckbox;

        /// <summary>
        /// Container in which dialogs are displayed
        /// </summary>
        private GameObject _dialogContainer;
        /// <summary>
        /// LayoutElement of this panel, useful for resizing
        /// </summary>
        private LayoutElement _layoutElement;
        /// <summary>
        /// Container where other shape panels are stored
        /// </summary>
        private ShapePanelContainer _shapePanelContainer;
        /// <summary>
        /// Text object displaying the line's name
        /// </summary>
        private Text nameText;
        /// <summary>
        /// Text object displaying the line's formula
        /// </summary>
        private Text formulaText;
        /// <summary>
        /// Text object displaying the line's length
        /// </summary>
        private Text lengthText;
        /// <summary>
        /// The line's color swatch image
        /// </summary>
        private Image _colorSwatchImage;
        /// <summary>
        /// Input field for parameter M
        /// </summary>
        private InputField _parameterMInputField;
        /// <summary>
        /// Input field for parameter B
        /// </summary>
        private InputField _parameterBInputField;
        /// <summary>
        /// Whether or not this panel is expanded
        /// </summary>
        private bool _isExpanded;

        /// <summary>
        /// Determines which way the line's formula is displayed
        /// </summary>
        enum DetailDisplayMode
        {
            /// <summary>
            /// y = mx + b
            /// </summary>
            Formula,
            /// <summary>
            /// m = ?, b = ?
            /// </summary>
            Parameters,
            /// <summary>
            /// Coordinates of the line's handles
            /// </summary>
            Points
        }

        private DetailDisplayMode _detailDisplayMode;

        void Start()
        {
            var parent = transform.parent;
            _shapePanelContainer = parent.GetComponent<ShapePanelContainer>();
            nameText = nameTextTransform.GetComponent<Text>();
            formulaText = formulaTextTransform.GetComponent<Text>();
            lengthText = lengthTextTransform.GetComponent<Text>();
            _parameterMInputField = parameterMInputFieldTransform.GetComponent<InputField>();
            _parameterBInputField = parameterBInputFieldTransform.GetComponent<InputField>();
		
            _dialogContainer = GameObject.Find("Dialog Container");
		
            _detailDisplayMode = DetailDisplayMode.Formula;

            // TODO: dirty hack but seems necessary for resolution-independent layout
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.localScale = new Vector2(1f, 1f);

            _layoutElement = GetComponent<LayoutElement>();
            _layoutElement.minHeight = contractedHeight;
            expandContainerTransform.gameObject.SetActive(false);
            _isExpanded = false;

            angleButton.GetComponent<Button>().interactable = !Line.Lock.AngleLocked;
            deleteButton.GetComponent<Button>().interactable = !Line.Lock.DeleteLocked;
            expandButton.GetComponent<Button>().interactable = !Line.Lock.ExpandLocked;
            infiniteCheckbox.GetComponent<Toggle>().interactable = !Line.Lock.InfiniteLocked;
            snapToGridCheckbox.GetComponent<Toggle>().interactable = !Line.Lock.SnapLocked;

            colorSwatchTransform.gameObject.GetComponent<Button>().interactable = !Line.Lock.ColorLocked;
            nameTextTransform.gameObject.GetComponent<Button>().interactable = !Line.Lock.NameLocked;
            _parameterMInputField.interactable = !Line.Lock.MLocked;
            _parameterBInputField.interactable = !Line.Lock.BLocked;
        }

        void Update()
        {
            //lockOverlayTransform.gameObject.SetActive(Line.Locked);
            nameText.text = Line.Name;
            formulaText.text = GetFormulaString(Line.FormulaValues.x, Line.FormulaValues.y);

            if (Line.Lock.LengthLocked)
            {
                lengthText.text = "?";
            }
            else
            {
                lengthText.text = Line.Infinite ? "Unendlich" : Line.Length.ToString("0.##");
            }

            if (!_parameterMInputField.isFocused)
            {
                _parameterMInputField.text = Line.Lock.MLocked ? "?" : Line.FormulaValues.x.ToString("0.##");
            }

            if (!_parameterBInputField.isFocused)
            {
                _parameterBInputField.text = Line.Lock.BLocked ? "?" : Line.FormulaValues.y.ToString("0.##");
            }
        }

        /// <summary>
        /// Returns the formula string of the line depending on the current detail display mode.
        /// The parameters come from the form y = mx + b.
        /// </summary>
        /// <param name="m">The slope factor of the line</param>
        /// <param name="b">The vertical offset of the line</param>
        /// <returns>The formula string of the line, depending on the current detail display mode</returns>
        private string GetFormulaString(float m, float b)
        {
            var isVertical = Mathf.Abs(Line.PointAHandle.x - Line.PointBHandle.x) <= Mathf.Epsilon;
            var output = "";

            switch (_detailDisplayMode)
            {
                case DetailDisplayMode.Formula when Line.Lock.FormulaLocked:
                    output += "?";
                    break;
                case DetailDisplayMode.Formula:
                    output += "y = " + (isVertical ? "Infinite " : m.ToString("0.##")) + "x ";
                    output += ((b < 0f) ? "- " : "+ ");
                    output += Mathf.Abs(b).ToString("0.##");
                    break;
                case DetailDisplayMode.Parameters when Line.Lock.ParametersLocked:
                    output += "?";
                    break;
                case DetailDisplayMode.Parameters:
                    output = "m = " + m.ToString("0.##") + " b = " + b.ToString("0.##");
                    break;
                case DetailDisplayMode.Points when Line.Lock.PointsLocked:
                    output += "?";
                    break;
                case DetailDisplayMode.Points:
                    output += "P1(" + Line.PointAHandle.x.ToString("0.##") + ", " + Line.PointAHandle.y.ToString("0.##") + ") ";
                    output += "P2(" + Line.PointBHandle.x.ToString("0.##") + ", " + Line.PointBHandle.y.ToString("0.##") + ")";
                    break;
            }

            return output;
        }
        /// <summary>
        /// Updates the line panel's color swatch
        /// </summary>
        public void UpdateColorSwatch()
        {
            if (!_colorSwatchImage)
            {
                _colorSwatchImage = colorSwatchTransform.GetComponent<Image>();
            }

            _colorSwatchImage.color = Line.Color;
        }

        /// <summary>
        /// Called when the color swatch is clicked, randomizes the line's color
        /// </summary>
        public void ColorSwatchButtonClicked()
        {
            Line.RandomizeColor();
            UpdateColorSwatch();
        }
        /// <summary>
        /// Called when the name button is clicked, opens the name dialog
        /// </summary>
        public void NameButtonClicked()
        {
            var renameLineDialogGameObject = Instantiate(renameLineDialogPrefab, _dialogContainer.transform);
            var renameLineDialogRectTransform = renameLineDialogGameObject.GetComponent<RectTransform>();
            renameLineDialogRectTransform.offsetMin = new Vector2(0f, 0f);
            renameLineDialogRectTransform.offsetMax = new Vector2(0f, 0f);
            var renameLineDialog = renameLineDialogGameObject.GetComponent<RenameLineDialog>();
            renameLineDialog.SetInitialValues(Line, Line.Name);
        }
        /// <summary>
        /// Called when the formula button is clicked, cycles through the available detail display modes
        /// </summary>
        public void FormulaButtonClicked()
        {
            _detailDisplayMode++;

            if (_detailDisplayMode > DetailDisplayMode.Points)
            {
                _detailDisplayMode = DetailDisplayMode.Formula;
            }
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
        /// Called when the delete button is clicked, deletes all angle panels this line is involved in and then removes and deletes its own line panel
        /// </summary>
        public void DeleteButtonClicked()
        {
            var anglePanels = _shapePanelContainer.GetComponentsInChildren<AnglePanel>();
            foreach(var panel in anglePanels)
            {
                if (panel.Angle.LineA.Equals(Line) || panel.Angle.LineB.Equals(Line))
                {
                    panel.DeleteButtonClicked();
                }
            }
            _shapePanelContainer.RemovePanel(gameObject);
        }
        /// <summary>
        /// Called when the angle button is clicked, opens the angle from line dialog
        /// </summary>
        public void AngleButtonClicked()
        {
            var angleFromLineDialogGameObject = Instantiate(angleFromLineDialogPrefab, _dialogContainer.transform);
            var angleFromLineDialogRectTransform = angleFromLineDialogGameObject.GetComponent<RectTransform>();
            angleFromLineDialogRectTransform.offsetMin = new Vector2(0f, 0f);
            angleFromLineDialogRectTransform.offsetMax = new Vector2(0f, 0f);
            var angleFromLineDialog = angleFromLineDialogGameObject.GetComponent<AngleFromLineDialog>();
            angleFromLineDialog.SetInitialValues(Line);
        }
        /// <summary>
        /// Called when the value in the parameter M input field changes
        /// </summary>
        /// <param name="value">The value the input field changed to</param>
        public void ParameterMInputFieldChanged(string value)
        {
            var m = float.Parse(value);
            Line.FormulaValues = new Vector2(m, Line.FormulaValues.y);
        }
        /// <summary>
        /// Called when the value in the parameter B input field changes
        /// </summary>
        /// <param name="value">The value the input field changed to</param>
        public void ParameterBInputFieldChanged(string value)
        {
            var b = float.Parse(value);
            Line.FormulaValues = new Vector2(Line.FormulaValues.x, b);
        }
        /// <summary>
        /// Called when the value in the snap to grid toggle changes
        /// </summary>
        /// <param name="value">The value the toggle changed to</param>
        public void SnapToGridToggleChanged(bool value)
        {
            Line.SnapToGrid = value;
        }
        /// <summary>
        /// Called when the value in the infinite toggle changes
        /// </summary>
        /// <param name="value">The value the toggle changed to</param>
        public void InfiniteToggleChanged(bool value)
        {
            Line.Infinite = value;
        }
    }
}
