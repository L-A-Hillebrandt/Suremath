using Math_View;
using UnityEngine;
using UnityEngine.UI;

namespace Panels.Shape_Panel
{
    /// <summary>
    /// Panel with buttons to make new shapes
    /// </summary>
    public class NewShapePanel : MonoBehaviour
    {
        /// <summary>
        /// The container to add the new shapes to
        /// </summary>
        private ShapePanelContainer _shapePanelContainer;

        void Start()
        {
            _shapePanelContainer = transform.parent.GetComponent<ShapePanelContainer>();
            
            // TODO: dirty hack but seems necessary for resolution-independent layout
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.localScale = new Vector2(1f, 1f);
        }

        void Update()
        {
            var _angleButtonGameObject = GameObject.Find("New Angle Button").gameObject;
            if (_shapePanelContainer.GetComponentsInChildren<LinePanel>().Length >= 2)
            {
                _angleButtonGameObject.GetComponent<Button>().interactable = true;
            }
            else
            {
                _angleButtonGameObject.GetComponent<Button>().interactable = false;
            }
        }

        /// <summary>
        /// Called when the new line button is clicked, creates a new line with default values
        /// </summary>
        public void NewLineButtonClicked()
        {
            var lineLock = new LineLock(false);
            _shapePanelContainer.AddLinePanel(lineLock, "Neue Gerade", Color.black, new Vector2(0f, 0f), new Vector2(1f, 1f), true);
        }
        /// <summary>
        /// Called when the new parabola button is clicked, creates a new parabola with default values
        /// </summary>
        public void NewParabolaButtonClicked()
        {
            var parabolaLock = new ParabolaLock(false);
            _shapePanelContainer.AddParabolaPanel(parabolaLock, "Neue Parabel", Color.black, new Vector2(0f, 0f), new Vector2(1f, 1f), true);
        }
        /// <summary>
        /// Called when the new angle button is clicked, creates a new angle panel with its selection mode active
        /// </summary>
        public void NewAngleButtonClicked()
        {
            _shapePanelContainer.AddAnglePanel();
        }
    }
}
