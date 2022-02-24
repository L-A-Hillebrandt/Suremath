using UnityEngine;
using UnityEngine.UI;

namespace Dialogs.Error_Dialog
{
    /// <summary>
    /// Dialog that shows errors.
    /// </summary>
    public class ErrorDialog : MonoBehaviour
    {
        [SerializeField, Tooltip("The title text of the error message.")]
        private Text titleText;

        [SerializeField, Tooltip("The message text of the error.")]
        private Text messageText;

        void Start()
        {
        
        }

        void Update()
        {
        
        }

        /// <summary>
        /// Function called when the cancel button is clicked, destroys the dialog.
        /// </summary>
        public void CancelButtonClicked()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// Function to set the texts in the error message.
        /// </summary>
        /// <param name="title">The title of the error</param>
        /// <param name="message">The error message</param>
        public void SetTexts(string title, string message)
        {
            titleText.text = title;
            messageText.text = message;
        }
    }
}
