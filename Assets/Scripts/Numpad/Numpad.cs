using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Numpad : MonoBehaviour
{
    public GameObject buttonPrefab; // Button prefab to spawn
    public Transform parentTransform; // Parent transform for buttons
    public TMP_InputField inputField; // Reference to the input field where numbers will be inserted

    public int numRows = 3;
    public int numColumns = 3;

    public float buttonSpacing = 10f; // Spacing between buttons

    void OnEnable()
    {
        SpawnButtons();
    }

    void SpawnButtons()
    {
        float buttonWidth = buttonPrefab.GetComponent<RectTransform>().rect.width;
        float buttonHeight = buttonPrefab.GetComponent<RectTransform>().rect.height;

        float startX = -(numColumns - 1) * (buttonWidth + buttonSpacing) / 2;
        float startY = (numRows - 1) * (buttonHeight + buttonSpacing) / 2;

        int buttonCount = 1;

        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numColumns; col++)
            {
                GameObject buttonGO = Instantiate(buttonPrefab, parentTransform);
                buttonGO.GetComponentInChildren<TMP_Text>().text = buttonCount.ToString();
                int currentNumber = buttonCount; // Store the current number
                buttonGO.GetComponent<Button>().onClick.AddListener(() => OnNumberButtonClick(currentNumber.ToString()));
                buttonGO.transform.localPosition = new Vector3(startX + col * (buttonWidth + buttonSpacing), startY - row * (buttonHeight + buttonSpacing), 0);

                buttonCount++;
            }
        }

        // Spawn 0 button at the bottom left side
        GameObject zeroButton = Instantiate(buttonPrefab, parentTransform);
        zeroButton.GetComponentInChildren<TMP_Text>().text = "0";
        zeroButton.GetComponent<Button>().onClick.AddListener(() => OnNumberButtonClick("0"));
        zeroButton.transform.localPosition = new Vector3(startX, startY - numRows * (buttonHeight + buttonSpacing), 0);

        // Spawn X button at the bottom right side
        /*GameObject deleteButton = Instantiate(buttonPrefab, parentTransform);
        deleteButton.GetComponentInChildren<TMP_Text>().text = "X";
        deleteButton.GetComponent<Button>().onClick.AddListener(OnDeleteButtonClick);
        deleteButton.transform.localPosition = new Vector3(startX + (numColumns - 1) * (buttonWidth + buttonSpacing), startY - numRows * (buttonHeight + buttonSpacing), 0);*/
    }

    void OnNumberButtonClick(string number)
    {
        inputField.text += number;
    }

    public void OnDeleteButtonClick()
    {
        if (inputField.text.Length > 0)
        {
            inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
        }
    }
}
