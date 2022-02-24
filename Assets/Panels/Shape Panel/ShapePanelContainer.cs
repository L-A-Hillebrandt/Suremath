using Dialogs.Error_Dialog;
using Math_View;
using UnityEngine;

namespace Panels.Shape_Panel
{
    /// <summary>
    /// Container for all shape panels
    /// </summary>
    public class ShapePanelContainer : MonoBehaviour
    {
        /// <summary>
        /// Prefab for the panel containing buttons to make new shapes
        /// </summary>
        [SerializeField, Tooltip("Prefab for the panel containing buttons to make new shapes")] 
        private GameObject newShapePanelPrefab;
        /// <summary>
        /// Prefab for new line panels
        /// </summary>
        [SerializeField, Tooltip("Prefab for new line panels")]
        private GameObject linePanelPrefab;
        /// <summary>
        /// Prefab for new parabola panels
        /// </summary>
        [SerializeField, Tooltip("Prefab for new line panels")]
        private GameObject parabolaPanelPrefab;
        /// <summary>
        /// Prefab for new angle panels
        /// </summary>
        [SerializeField, Tooltip("Prefab for new angle panels")]
        private GameObject anglePanelPrefab;
        /// <summary>
        /// Prefab for the error dialog
        /// </summary>
        [SerializeField, Tooltip("Prefab for the error dialog")]
        private GameObject errorDialogPrefab;

        /// <summary>
        /// Container for dialogs
        /// </summary>
        private GameObject _dialogContainer;

        void Start()
        {
            _dialogContainer = GameObject.Find("Dialog Container");
        }

        /// <summary>
        /// Displays an error dialog with the given title and message
        /// </summary>
        /// <param name="title">The title of the error message</param>
        /// <param name="message">The error message</param>
        public void DisplayErrorMessage(string title, string message)
        {
            var errorDialog = Instantiate(errorDialogPrefab, _dialogContainer.transform);

            var errorDialogRectTransform = errorDialog.GetComponent<RectTransform>();
            errorDialogRectTransform.offsetMin = new Vector2(0f, 0f);
            errorDialogRectTransform.offsetMax = new Vector2(0f, 0f);

            var dialogScript = errorDialog.GetComponent<ErrorDialog>();

            dialogScript.SetTexts(title, message);
        }

        /// <summary>
        /// Removes the panel that contains buttons to make new shapes
        /// </summary>
        void RemoveNewShapePanel()
        {
            if (transform.childCount > 0)
            {
                Destroy(transform.GetChild(transform.childCount - 1).gameObject);
            }
        }

        /// <summary>
        /// Adds the panel that contains buttons to make new shapes
        /// </summary>
        private void AddNewShapePanel()
        {
            Instantiate(newShapePanelPrefab, transform);
        }

        /// <summary>
        /// Adds a new line panel to the container.
        /// </summary>
        /// <param name="lineLock">LineLock object that specifies what inputs and fields are locked/visible</param>
        /// <param name="lineName">The name of the line</param>
        /// <param name="color">The color of the line</param>
        /// <param name="pointAHandle">The coordinates of handle A of the line</param>
        /// <param name="pointBHandle">The coordinates of handle B of the line</param>
        /// <param name="infinite">Whether or not the line is infinite</param>
        /// <param name="id">The id of the line</param>
        /// <returns>The game object of the LinePanel that was created</returns>
        /// <see cref="LinePanel"/>
        public GameObject AddLinePanel(LineLock lineLock, string lineName, Color color, Vector2 pointAHandle, Vector2 pointBHandle, bool infinite, int id, bool snapToGrid = true)
        {
            RemoveNewShapePanel();

            var linePanelGameObject = Instantiate(linePanelPrefab, transform);
            var linePanel = linePanelGameObject.GetComponent<LinePanel>();
            linePanel.Line = new Line(lineLock, lineName, color, pointAHandle, pointBHandle, infinite, id, snapToGrid);

            if (color == Color.black)
            {
                linePanel.Line.RandomizeColor();
            }

            linePanel.UpdateColorSwatch();

            AddNewShapePanel();

            return linePanelGameObject;
        }
        /// <summary>
        /// Adds a new line panel to the container.
        /// </summary>
        /// <param name="lineLock">LineLock object that specifies what inputs and fields are locked/visible</param>
        /// <param name="lineName">The name of the line</param>
        /// <param name="color">The color of the line</param>
        /// <param name="pointAHandle">The coordinates of handle A of the line</param>
        /// <param name="pointBHandle">The coordinates of handle B of the line</param>
        /// <param name="infinite">Whether or not the line is infinite</param>
        /// <param name="snapToGrid">Whether or not the line snaps to the grid</param>
        /// <returns></returns>
        public GameObject AddLinePanel(LineLock lineLock, string lineName, Color color, Vector2 pointAHandle, Vector2 pointBHandle, bool infinite, bool snapToGrid = true)
        {
            RemoveNewShapePanel();

            var linePanelGameObject = Instantiate(linePanelPrefab, transform);
            var linePanel = linePanelGameObject.GetComponent<LinePanel>();
            linePanel.Line = new Line(lineLock, lineName, color, pointAHandle, pointBHandle, infinite, snapToGrid);

            if (color == Color.black)
            {
                linePanel.Line.RandomizeColor();
            }

            linePanel.UpdateColorSwatch();

            AddNewShapePanel();

            return linePanelGameObject;
        }

