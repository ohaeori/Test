﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiResScreenShos_Real : MonoBehaviour
{
    private int resWidth = 1280;
    private int resHeight = 752;
    private int angle = 0;
    private int repeat = 0;

    private bool takeHiResShot = false;

    [SerializeField]
    private GameObject model;

    void Awake()
    {
        resWidth = Screen.width;
        resHeight = Screen.height;
    }

    private void Start()
    {
        moveChild(model);
    }

    public string ScreenShotName(int width, int height, string name)
    {
        return string.Format("{0}/{1}_{2}_{3}.png",
                             Application.dataPath + "/ScreenShot",
                             this.gameObject.name, name, Convert.ToString(angle) + "°");
    }

    public void TakeHiResShot()
    {
        takeHiResShot = true;
    }

    void ScreenShot(string objName)
    {
        takeHiResShot = false;

        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
        GetComponent<Camera>().targetTexture = rt;
        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        GetComponent<Camera>().Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        GetComponent<Camera>().targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        Destroy(rt);

        // 파일로 저장.
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = ScreenShotName(resWidth, resHeight, objName);
        System.IO.File.WriteAllBytes(filename, bytes);
        Debug.Log(string.Format("Took screenshot to: {0}", filename));

        //takeHiResShot |= Input.GetKeyDown("k");
        //if (takeHiResShot)
        //{

        //}
    }

    IEnumerator TimeRotation(GameObject target, float time, float yAngle, GameObject obj)
    {
        while (repeat<4)
        {
            yield return new WaitForSeconds(time);
            target.transform.Rotate(0, yAngle, 0);
            ScreenShot(obj.name);
            angle += 90;
            repeat++;
        }
    }

    void moveChild(GameObject obj)
    {
        if (obj.transform.position.y > 0)
            transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, obj.transform.position.z);
        else
            transform.position = new Vector3(obj.transform.position.x, 2, obj.transform.position.z);
        transform.rotation = new Quaternion(obj.transform.rotation.x, obj.transform.rotation.y, obj.transform.rotation.z, obj.transform.rotation.w);

        repeat = 0; angle = 0;
        StartCoroutine(TimeRotation(this.gameObject, 1f, 90f, obj));

        if (obj.transform.childCount != 0)
        {
            for (int i = 0; i < obj.transform.childCount; i++)
                moveChild(GameObject.Find(obj.transform.GetChild(i).name));
        }
    }
}