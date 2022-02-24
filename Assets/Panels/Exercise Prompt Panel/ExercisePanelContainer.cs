using UnityEngine;
using UnityEngine.UI;

namespace Panels.Exercise_Prompt_Panel
{
    /// <summary>
    /// Script for the container for exercise panels
    /// </summary>
    public class ExercisePanelContainer : MonoBehaviour
    {
        /// <summary>
        /// Prefab for text prompt panels
        /// </summary>
        [SerializeField, Tooltip("Prefab for text prompt panels")] 
        private GameObject textPromptPanelPrefab;
        /// <summary>
        /// Prefab for value prompt panels
        /// </summary>
        [SerializeField, Tooltip("Prefab for value prompt panels")]
        private GameObject valuePromptPanelPrefab;
        /// <summary>
        /// Prefab for point prompt panels
        /// </summary>
        [SerializeField, Tooltip("Prefab for point prompt panels")]
        private  GameObject pointPromptPanelPrefab;

        /// <summary>
        /// Adds a text prompt panel with the provided text to the panel container.
        /// </summary>
        /// <param name="promptText">The text of the prompt</param>
        public void AddTextPromptPanel(string promptText = "")
        {
            var promptPanelGameObject = (GameObject)Instantiate(textPromptPanelPrefab, transform);

            if (promptText.Length <= 0) return;
            var text = promptPanelGameObject.GetComponentInChildren<Text>();
            text.text = promptText;
        }

        /// <summary>
        /// Adds a value prompt panel with the provided text to the panel container.
        /// </summary>
        /// <param name="text">The text of the prompt</param>
        /// <param name="solution">The solution of the prompt</param>
        public void AddValuePromptPanel(string text, float solution)
        {
            var valuePromptPanelGameObject = (GameObject)Instantiate(valuePromptPanelPrefab, transform);
            var valuePromptPanel = valuePromptPanelGameObject.GetComponent<ValuePromptPanel>();
            valuePromptPanel.SetInitialValues(text, solution);
        }

        /// <summary>
        /// Adds a point prompt panel with the provided text to the panel container.
        /// </summary>
        /// <param name="text">The text of the prompt</param>
        /// <param name="solution">The solution of the prompt</param>
        public void AddPointPromptPanel(string text, Vector2 solution)
        {
            var pointPromptPanelGameObject = (GameObject)Instantiate(pointPromptPanelPrefab, transform);
            var pointPromptPanel = pointPromptPanelGameObject.GetComponent<PointPromptPanel>();
            pointPromptPanel.SetInitialValues(text, solution);
        }

        /// <summary>
        /// Clears all prompt panels from the prompt panel container.
        /// </summary>
        /// <param name="leaveDefaultPrompt">Whether or not to leave the default prompt in the container</param>
        public void ClearPanels(bool leaveDefaultPrompt)
        {
            for (var i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }

            if (leaveDefaultPrompt)
            {
                AddTextPromptPanel();
            }
        }

        /// <summary>
        /// Checks all prompt panels in the prompt panel container for whether or not the user entered the correct solution for each prompt.
        /// </summary>
        public void ValidatePanels()
        {
            var valueInputPanels = GetComponentsInChildren<ValuePromptPanel>();
            foreach (var valueInputPanel in valueInputPanels)
            {
                valueInputPanel.Validate();
            }

            var pointInputPanels = GetComponentsInChildren<PointPromptPanel>();
            foreach (var pointInputPanel in pointInputPanels)
            {
                pointInputPanel.Validate();
            }
        }
    }
}
