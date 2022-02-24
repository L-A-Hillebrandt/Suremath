using Panels.Shape_Panel;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Math_View
{
    /// <summary>
    /// Object that manages user interaction with the math view
    /// </summary>
    [RequireComponent(typeof(MathViewRenderer))]
    [RequireComponent(typeof(RectTransform))]
    public class MathViewInputManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IScrollHandler
    {
        /// <summary>
        /// Container for shape panels
        /// </summary>
        [SerializeField, Tooltip("Container for shape panels")]
        private Transform panelContainer;
        /// <summary>
        /// The speed at which the camera can be dragged around
        /// </summary>
        [SerializeField, Tooltip("The speed at which the camera can be dragged around")]
        private float cameraDragSpeed;

        /// <summary>
        /// Object that handles rendering shapes in the math view
        /// </summary>
        private MathViewRenderer _mathViewRenderer;

        /// <summary>
        /// A line that is currently being dragged. Null, if there is no currently dragged line.
        /// </summary>
        private Line _draggedLine;

        /// <summary>
        /// Which part of a Line is being dragged. 0 if it's handle A, 1 if it's handle B, 2 if it's both
        /// </summary>
        private int _draggedLinePiece;

        /// <summary>
        /// A parabola that is currently being dragged. Null, if there is no currently dragged parabola.
        /// </summary>
        private Parabola _draggedParabola;
        /// <summary>
        /// Which part of a parabola is being dragged. 0 if it's the origin handle, 1 if it's the curve handle, 2 if it's both
        /// </summary>
        private int _draggedParabolaPiece;

        /// <summary>
        /// Time in seconds since the last click
        /// </summary>
        private float _doubleClickCooldown;

        /// <summary>
        /// RectTransform of the math view, for interpolating mouse position
        /// </summary>
        private RectTransform _rectTransform;

        /// <summary>
        /// Object that manages line selection if there's an angle being newly set up
        /// </summary>
        private LineSelectionManager _lineSelectionManager;

        /// <summary>
        /// Related to the LineSelectionManager, shows whether or not the line selection is active
        /// </summary>
        private bool _selectionModeActive;

        void Start()
        {
            _mathViewRenderer = GetComponent<MathViewRenderer>();
            //Fetch rectangle of math view
            _rectTransform = GetComponent<RectTransform>();
		
            _draggedLine = null;
            _draggedParabola = null;

            _doubleClickCooldown = 0f;
        }

        void Update()
        {
            if (_doubleClickCooldown > 0f)
            {
                _doubleClickCooldown -= Time.deltaTime;
            }
        }

        public void OnPointerDown(PointerEventData data)
        {
            /*
		Vector2 mousePosition = data.position - new Vector2(780f, 310f);
		mousePosition /= 260f;
		*/

            Vector3[] corners = new Vector3[4];
            _rectTransform.GetWorldCorners(corners);
            //Subtract mouse coordinates from top left corner of math view rectangle to get coordinates within that rectangle
            Vector2 mousePosition = data.position - new Vector2(corners[0].x, corners[0].y);

            //Get size/resolution of math view rectangle by subtracting top left coordinates from bottom right coordinates
            Vector2 size = corners[2] - corners[0];

            //Scaling mouse movements to the math view renderer area
            mousePosition.x /= size.x;
            mousePosition.y /= size.y;

            mousePosition.x = mousePosition.x * 2f - 1f;
            mousePosition.y = mousePosition.y * 2f - 1f;
            //Debug.Log(mousePosition);

            //Get all angle panels in the shape panel container
            AnglePanel[] anglePanels = panelContainer.GetComponentsInChildren<AnglePanel>();
            bool hasDetectedActiveAnglePanel = false;
            foreach (AnglePanel anglePanel in anglePanels)
            {
                if (anglePanel.SelectionModeActive)
                {
                    _selectionModeActive = true;
                    hasDetectedActiveAnglePanel = true;
                    _lineSelectionManager = anglePanel.LineSelectionManager;
                    break;
                }
            }

            if (!hasDetectedActiveAnglePanel)
            {
                _selectionModeActive = false;
            }

            //Get all line panels in the shape panel container
            LinePanel[] linePanels = panelContainer.GetComponentsInChildren<LinePanel>();
            foreach (LinePanel linePanel in linePanels)
            {
                Line line = linePanel.Line;

                //Ignore line if line is locked and thus can't be manipulated
                if (line.Locked)
                    continue;

                //TODO: Find out about raster graphics maths
                Vector2 distanceClickToHandle = (line.PointAHandle /*- cameraPosition*/ / (_mathViewRenderer.CameraZoom / 2f)) - mousePosition;
                if (distanceClickToHandle.magnitude < 0.1f)
                {
                    if (line.Lock.HandleALocked)
                    {
                        continue;
                    }

                    if (_selectionModeActive && _lineSelectionManager.CurrentlySelectedLines.Count < 2)
                    {
                        _lineSelectionManager.AddLine(line);
                    }
                    else
                    {
                        line.PointAHandle += data.delta * _mathViewRenderer.CameraZoom * (cameraDragSpeed / 1000f);
                        _draggedLine = line;
                        _draggedParabola = null;

                        //If there has been a click in the last half second
                        if (_doubleClickCooldown > 0f && (!line.Lock.HandleALocked && !line.Lock.HandleBLocked))
                        {
                            _draggedLinePiece = 2;
                        }
                        else
                        {
                            _draggedLinePiece = 0;
                        }

                        return;
                    }
                }

                distanceClickToHandle = (line.PointBHandle /*- cameraPosition*/ / (_mathViewRenderer.CameraZoom / 2f)) - mousePosition;
                if (distanceClickToHandle.magnitude < 0.1f)
                {
                    if (line.Lock.HandleBLocked)
                    {
                        continue;
                    }

                    if (_selectionModeActive && _lineSelectionManager.CurrentlySelectedLines.Count < 2)
                    {
                        _lineSelectionManager.AddLine(line);
                    }
                    else
                    {
                        line.PointBHandle += data.delta * _mathViewRenderer.CameraZoom * (cameraDragSpeed / 1000f);
                        _draggedLine = line;
                        _draggedParabola = null;

                        //If there has been a click in the last half second
                        if (_doubleClickCooldown > 0f && (!line.Lock.HandleALocked && !line.Lock.HandleBLocked))
                        {
                            _draggedLinePiece = 2;
                        }
                        else
                        {
                            _draggedLinePiece = 1;
                        }

                        return;
                    }
                }
            }

            ParabolaPanel[] parabolaPanels = panelContainer.GetComponentsInChildren<ParabolaPanel>();
            foreach (ParabolaPanel parabolaPanel in parabolaPanels)
            {
                Parabola parabola = parabolaPanel.Parabola;

                //Ignore parabola if parabola is locked and thus can't be manipulated
                if (parabola.Locked)
                    continue;

                Vector2 distanceClickToHandle = (parabola.OriginHandle /*- cameraPosition*/ / (_mathViewRenderer.CameraZoom / 2f)) - mousePosition;
                //Debug.Log("Handle = " + (parabola.CurveHandle /*- cameraPosition*/ / (cameraZoom / 2f)) + "   Mouse Click = " + mouseClick);
                if (distanceClickToHandle.magnitude < 0.1f)
                {
                    if (parabola.Lock.OriginLocked)
                    {
                        continue;
                    }
                    parabola.OriginHandle += data.delta * _mathViewRenderer.CameraZoom * (cameraDragSpeed / 1000f);
                    _draggedParabola = parabola;
                    _draggedLine = null;

                    //If there has been a click in the last half second
                    if (_doubleClickCooldown > 0f && (!parabola.Lock.OriginLocked && !parabola.Lock.CurveLocked))
                    {
                        _draggedParabolaPiece = 2;
                    }
                    else
                    {
                        _draggedParabolaPiece = 0;
                    }

                    return;
                }

                distanceClickToHandle = (parabola.CurveHandle /*- cameraPosition*/ / (_mathViewRenderer.CameraZoom / 2f)) - mousePosition;
                if (distanceClickToHandle.magnitude < 0.1f)
                {
                    if (parabola.Lock.CurveLocked)
                    {
                        continue;
                    }
                    parabola.CurveHandle += data.delta * _mathViewRenderer.CameraZoom * (cameraDragSpeed / 1000f);
                    _draggedParabola = parabola;
                    _draggedLine = null;

                    //If there has been a click in the last half second
                    if (_doubleClickCooldown > 0f && (!parabola.Lock.OriginLocked && !parabola.Lock.CurveLocked))
                    {
                        _draggedParabolaPiece = 2;
                    }
                    else
                    {
                        _draggedParabolaPiece = 1;
                    }

                    return;
                }

                /*
			v = (parabola.CurveHandleCopy / (cameraZoom / 2f)) - mousePosition;
			if (v.magnitude < 0.1f)
			{
				parabola.CurveHandleCopy += data.delta * cameraZoom * (cameraDragSpeed / 1000f);
				draggedParabola = parabola;
				draggedParabolaPiece = 2;
				return;
			}
			*/
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_selectionModeActive)
            {
                if (_draggedLine != null)
                {
                    if (_draggedLine.SnapToGrid)
                    {
                        _draggedLine.RasterizeHandles();
                    }

                    _draggedLine = null;
                    _doubleClickCooldown = 0.5f;
                }
                else if (_draggedParabola != null)
                {
                    if (_draggedParabola.SnapToGrid)
                    {
                        _draggedParabola.RasterizeOriginAndHandles();
                    }

                    _draggedParabola = null;
                    _doubleClickCooldown = 0.5f;
                }
            }
        }

        public void OnDrag(PointerEventData data)
        {
            //LinePiece 0 = PointA/OriginHandle, LinePiece 1 = PointB/CurveHandle, LinePiece 2 = both.
            //Essentially, if draggedLinePiece is 2, it will always drag both handles, regardless of which one was dragged
            if (!_selectionModeActive)
            {
                if (_draggedLine != null)
                {
                    if (_draggedLinePiece == 0 || _draggedLinePiece == 2)
                        _draggedLine.PointAHandle += data.delta * _mathViewRenderer.CameraZoom * (cameraDragSpeed / 1000f);

                    if (_draggedLinePiece == 1 || _draggedLinePiece == 2)
                        _draggedLine.PointBHandle += data.delta * _mathViewRenderer.CameraZoom * (cameraDragSpeed / 1000f);
                }
                else if (_draggedParabola != null)
                {
                    if (_draggedParabolaPiece == 0 || _draggedParabolaPiece == 2)
                        _draggedParabola.OriginHandle += data.delta * _mathViewRenderer.CameraZoom * (cameraDragSpeed / 1000f);

                    if (_draggedParabolaPiece == 1 || _draggedParabolaPiece == 2)
                        _draggedParabola.CurveHandle += data.delta * _mathViewRenderer.CameraZoom * (cameraDragSpeed / 1000f);
                }
                else
                {
                    //cameraPosition += data.delta * cameraZoom * (cameraDragSpeed / 1000f);
                }
            }
        }

        public void OnScroll(PointerEventData data)
        {
            _mathViewRenderer.CameraZoom -= data.scrollDelta.y;

            if (_mathViewRenderer.CameraZoom < 1f)
                _mathViewRenderer.CameraZoom = 1f;
            else if (_mathViewRenderer.CameraZoom > 100f)
                _mathViewRenderer.CameraZoom = 100f;

            //Debug.Log("Camera Zoom = " + cameraZoom);
        }
    }
}
