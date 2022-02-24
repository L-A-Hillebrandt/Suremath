using Math_View;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogs.Rename_Angle_Dialog
{
    /// <summary>
    /// A dialog that lets the user rename an angle
    /// </summary>
    public class RenameAngleDialog : MonoBehaviour
    {
        [SerializeField]
        InputField inputField;

        Angle _angle;

        /// <summary>
        /// Sets the dialog up with the angle object to be changed and the angle's old name
        /// </summary>
        /// <param name="angle">The angle to be changed</param>
        /// <param name="oldName">The angle to be changed</param>
        public void SetInitialValues(Angle angle, string oldName = "")
        {
            this._angle = angle;
            inputField.text = oldName;

            // TODO: dirty hack but seems necessary for resolution-independent layout
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.localScale = new Vector2(1f, 1f);
        }

        public void CancelButtonClicked()
        {
            Destroy(gameObject);
        }

        public void AcceptButtonClicked()
        {
            if (inputField.text.Length > 0)
            {
                _angle.Name = inputField.text;
            }

            CancelButtonClicked();
        }
    }
}
