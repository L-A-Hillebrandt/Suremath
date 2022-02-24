using Dialogs.Exercise_Dialog;
using UnityEngine;
using UnityEngine.UI;

namespace Panels.Exercise_Browser_Panel
{
    /// <summary>
    /// A panel showing a list of exercises for the user to choose
    /// </summary>
    public class ExerciseBrowserPanel : MonoBehaviour
    {
        /// <summary>
        /// Game object for the UI element that shows the title of an exercise
        /// </summary>
        public GameObject titleTextGameObject;

        /// <summary>
        /// Game object for the UI element that shows the author of an exercise
        /// </summary>
        [SerializeField, Tooltip("Game object for the UI element that shows the author of an exercise")]
        private GameObject authorTextGameObject;

        /// <summary>
        /// Game object for the UI element that shows the faculty an exercise originated from
        /// </summary>
        [SerializeField, Tooltip("Game object for the UI element that shows the faculty an exercise originated from")]
        private GameObject facultyTextGameObject;

        /// <summary>
        /// The exercise dialog this panel belongs to
        /// </summary>
        private ExerciseDialog _exerciseDialog;

        /// <summary>
        /// The id of the exercise
        /// </summary>
        private int _id;

        void Start()
        {
            // TODO: dirty hack but seems necessary for resolution-independent layout
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.localScale = new Vector2(1f, 1f);
        }

        /// <summary>
        /// Sets the exercise dialog for this panel.
        /// </summary>
        /// <param name="exerciseDialog">The exercise dialog to set</param>
        public void SetExerciseDialog(ExerciseDialog exerciseDialog)
        {
            _exerciseDialog = exerciseDialog;
        }

        /// <summary>
        /// Populates this panel with initial values of an exercise.
        /// </summary>
        /// <param name="id">The id of an exercise</param>
        /// <param name="title">The title of an exercise</param>
        /// <param name="author">The author of an exercise</param>
        /// <param name="faculty">The faculty an exercise originated from</param>
        public void SetInitialValues(int id, string title, string author, string faculty)
        {
            _id = id;

            var titleText = titleTextGameObject.GetComponent<Text>();
            titleText.text = title;

            var authorText = authorTextGameObject.GetComponent<Text>();
            authorText.text = author;

            var facultyText = facultyTextGameObject.GetComponent<Text>();
            facultyText.text = faculty;
        }

        /// <summary>
        /// Calls the LoadExercise method on the exercise dialog this panel belongs to with the id of the exercise this panel corresponds with.
        /// </summary>
        public void LoadExercise()
        {
            _exerciseDialog.LoadExercise(_id);
        }
    }
}
