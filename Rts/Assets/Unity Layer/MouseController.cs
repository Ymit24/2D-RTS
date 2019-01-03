using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour {
	static public event Action<Vector2, int, bool> OnClick;
	static private GameObject mouseControllerGO;
	static public void CreateIfNeeded() {
		if (mouseControllerGO != null) return;
		mouseControllerGO = new GameObject("MouseController", typeof(MouseController));
	}
	
	protected void HandleMouseAction(int button, bool pressed) {
		if (OnClick != null) {
			OnClick(Input.mousePosition, button, pressed);
		}
	}
	
	protected void Update()
	{
		if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() == true)
		{
			return;
		}
		if (Input.GetMouseButtonDown(0)) { HandleMouseAction(0, true);  }
		if (Input.GetMouseButtonDown(1)) { HandleMouseAction(1, true);  }
		if (Input.GetMouseButtonDown(2)) { HandleMouseAction(2, true);  }
		if (Input.GetMouseButtonUp(0))   { HandleMouseAction(0, false); }
		if (Input.GetMouseButtonUp(1))   { HandleMouseAction(1, false); }
		if (Input.GetMouseButtonUp(2))   { HandleMouseAction(2, false); }
	}
}