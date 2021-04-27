using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShotHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject mainCanvas;
    [SerializeField]
    private GameObject captureCanvas;

    public void OnTakeScreenShot()
    {
        StartCoroutine(TakeScreenshot());
    }
    private IEnumerator TakeScreenshot()
    {
        SetUIActive(false);

        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();
        NativeGallery.SaveImageToGallery(ss, "Native Screenshots", DateTime.Now.ToString().Replace("/", "-"));

        SetUIActive(true);
    }
    public void SetUIActive(bool boo)
    {
        mainCanvas.SetActive(boo);
        captureCanvas.SetActive(!boo);
    }

}
