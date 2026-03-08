using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ritterkreuz.MainMenu
{
	public class MainMenuStateMachine : MonoBehaviour
	{
		[Header("Windows to show and hide")]
		[SerializeField] CanvasGroup mainMenuWindow = null;
		[SerializeField] CanvasGroup controlsMenuWindow = null;
		[SerializeField] CanvasGroup creditsMenuWindow = null;
		[SerializeField] float windowFadeTime = 0.2f;

		public enum WindowState { MainMenu, StartGame, Controls, Credits, ExitGame, None }
		public WindowState CurrentState { get; private set; } = WindowState.None;
		readonly Dictionary<WindowState, CanvasGroup> windowsMap = new();
		readonly HashSet<CanvasGroup> activeWindows = new();
		public event Action SettingsButtonColorCheck;

		void Awake()
		{
			InitTheActiveWindowList();
			LinkTheStateDictionary();
			ChangeState(WindowState.MainMenu, true);
		}



		void InitTheActiveWindowList()
		{
			mainMenuWindow.gameObject.SetActive(true);
			controlsMenuWindow.gameObject.SetActive(true);
			creditsMenuWindow.gameObject.SetActive(true);

			activeWindows.Add(mainMenuWindow);
			activeWindows.Add(controlsMenuWindow);
			activeWindows.Add(creditsMenuWindow);
		}

		void LinkTheStateDictionary()
		{
			windowsMap.Add(WindowState.MainMenu, mainMenuWindow);
			windowsMap.Add(WindowState.Controls, controlsMenuWindow);
			windowsMap.Add(WindowState.Credits, creditsMenuWindow);
		}

		public void InvokeButtonHitEvent(WindowState state)
		{
			ChangeState(state, false);
			SettingsButtonColorCheck?.Invoke();
		}

		void ChangeState(WindowState state, bool instant)
		{
			if (CurrentState == state) { return; }
			if (state == WindowState.ExitGame)
			{
#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
			}
			else if (state == WindowState.StartGame)
			{
				SceneManager.LoadScene(1);
			}

			CurrentState = state;
			if (windowsMap.TryGetValue(CurrentState, out CanvasGroup activeWindow))
			{
				CloseAllIrrelevantActiveWindows(instant);
				if (activeWindow != null)
				{
					StartCoroutine(FadeWindowIn(activeWindow, instant));
				}
			}
		}

		IEnumerator FadeWindowIn(CanvasGroup windowToFade, bool instant)
		{
			if (instant)
			{
				windowToFade.alpha = 1;
				yield return null;
			}
			else
			{
				while (windowToFade.alpha < 1)
				{
					windowToFade.alpha = Mathf.Min(windowToFade.alpha + Time.deltaTime / windowFadeTime, 1);
					yield return null;
				}
				activeWindows.Add(windowToFade);
				windowToFade.interactable = true;
				windowToFade.blocksRaycasts = true;
			}
		}

		IEnumerator FadeWindowOut(CanvasGroup windowToFade, bool instant)
		{
			windowToFade.interactable = false;
			windowToFade.blocksRaycasts = false;
			activeWindows.Remove(windowToFade);
			if (instant)
			{
				windowToFade.alpha = 0;
				yield return null;
			}
			else
			{
				while (windowToFade.alpha > 0)
				{
					windowToFade.alpha = Mathf.Max(windowToFade.alpha - Time.deltaTime / windowFadeTime, 0);
					yield return null;
				}
			}
		}

		void CloseAllIrrelevantActiveWindows(bool instant)
		{
			List<CanvasGroup> windowsToClose = new();
			foreach (CanvasGroup window in activeWindows)
			{
				if (window == null) { Debug.LogError("Some window was not assigned to main menu state machine!"); }
				if (window != windowsMap[CurrentState])
				{
					windowsToClose.Add(window);
				}
			}

			foreach (CanvasGroup window in windowsToClose)
			{
				if (window != null)
				{
					StartCoroutine(FadeWindowOut(window, instant));
				}
			}
		}
	}
}
