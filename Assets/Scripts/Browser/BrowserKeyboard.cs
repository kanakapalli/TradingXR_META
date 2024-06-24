using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BrowserKeyboard : MonoBehaviour
{
    public TMP_InputField targetInputField;
    public GameObject buttonPrefab;

    private Dictionary<string, string> keyValues = new Dictionary<string, string>()
    {
        { "1", "1" }, { "2", "2" }, { "3", "3" }, { "4", "4" }, { "5", "5" },
        { "6", "6" }, { "7", "7" }, { "8", "8" }, { "9", "9" }, { "0", "0" },
        { "Q", "Q" }, { "W", "W" }, { "E", "E" }, { "R", "R" }, { "T", "T" },
        { "Y", "Y" }, { "U", "U" }, { "I", "I" }, { "O", "O" }, { "P", "P" },
        { "A", "A" }, { "S", "S" }, { "D", "D" }, { "F", "F" }, { "G", "G" },
        { "H", "H" }, { "J", "J" }, { "K", "K" }, { "L", "L" },
        { "Z", "Z" }, { "X", "X" }, { "C", "C" }, { "V", "V" }, { "B", "B" },
        { "N", "N" }, { "M", "M" }, { ".", "." }, { "/", "/" }, { "\\", "\\" },
        { "Space", " " }, { "Backspace", "Backspace" }
    };

    private void Start()
    {
        CreateKeyboard();
    }

    private void CreateKeyboard()
    {
        // Create the rows for the keyboard
        string[] row1 = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
        string[] row2 = { "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P" };
        string[] row3 = { "A", "S", "D", "F", "G", "H", "J", "K", "L" };
        string[] row4 = { "Z", "X", "C", "V", "B", "N", "M", ".", "/", "\\" };
        string[] row5 = { "Space", "Backspace" };

        // Create keyboard rows
        CreateRow(row1);
        CreateRow(row2);
        CreateRow(row3);
        CreateRow(row4);
        CreateRow(row5);
    }

    private void CreateRow(string[] keys)
    {
        // Create a new GameObject for the row
        GameObject row = new GameObject("Row");
        row.transform.SetParent(transform, false);

        // Add GridLayoutGroup to the row
        GridLayoutGroup gridLayoutGroup = row.AddComponent<GridLayoutGroup>();
        RectTransform rectTransform = row.GetComponent<RectTransform>();

        // Adjust the layout settings
        gridLayoutGroup.cellSize = new Vector2(100, 100); // Adjust size as needed
        gridLayoutGroup.spacing = new Vector2(10, 10); // Adjust spacing as needed
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = keys.Length;
        gridLayoutGroup.childAlignment = TextAnchor.MiddleCenter;

        // Instantiate keys for the row
        foreach (string key in keys)
        {
            GameObject buttonGO = Instantiate(buttonPrefab, row.transform);
            buttonGO.name = key;

            // Set button text
            TextMeshProUGUI buttonText = buttonGO.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = key;
            }

            // Add click listener
            Button button = buttonGO.GetComponent<Button>();
            button.onClick.AddListener(() => OnKeyPress(keyValues[key]));
        }
    }

    private void OnKeyPress(string key)
    {
        if (key == "Backspace")
        {
            if (targetInputField.text.Length > 0)
            {
                targetInputField.text = targetInputField.text.Substring(0, targetInputField.text.Length - 1);
            }
        }
        else
        {
            targetInputField.text += key;
        }
    }
}
