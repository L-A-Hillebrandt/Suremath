using System;
using Dialogs.Rename_Angle_Dialog;
using Math_View;
using UnityEngine;
using UnityEngine.UI;

namespace Panels.Shape_Panel
{
    /// <summary>
    /// The control panel for an angle
    /// </summary>
    [RequireComponent(typeof(LayoutElement))]
    public class AnglePanel : MonoBehaviour
    {
        /// <summary>
        /// The Angle object corresponding with this panel. Settable to be able to be externally initialized.
        /// </summary>
        public Angle Angle { get; set; }

        /// <summary>
        /// Utility for when this panel has just been created, to select two lines to create an angle from.
        /// </summary>
        public LineSelectionManager  LineSelectionManager { get; private set; }
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
        /// Transform of the angle Text
        /// </summary>
        [SerializeField, Tooltip("Transform of the angle Text")]
        private Transform angleTextTransform;
        /// <summary>
        /// Transform of the color swatch
        /// </summary>
        [SerializeField, Tooltip("Transform of the color swatch")]
        private Transform colorSwatchTransform;
        /// <summary>
        /// Transform of the sinus Text
        /// </summary>
        [SerializeField, Tooltip("Transform of the sinus Text")]
        private Transform sinTextTransform;
        /// <summary>
        /// Transform of the cosine Text
        /// </summary>
        [SerializeField, Tooltip("Transform of the cosine Text")]
        private Transform cosTextTransform;
        /// <summary>
        /// Transform of the tangens Text
        /// </summary>
        [SerializeField, Tooltip("Transform of the tangens Text")]
        private Transform tanTextTransform;
        /// <summary>
        /// Transform of the expand container
        /// </summary>
        [SerializeField, Tooltip("Transform of the expand container")]
        private Transform expandContainerTransform;
        /// <summary>
        /// Prefab for the dialog for renaming angles
        /// </summary>
        [SerializeField, Tooltip("Prefab for the dialog for renaming angles")]
        private GameObject renameAngleDialogPrefab;
        /// <summary>
        /// Game Object of the expand button
        /// </summary>
        [SerializeField, Tooltip("Game Object of the expand button")]
        private GameObject expandButtonGameObject;
        /// <summary>
        /// Game Object of the delete button
        /// </summary>
        [SerializeField, Tooltip("Game Object of the delete button")]
        private GameObject deleteButtonGameObject;
        /// <summary>
        /// Game Object of the adjacent button
        /// </summary>
        [SerializeField, Tooltip("Game Object of the adjacent button")]
        private GameObject adjacentButtonGameObject;

        /// <summary>
        /// Whether or not the selection mode is active
        /// </summary>
        public bool SelectionModeActive { get; set; }
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
        /// Text object that displays the angle's name
        /// </summary>
        private Text _nameText;
        /// <summary>
        /// Text object that displays the angle's value
        /// </summary>
        private Text _angleText;
        /// <summary>
        /// Text object that displays the angle's sine
        /// </summary>
        private Text _sinText;
        /// <summary>
        /// Text object that displays the angle's cosine
        /// </summary>
        private Text _cosText;
        /// <summary>
        /// Text object that displays the angle's tangens
        /// </summary>
        private Text _tanText;
        /// <summary>
        /// Color swatch for the color of the angle
        /// </summary>
        private Image _colorSwatchImage;
        /// <summary>
        /// Whether or not the panel is expanded
        /// </summary>
        private bool _isExpanded;

        // Use this for initialization
        void Start()
        {
            var parent = transform.parent;
            _shapePanelContainer = parent.GetComponent<ShapePanelContainer>();
            _nameText = nameTextTransform.GetComponent<Text>();
            _angleText = angleTextTransform.GetComponent<Text>();
            _sinText = sinTextTransform.GetComponent<Text>();
            _cosText = cosTextTransform.GetComponent<Text>();
            _tanText = tanTextTransform.GetComponent<Text>();


            _dialogContainer = GameObject.Find("Dialog Container");

            //initialize point selection mode
            LineSelectionManager ??= new LineSelectionManager(this);

            // TODO: dirty hack but seems necessary for resolution-independent layout
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.localScale = new Vector2(1f, 1f);

            _layoutElement = GetComponent<LayoutElement>();
            _layoutElement.minHeight = contractedHeight;
            expandContainerTransform.gameObject.SetActive(false);
            _isExpanded = false;

            expandButtonGameObject.GetComponent<Button>().interactable = !Angle.Lock.ExpandLocked;
            deleteButtonGameObject.GetComponent<Button>().interactable = !Angle.Lock.DeleteLocked;
            adjacentButtonGameObject.GetComponent<Button>().interactable = !Angle.Lock.AdjacentLocked;

            _nameText.gameObject.GetComponent<Button>().interactable = !Angle.Lock.NameLocked;
        }

