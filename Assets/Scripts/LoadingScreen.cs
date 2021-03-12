using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour, ISave
{
    static string asTextCat;
    static string asTextEntry;
    static int alRandomNum;
    static string asImageFile = "";
    static string actualText;
    public static AudioClip clip;
    static DateTime time;
    public Text text;
    public Image image;
    public static LoadingScreen instance;
    const float yPos = -251.19f;
    float totalTime;
    public static string newPlayerPosition;
    bool isSecondEnabling;
    private void OnEnable()
    {
        instance = this;
        if (isSecondEnabling)
        {
            actualText = LangAdapter.FindEntry(asTextCat, asTextEntry + "0" + UnityEngine.Random.Range(1, alRandomNum + 1));
            time = DateTime.Now;
        }
        isSecondEnabling = true;
        text.text = actualText;
        image.sprite = Resources.Load<Sprite>("loadingscreens/" + asImageFile.Replace(".jpg",""));

        if (string.IsNullOrEmpty(actualText))
        {
            gameObject.SetActive(false);
            PlayerController.instance.playerBody.GetComponent<Collider>().enabled = true;
            PlayerController.instance.fader.FadeOff(3);
        }
        else
        {
            PlayerController.instance.fader.FadeOn(Color.black, 0);
            Inventory.SetInventoryDisabled(true);
            text.GetComponent<Fader>().FadeOn(Color.white, 0);
            text.rectTransform.anchoredPosition = new Vector2(text.rectTransform.anchoredPosition.x, yPos);
            totalTime = text.text.Length * 0.05f;
            PlayerController.instance.FreezeMovement = true;
            PlayerController.instance.FreezeRotation = true;
            StartCoroutine(MoveText());
        }
    }

    // Update is called once per frame
    public static void SetupLoadingScreen(string asTextCat, string asTextEntry, int alRandomNum, string asImageFile)
    {
        LoadingScreen.asTextCat = asTextCat;
        LoadingScreen.asTextEntry = asTextEntry;
        LoadingScreen.alRandomNum = alRandomNum;
        LoadingScreen.asImageFile = asImageFile;
    }
    IEnumerator MoveText()
    {
        yield return null;
        var pos = new Vector2(text.rectTransform.anchoredPosition.x, 0);
        float asMin = 3;
        Debug.Log("Loading time: " + (DateTime.Now - time).TotalSeconds);
        while ((DateTime.Now - time).TotalSeconds < totalTime && !Input.GetMouseButton(0) || asMin > 0)
        { 
            text.rectTransform.anchoredPosition = Vector2.Lerp(text.rectTransform.anchoredPosition, pos, Time.deltaTime);
            asMin -= Time.deltaTime;
            yield return null;
        }
        text.GetComponent<Fader>().FadeOff();
        yield return new WaitForSeconds(1);
        PlayerController.instance.FreezeMovement = false;
        PlayerController.instance.FreezeRotation = false;
        Inventory.SetInventoryDisabled(false);
        PlayerController.instance.playerBody.GetComponent<Collider>().enabled = true;
        Scenario.currentScenario.TeleportPlayer(newPlayerPosition);
        PlayerController.instance.fader.FadeOff(3);
        SoundManager.PlayClip(clip);
        gameObject.SetActive(false);
    }

    public void OnLoad(Data data)
    {
        data.StringKeys.TryGetValue("LoadingScreen.asTextCat", out asTextCat, string.Empty);
        data.StringKeys.TryGetValue("LoadingScreen.asTextEntry", out asTextEntry, string.Empty);
        data.IntKeys.TryGetValue("LoadingScreen.alRandomNum", out alRandomNum, 0);
        data.StringKeys.TryGetValue("LoadingScreen.asImageFile", out asImageFile, string.Empty);
    }

    public void OnSave(Data data)
    {
        data.StringKeys.SetValueSafety("LoadingScreen.asTextCat", asTextCat);
        data.StringKeys.SetValueSafety("LoadingScreen.asTextEntry", asTextEntry);
        data.IntKeys.SetValueSafety("LoadingScreen.alRandomNum", alRandomNum);
        data.StringKeys.SetValueSafety("LoadingScreen.asImageFile", asImageFile);
    }
}
