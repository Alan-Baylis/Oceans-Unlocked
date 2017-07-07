using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class AsyncLoader : MonoBehaviour {

  public Slider slider;

  public void LoadLevel(int sceneIndex) {
    StartCoroutine(LoadAsync(sceneIndex));
  }

  IEnumerator LoadAsync (int sceneIndex) {
    AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
    while (!operation.isDone) {
      float progress = Mathf.Clamp01(operation.progress / .9f);
      Debug.Log(progress);
      slider.value = progress;
      yield return null;
    }
  }

}
