using UnityEngine;
using System.Collections;

public class LoadOnAwake : MonoBehaviour {

  public AsyncLoader asyncLoader;
  public int sceneIndex;

  void Start() {
    asyncLoader = GetComponent<AsyncLoader>();
    LoadGame(sceneIndex);
  }


  void LoadGame(int sceneIndex) {
    print("Test");
    asyncLoader.LoadLevel(sceneIndex);
  }
}
