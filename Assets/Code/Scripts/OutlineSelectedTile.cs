using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OutlineSelectedTile : MonoBehaviour {
    private Transform highlight;
    private Transform selection;
    private RaycastHit hit;

    void Update() {
        if (highlight != null) {
            highlight.gameObject.GetComponent<Outline>().enabled = false;
            highlight = null;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out hit)) {
            highlight = hit.transform;
            if (highlight.CompareTag("Selectable") && highlight != selection) {
                if (highlight.gameObject.GetComponent<Outline>() != null) {
                    highlight.gameObject.GetComponent<Outline>().enabled = true;
                } else {
                    Outline outline = highlight.gameObject.AddComponent<Outline>();
                    outline.enabled = true;
                    highlight.gameObject.GetComponent<Outline>().OutlineColor = Color.yellow;
                    highlight.gameObject.GetComponent<Outline>().OutlineWidth = 7.0f;
                }
            }
            else {
                highlight = null;
            }
        }

        if (Input.GetMouseButtonDown(0)) {
            if (highlight) {
                if (selection != null) {
                    selection.gameObject.GetComponent<Outline>().enabled = false;
                }
                selection = hit.transform;
                selection.gameObject.GetComponent<Outline>().enabled = true;
                highlight = null;
            }
            else {
                if (selection) {
                    selection.gameObject.GetComponent<Outline>().enabled = false;
                    selection = null;
                }
            }
        }
    }
}
