using Assets.Dialogs;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogs.Menu_Dialog
{
    /// <summary>
    /// A dialog with basic menu functions
    /// </summary>
    public class MenuDialog : MonoBehaviour
    {
        [SerializeField, Tooltip("Text object displaying the exercise path")]
        private Text exercisePathText;
        [SerializeField, Tooltip("Text object displaying the exercise list URL")]
        private Text exerciseListUrlText;
        [SerializeField, Tooltip("Text object displaying the exercise URL")]
        private Text exerciseUrlText;

        /// <summary>
        /// Config object that stores information on where exercise files are stored or can be accessed on the web
        /// </summary>
        private Config _config;

        //TODO: Display exercise path and URL
        void Start()
        {
            _config = ConfigManager.ReadConfig();
            exercisePathText.text = _config.LocalExercisePath;
            exerciseListUrlText.text = _config.ExerciseListUrl;
            exerciseUrlText.text = _config.ExerciseUrl;
        }

        /// <summary>
        /// Function called when the cancel button is clicked, destroys the dialog
        /// </summary>
        public void CancelButtonClicked()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// Function called when the accept button is clicked, terminates the application
        /// </summary>
        public void AcceptButtonClicked()
        {
            Application.Quit();
        }
    }
}
