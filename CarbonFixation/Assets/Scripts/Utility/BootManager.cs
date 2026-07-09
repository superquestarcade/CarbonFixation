using Cysharp.Threading.Tasks;

namespace Utility
{
	public class BootManager : MonoBehaviourPlus
	{
		// public string MainMenuSceneName = "Menu";

		public void Awake()
		{
			// SceneTransition.TransitionSceneAsync(MainMenuSceneName).Forget();
			SceneTransition.Init();
		}
	}
}