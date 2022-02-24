using Math_View;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogs.Rename_Parabola_Dialog
{
    public class RenameParabolaDialog : MonoBehaviour
    {
        [SerializeField, Tooltip("Input field for the new name")]
        private InputField inputField;

        private Parabola _parabola;

        public void SetInitialValues(Parabola parabola, string oldName = "")
        {
            _parabola = parabola;
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
                _parabola.Name = inputField.text;
            }

            CancelButtonClicked();
        }
    }
}
