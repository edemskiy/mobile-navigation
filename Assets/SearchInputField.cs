using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchInputField : MonoBehaviour
{
    public Button clearButton;
    private InputField inputField;
    // Start is called before the first frame update
    private void Start()
    {
        inputField = GetComponent<InputField>();
    }
    public void OnValueChanged()
    {
        clearButton.gameObject.SetActive(inputField.text.Length > 0);
    }
}