        // Update is called once per frame
        void Update()
        {
            if (Angle != null)
            {
                SelectionModeActive = false;
                LineSelectionManager.AngleSelectionModeActive = false;
                Angle.CalculateAngle();
                Angle.CalculateVertex();
                _nameText.text = Angle.Name;

                if (Angle.Lock.SinLocked)
                {
                    _sinText.text = "?";
                }
                else
                {
                    _sinText.text = Math.Round(Math.Sin(Angle.AngleInRad), 2) + "";
                }

                if (Angle.Lock.CosLocked)
                {
                    _cosText.text = "?";
                }
                else
                {
                    _cosText.text = Math.Round(Math.Cos(Angle.AngleInRad), 2) + "";
                }
            

                if (Angle.Color == Color.black)
                {
                    Angle.RandomizeColor();
                }

                UpdateColorSwatch();

                var tangens = Math.Round(Math.Tan(Angle.AngleInRad), 2);

                if (Angle.Lock.TanLocked)
                {
                    _tanText.text = "?";
                }
                else
                {
                    if (90.0 - Math.Abs(Angle.AngleInDeg) <= double.Epsilon)
                    {
                        _tanText.text = "--";
                    }
                    else
                    {
                        Debug.Log(_tanText.text);
                        _tanText.text = $"{tangens}";
                    }
                }
            
                if (Angle.UseRadians)
                {
                    if (Angle.Lock.RadLocked)
                    {
                        _angleText.text = "? rad";
                    }
                    else
                    {
                        _angleText.text = Math.Round(Angle.AngleInRad, 2) + " rad";
                    }
                }
                else
                {
                    if (Angle.Lock.DegLocked)
                    {
                        _angleText.text = "? °";
                    }
                    else
                    {
                        _angleText.text = Math.Round(Angle.AngleInDeg, 2) + "°";
                    }
                }
            }
            else
            {
                SelectionModeActive = true;
                LineSelectionManager.AngleSelectionModeActive = true;
                _angleText.text = "2 PUNKTE AUSWÄHLEN!";
            }


        }
        /// <summary>
        /// Updates the contents of the color swatch
        /// </summary>
        public void UpdateColorSwatch()
        {
            if (!_colorSwatchImage)
            {
                _colorSwatchImage = colorSwatchTransform.GetComponent<Image>();
            }

            _colorSwatchImage.color = Angle.Color;
        }
        /// <summary>
        /// Function called when the color swatch is clicked, randomizes the color
        /// </summary>
        public void ColorSwatchButtonClicked()
        {
            Angle.RandomizeColor();
            UpdateColorSwatch();
        }
        /// <summary>
        /// Function called when the delete button is clicked. Removes the panel and destroys it.
        /// </summary>
        public void DeleteButtonClicked()
        {
            _shapePanelContainer.RemovePanel(gameObject);
        }

        /// <summary>
        /// Toggles the angle panel's expanded form on and off when the expand button is clicked
        /// </summary>
        public void ExpandButtonClicked()
        {
            _isExpanded = !_isExpanded;

            _layoutElement.minHeight = _isExpanded ? expandedHeight : contractedHeight;

            expandContainerTransform.gameObject.SetActive(_isExpanded);
        }

        /// <summary>
        /// Toggles between adjacent angles when the adjacent angle button is clicked
        /// </summary>
        public void AdjacentAngleButtonClicked()
        {
            Angle?.ToggleAdjacentAngle();
        }

        /// <summary>
        /// Toggles between radians and degrees when the radians button is clicked
        /// </summary>
        public void RadiansButtonClicked()
        {
            Angle?.ToggleRadians();
        }

        /// <summary>
        /// Spawns a RenameAngleDialog when the name button is clicked
        /// </summary>
        public void NameButtonClicked()
        {
            var renameAngleDialogGameObject = Instantiate(renameAngleDialogPrefab, _dialogContainer.transform);
            var renameAngleDialogRectTransform = renameAngleDialogGameObject.GetComponent<RectTransform>();
            renameAngleDialogRectTransform.offsetMin = new Vector2(0f, 0f);
            renameAngleDialogRectTransform.offsetMax = new Vector2(0f, 0f);
            var renameAngleDialog = renameAngleDialogGameObject.GetComponent<RenameAngleDialog>();
            renameAngleDialog.SetInitialValues(Angle, Angle.Name);
        }
    }
}
