using Math_View;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogs.Rename_Line_Dialog
{
    /// <summary>
    /// A dialog that lets the user rename a line 
    /// </summary>
    public class RenameLineDialog : MonoBehaviour
    {
        [SerializeField, Tooltip("The input field for the new name")]
        private InputField inputField;

        private Line _line;

        /// <summary>
        /// Sets the dialog up with the line it was called on and the line's old name
        /// </summary>
        /// <param name="line">The line the dialog was called on</param>
        /// <param name="oldName">The line's old name</param>
        public void SetInitialValues(Line line, string oldName = "")
        {
            _line = line;
            inputField.text = oldName;

            // TODO: dirty hack but seems necessary for resolution-independent layout
            var rectTransform = GetComponent<RectTransform>();
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
                _line.Name = inputField.text;
            }

            CancelButtonClicked();
        }
    }
}
