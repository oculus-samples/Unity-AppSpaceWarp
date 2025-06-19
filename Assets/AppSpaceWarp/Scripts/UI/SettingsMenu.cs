// (c) Meta Platforms, Inc. and affiliates. Confidential and proprietary.


using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace AppSpaceWarp.UI
{
    public class SettingsMenu : MonoBehaviour
    {
        public bool IsASWEnabledAtStart;
        public bool IsBatonsEnabledAtStart;
        public Toggle EnableASWToggle;
        public Toggle EnableBatonsToggle;

        public Button SceneButtonPrefab;
        public RectTransform SceneButtonsParent;

        public GameObject CanvasMeshObject;
        private GameObject[] m_batons;
        private GameObject m_canvasObject;

        private void Awake()
        {
            m_canvasObject = GetComponentInChildren<Canvas>().gameObject;
            SetIsDisplaying(false);
        }

        private void Start()
        {

            m_batons = GameObject.FindGameObjectsWithTag("Baton");

            //need to forcibly call the setter functions, because they won't be called if this matches the serialized toggle value
            EnableASWToggle.isOn = IsASWEnabledAtStart;
            SetASWEnabled(IsASWEnabledAtStart);
            EnableBatonsToggle.isOn = IsBatonsEnabledAtStart;
            SetBatonsEnabled(IsBatonsEnabledAtStart);

            for (var i = SceneButtonsParent.childCount - 1; i >= 0; i--)
            {
                Destroy(SceneButtonsParent.GetChild(i).gameObject);
            }

            for (var i = 0; i < SceneManager.sceneCountInBuildSettings; ++i)
            {
                var button = Instantiate(SceneButtonPrefab, SceneButtonsParent);
                var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                var sceneName = scenePath[(scenePath.LastIndexOf('/') + 1)..];
                sceneName = sceneName[..sceneName.IndexOf(".")];
                button.GetComponentInChildren<TMPro.TMP_Text>().text = sceneName;
                button.onClick.AddListener(delegate { SceneManager.LoadScene(scenePath); });
            }
        }

        private void Update()
        {
            if (OVRInput.GetDown(OVRInput.RawButton.Start))
                SetIsDisplaying(!GetIsDisplaying());
        }

        public void SetASWEnabled(bool val)
        {
#if USING_OPENXR
            string openXrVersion = UnityEngine.XR.OpenXR.OpenXRRuntime.pluginVersion;
            if (int.Parse(openXrVersion.Split(".")[1]) >= 11) // AppSW requires OpenXR plugin >= 1.11.0
            {
#endif
            OVRManager.SetSpaceWarp(val);
#if USING_OPENXR
            }
#endif
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
}
