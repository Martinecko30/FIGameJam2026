using UnityEngine;

public class InputFocus : MonoBehaviour
{
        private void Awake()
		{
			FocusTheGameWindow();
			ToggleCursor(false);
		}

        private static void ToggleCursor(bool value)
		{
			if (value)
			{
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.Confined;

			}
			else
			{
				Cursor.visible = false;
				Cursor.lockState = CursorLockMode.Locked;
			}
		}

        private static void FocusTheGameWindow()
		{
#if UNITY_EDITOR
			var gameViewWindow = UnityEditor.EditorWindow.GetWindow(typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.GameView"));
			gameViewWindow.Focus();
			Event focusMouseClick = new()
			{
				button = 0,
				clickCount = 1,
				type = EventType.MouseDown,
				mousePosition = gameViewWindow.rootVisualElement.contentRect.center
			};
			gameViewWindow.SendEvent(focusMouseClick);
#endif
		}
}
