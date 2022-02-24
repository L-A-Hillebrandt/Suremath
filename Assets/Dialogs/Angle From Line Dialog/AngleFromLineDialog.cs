using Math_View;
using Panels.Shape_Panel;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogs.Angle_From_Line_Dialog
{
    /// <summary>
    /// A dialog that lets the user create a new angle from an existing line
    /// </summary>
    public class AngleFromLineDialog : MonoBehaviour
    {
        /// <summary>
        /// The input field for the value of the angle
        /// </summary>
        [SerializeField, Tooltip("The input field for the value of the angle")]
        private InputField inputField;

        /// <summary>
        /// The button that toggles between radians and degrees
        /// </summary>
        [SerializeField, Tooltip("The button that toggles between radians and degrees")]
        private Text radToggleButtonText;

        /// <summary>
        /// The shape panel container the newly generated line and angle panels are to be inserted into
        /// </summary>
        private ShapePanelContainer _shapePanelContainer;

        /// <summary>
        /// The line this dialog was called from
        /// </summary>
        private Line _line;

        /// <summary>
        /// Whether or not the angle uses radians
        /// </summary>
        private bool UseRadians
        {
            get;
            set;
        }

        void Start()
        {
            _shapePanelContainer = GameObject.Find("Shape Panel Container").GetComponent<ShapePanelContainer>();

            UseRadians = false;
        }

        /// <summary>
        /// Sets the dialog up with the line it was called from
        /// </summary>
        /// <param name="line">The line on which to base the new angle</param>
        public void SetInitialValues(Line line)
        {
            _line = line;

            // TODO: dirty hack but seems necessary for resolution-independent layout
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.localScale = new Vector2(1f, 1f);
        }

        /// <summary>
        /// Function called when the cancel button is clicked, destroys the dialog
        /// </summary>
        public void CancelButtonClicked()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// Function called when the accept button is clicked, parses the input and creates a new line and a new angle from it
        /// </summary>
        public void AcceptButtonClicked()
        {
            if (inputField.text.Length > 0)
            {
                //Find angle of base line from slope
                var baseAngle = Mathf.Atan(_line.FormulaValues.x);
                float newAngle;
                float input;
                try
                {
                    //Replace periods with commas in the input to ease user frustration
                    input = float.Parse(inputField.text.Replace('.', ','));
                }
                catch(System.FormatException e)
                {
                    _shapePanelContainer.DisplayErrorMessage("Fehler", "Eingegebener Wert ist keine Zahl.");
                    Debug.Log(e.Message);
                    return;
                }

                if (UseRadians)
                {
                    //If the angle entered is already in radians, no conversion is needed
                    newAngle = baseAngle + input;
                }
                else
                {
                    //Otherwise, we need to convert degrees to radians
                    newAngle = baseAngle + ((input * Mathf.PI) / 180f);
                }

                //The origin of the new line is always (0,0)
                var pointA = new Vector2(0f, 0f);
                //The second handle of the new line is calculated with cosine and sine of the new angle
                var pointB = new Vector2(Mathf.Cos(newAngle) * 2.0f, Mathf.Sin(newAngle) * 2.0f);

                //Creating the new line and saving its GameObject because we need the Line object later for the angle
                var lineLock = new LineLock(false);
                var newLinePanelGameObject = _shapePanelContainer.AddLinePanel(lineLock, "Neue Gerade", Color.black, pointA, pointB, true, false);
                var newLine = newLinePanelGameObject.GetComponent<LinePanel>();

                //Creating the angle panel and its angle object
                var anglePanelGameObject = _shapePanelContainer.AddAnglePanel();
                var anglePanel = anglePanelGameObject.GetComponent<AnglePanel>();
                var angleLock = new AngleLock(false);
                var angle = new Angle(angleLock,_line, newLine.Line, "Neuer Winkel", Color.black);
                angle.CalculateVertex();
                angle.CalculateAngle();
                anglePanel.Angle = angle;
            }

            CancelButtonClicked();
        }

        /// <summary>
        /// Function called when the radians toggle is clicked, toggles between radians and degrees
        /// </summary>
        public void RadiansToggleButtonClicked()
        {
            if (UseRadians)
            {
                UseRadians = false;
                radToggleButtonText.text = "°";
            }
            else
            {
                UseRadians = true;
                radToggleButtonText.text = "rad";
            }
        }
    }
}
