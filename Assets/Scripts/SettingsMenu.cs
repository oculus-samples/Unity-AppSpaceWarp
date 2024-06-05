// Copyright (c) Meta Platforms, Inc. and affiliates.

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public bool IsASWEnabledAtStart;
    public bool IsBatonsEnabledAtStart;
    public Toggle EnableASWToggle;
    public Toggle EnableBatonsToggle;

    public Button SceneButtonPrefab;
    public RectTransform SceneButtonsParent;

    public GameObject CanvasMeshObject;

    GameObject[] m_batons;
    GameObject m_canvasObject;

    void Awake()
    {
        m_canvasObject = GetComponentInChildren<Canvas>().gameObject;
        SetIsDisplaying(false);
    }

    void Start()
    {

        m_batons = GameObject.FindGameObjectsWithTag("Baton");
        
        //need to forcibly call the setter functions, because they won't be called if this matches the serialized toggle value
        EnableASWToggle.isOn = IsASWEnabledAtStart;
        SetASWEnabled(IsASWEnabledAtStart);
        EnableBatonsToggle.isOn = IsBatonsEnabledAtStart;
        SetBatonsEnabled(IsBatonsEnabledAtStart);

        for (int i = SceneButtonsParent.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(SceneButtonsParent.GetChild(i).gameObject);
        }

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; ++i)
        {
            var button = GameObject.Instantiate<Button>(SceneButtonPrefab, SceneButtonsParent);
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = scenePath.Substring(scenePath.LastIndexOf('/')+ 1);
            sceneName = sceneName.Substring(0, sceneName.IndexOf("."));
            button.GetComponentInChildren<TMPro.TMP_Text>().text = sceneName;
            button.onClick.AddListener(delegate { SceneManager.LoadScene(scenePath); });
        }
    }

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.Start))
            SetIsDisplaying(!GetIsDisplaying());
    }

    public void SetASWEnabled(bool val)
    {
        OVRManager.SetSpaceWarp(val);
    }

    public void SetBatonsEnabled(bool val)
    {
        foreach (var baton in m_batons)
        {
            baton.SetActive(val);
        }
    }

    public bool GetIsDisplaying()
    {
        return m_canvasObject.activeSelf;
    }

    public void SetIsDisplaying(bool val)
    {
        m_canvasObject.SetActive(val);
        CanvasMeshObject.SetActive(val);

    }
}