        /// <summary>
        /// Adds a new parabola panel to the container.
        /// </summary>
        /// <param name="name">The name of the parabola</param>
        /// <param name="color">The color of the parabola</param>
        /// <param name="originHandle">The coordinates of the origin of the parabola</param>
        /// <param name="curveHandle">The coordinates of the curve handle of the parabola</param>
        /// <param name="infinite">Whether or not the parabola is infinite</param>
        /// <param name="parabolaLock">ParabolaLock object that specifies what inputs and fields are locked/visible</param>
        /// <returns>The game object of the parabola panel that was created</returns>
        /// <see cref="ParabolaPanel"/>
        public GameObject AddParabolaPanel(ParabolaLock parabolaLock, string name, Color color, Vector2 originHandle, Vector2 curveHandle, bool infinite)
        {
            RemoveNewShapePanel();

            var parabolaPanelGameObject = Instantiate(parabolaPanelPrefab, transform);
            var parabolaPanel = parabolaPanelGameObject.GetComponent<ParabolaPanel>();
            parabolaPanel.Parabola = new Parabola(parabolaLock, name, color, originHandle, curveHandle, infinite);

            if (color == Color.black)
            {
                parabolaPanel.Parabola.RandomizeColor();
            }

            parabolaPanel.UpdateColorSwatch();

            AddNewShapePanel();

            return parabolaPanelGameObject;
        }

        /// <summary>
        /// Adds a new angle panel to the container. Caution! Angle object must be filled in later!
        /// </summary>
        /// <returns>The game object of the angle panel that was created</returns>
        /// <see cref="AnglePanel"/>
        public GameObject AddAnglePanel()
        {
            var linePanels = GetComponentsInChildren<LinePanel>();

            if (linePanels.Length < 2)
            {
                DisplayErrorMessage("Fehler", "Es sind nicht genug Geraden vorhanden, um einen Winkel zu erstellen.");
                return null;
            }
            RemoveNewShapePanel();

            var anglePanelGameObject = (GameObject)Instantiate(anglePanelPrefab, transform);

            AddNewShapePanel();
            return anglePanelGameObject;
        }

        /// <summary>
        /// Removes a panel from the container
        /// </summary>
        /// <param name="panel">The panel to be removed</param>
        public void RemovePanel(GameObject panel)
        {
            Destroy(panel);
        }

        /// <summary>
        /// Removes all shape panels currently in the container, but re-adds the one that lets the user make new shapes
        /// </summary>
        public void ClearAllPanels()
        {
            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }

            AddNewShapePanel();
        }

        /// <summary>
        /// Returns an XML-formatted representation of all shape panels in the container
        /// </summary>
        /// <returns>An XML representation of all shape panels in the container</returns>
        public string PanelsToString()
        {
            var output = "";

            for (var i = 0; i < transform.childCount; i++)
            {
                var childGameObject = transform.GetChild(i).gameObject;

                var linePanel = childGameObject.GetComponent<LinePanel>();
                if (linePanel != null)
                {
                    output += "<line name=\"" + linePanel.Line.Name + "\"id =\"" + linePanel.Line.ID + "\">";
                    output += "<color r=\"" + linePanel.Line.Color.r + "\" g=\"" + linePanel.Line.Color.g + "\" b=\"" + linePanel.Line.Color.b + "\"/>";
                    output += "<handle1 x=\"" + linePanel.Line.PointAHandle.x + "\" y=\"" + linePanel.Line.PointAHandle.y + "\"/>";
                    output += "<handle2 x=\"" + linePanel.Line.PointBHandle.x + "\" y=\"" + linePanel.Line.PointBHandle.y + "\"/>";
                    output += "<properties infinite=\"" + linePanel.Line.Infinite + "\"/>";
                    output += "</line>";
                    continue;
                }

                var parabolaPanel = childGameObject.GetComponent<ParabolaPanel>();
                if (parabolaPanel != null)
                {
                    output += "<parabola name=\"" + parabolaPanel.Parabola.Name + "\">";
                    output += "<color r=\"" + parabolaPanel.Parabola.Color.r + "\" g=\"" + parabolaPanel.Parabola.Color.g + "\" b=\"" + parabolaPanel.Parabola.Color.b + "\"/>";
                    output += "<originhandle x=\"" + parabolaPanel.Parabola.OriginHandle.x + "\" y=\"" + parabolaPanel.Parabola.OriginHandle.y + "\"/>";
                    output += "<curvehandle x=\"" + parabolaPanel.Parabola.CurveHandle.x + "\" y=\"" + parabolaPanel.Parabola.CurveHandle.y + "\"/>";
                    output += "<properties infinite=\"" + parabolaPanel.Parabola.Infinite + "\"/>";
                    output += "</parabola>";
                }

                var anglePanel = childGameObject.GetComponent<AnglePanel>();
                if (anglePanel != null)
                {
                    output += "<angle name=\"" + anglePanel.Angle.Name + "\">";
                    output += "<color r=\"" + anglePanel.Angle.Color.r + "\" g=\"" + anglePanel.Angle.Color.g + "\" b=\"" +
                              anglePanel.Angle.Color.b + "\"/>";
                    output += "<lines a=\"" + anglePanel.Angle.LineA.ID + "\" b=\"" + anglePanel.Angle.LineB.ID + "\">";
                    output += "</angle>";
                }
            }

            return output;
        }
    }
}
