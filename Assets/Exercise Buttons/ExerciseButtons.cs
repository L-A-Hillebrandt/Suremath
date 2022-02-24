using Panels.Exercise_Prompt_Panel;
using Panels.Shape_Panel;
using UnityEngine;
using UnityEngine.UI;

namespace Exercise_Buttons
{
    public class ExerciseButtons : MonoBehaviour
    {
        /// <summary>
        /// Prefab for the exercise dialog
        /// </summary>
        [SerializeField, Tooltip("Prefab for the exercise dialog")] 
        private GameObject exerciseDialogPrefab;
        /// <summary>
        /// Prefab for the menu dialog
        /// </summary>
        [SerializeField, Tooltip("Prefab for the menu dialog")]
        private GameObject menuDialogPrefab;
        /// <summary>
        /// Dialog container in which dialogs are executed
        /// </summary>
        [SerializeField, Tooltip("Dialog container in which dialogs are executed")]
        private GameObject dialogContainer;
        /// <summary>
        /// Container for shape panels
        /// </summary>
        [SerializeField, Tooltip("Container for shape panels")]
        private ShapePanelContainer shapePanelContainer;
        /// <summary>
        /// Container for exercise panels
        /// </summary>
        [SerializeField, Tooltip("Container for exercise panels")]
        private ExercisePanelContainer exercisePanelContainer;
        /// <summary>
        /// The text where the exercise's title is shown
        /// </summary>
        [SerializeField, Tooltip("The text where the exercise's title is shown")]
        private Text exerciseTitleText;

        /// <summary>
        /// Function called when the menu button is clicked. Opens the main menu.
        /// </summary>
        public void MenuButtonClicked()
        {
            var menuDialog = Instantiate(menuDialogPrefab, dialogContainer.transform);

            var menuDialogRectTransform = menuDialog.GetComponent<RectTransform>();
            menuDialogRectTransform.offsetMin = new Vector2(0f, 0f);
            menuDialogRectTransform.offsetMax = new Vector2(0f, 0f);
        }
        /// <summary>
        /// Function called when the unload exercise button is clicked, removes all currently loaded prompts and shapes and resets the title text
        /// </summary>
        public void UnloadExerciseButtonClicked()
        {
            shapePanelContainer.ClearAllPanels();
            exerciseTitleText.text = "Aufgabe laden...";
            exercisePanelContainer.ClearPanels(true);
        }
        /// <summary>
        /// Function called when the load exercise button is clicked. Opens the exercise dialog.
        /// </summary>
        public void LoadExerciseButtonClicked()
        {
            var exerciseDialog = Instantiate(exerciseDialogPrefab, dialogContainer.transform);

            var exerciseDialogRectTransform = exerciseDialog.GetComponent<RectTransform>();
            exerciseDialogRectTransform.offsetMin = new Vector2(0f, 0f);
            exerciseDialogRectTransform.offsetMax = new Vector2(0f, 0f);
        }
        /// <summary>
        /// Function called when the solve exercise button is clicked. Checks all prompts in the prompt panel for correct solutions.
        /// </summary>
        public void SolveExerciseButtonClicked()
        {
            exercisePanelContainer.ValidatePanels();
        }
    }
}