using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInteractableUI : MonoBehaviour
{
    [SerializeField] private GameObject interactableUI;
    [SerializeField] private PlayerInteract playerInteract;
    [SerializeField] private TextMeshProUGUI interactableMessage;

    private void Update() {
        if (playerInteract.GetInteractableObject() != null) {
            Show(playerInteract.GetInteractableObject());
        }
        else {
            Hide();
        }
    }

    private void Show(InteractableObject interactableObject) {
        // Debug.Log("Show");
        interactableUI.SetActive(true);
        interactableMessage.text = interactableObject.GetInteractText();
    }

    private void Hide() {
        // Debug.Log("Hide");
        interactableUI.SetActive(false);
    }
}
