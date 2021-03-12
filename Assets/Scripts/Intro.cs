using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    public string defaultScene;
    async void Start()
    {
        await Task.Delay(1);
        var saveManager = new SaveManager();
        if (saveManager.data.locationFlags.Count > 0)
            SceneManager.LoadScene(saveManager.data.locationFlags[saveManager.data.locationFlags.Count - 1]);
        else
            SceneManager.LoadScene(defaultScene);
    }
}
