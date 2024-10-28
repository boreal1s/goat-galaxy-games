using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            float interactRange = 2f;
            Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);
            foreach (Collider collider in colliderArray)
            {
                if (collider.TryGetComponent(out InteractableObject interactableObject)) {
                    interactableObject.Interact(collider.name);
                }
            }
        }
    }

    public InteractableObject GetInteractableObject()
    {
        List<InteractableObject> npcInteractableList = new List<InteractableObject>();
        float interactRange = 4f;
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);
        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent(out InteractableObject interactableObject)) {
                npcInteractableList.Add(interactableObject);
            }
        }

        InteractableObject closestInteractableObject = null;
        foreach (InteractableObject interactable in npcInteractableList) {
            if (closestInteractableObject == null) {
                closestInteractableObject = interactable;
            } else {
                if(Vector3.Distance(transform.position, interactable.transform.position) < 
                    Vector3.Distance(transform.position, closestInteractableObject.transform.position)) {
                        closestInteractableObject = interactable;
                    }
            }
        }

        return closestInteractableObject;
    }
}
