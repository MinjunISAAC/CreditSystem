// ----- C#
using System.Collections;
using System.Collections.Generic;

// ----- Unity
using UnityEngine;
using UnityEditor;

// ----- User Defined
using Utility.ForCredit.UI;
using Utility.ForData.UserSave;

namespace Utility.ForCredit 
{ 
    public class CreditSystem : MonoBehaviour
    {
        // --------------------------------------------------
        // Singleton
        // --------------------------------------------------
        // ----- Constructor
        private CreditSystem() { }

        // ----- Static Variables
        private static CreditSystem _instance = null;

        // ----- Variables
        private const string FILE_PATH = "CreditSystem.prefab";
        private bool _isSingleton = false;

        // ----- Property
        public static CreditSystem Instance
        {
            get
            {
                if (null == _instance)
                {
                    var existingInstance = FindObjectOfType<CreditSystem>();

                    if (existingInstance != null)
                    {
                        _instance = existingInstance;
                    }
                    else
                    {
                        var origin = Resources.Load<CreditSystem>(FILE_PATH);
                        _instance = Instantiate<CreditSystem>(origin);
                        _instance._isSingleton = true;
                        DontDestroyOnLoad(_instance.gameObject);
                    }
                }

                return _instance;
            }
        }

        // --------------------------------------------------
        // Components
        // --------------------------------------------------
        [Header("Canvas")]
        [SerializeField] private RectTransform _RECT_canvas    = null;
        
        [Space] [Header("Asset View Group")]
        [SerializeField] private CreditHudView _coinHubView    = null;
        [SerializeField] private CreditHudView _gemHubView     = null;

        // --------------------------------------------------
        // Functions - Event
        // --------------------------------------------------
        private void Awake()
        {
            // User Save Data Load
            UserSaveDataManager.Load();
            var userData = UserSaveDataManager.UserSaveData;

            if (null == _instance)
            {
                var existingInstance = FindObjectOfType<CreditSystem>();

                if (existingInstance != null)
                {
                    _instance = existingInstance;
                    DontDestroyOnLoad(_instance.gameObject);
                }
            }

            OnInit(userData.Coin, userData.Gem);
        }

        private void Update()
        {
            //Debug.Log($"width : {_RECT_canvas.sizeDelta.x} height : {_RECT_canvas.sizeDelta.y}");
        }

        private void OnDestroy()
        {
            if (_isSingleton)
                _instance = null;
        }


        // --------------------------------------------------
        // Functions - Nomal
        // --------------------------------------------------
        public void OnInit(int coinValue, int gemValue)
        {
            if (_coinHubView == null || _gemHubView == null)
            {
                Debug.LogError($"<color=red>[CreditSystem.OnInit] Credit들의 View가 존재하지 않는 View가 존재합니다.</color>");
                return;
            }

            _coinHubView.RefreshCreditValue(coinValue);
            _gemHubView. RefreshCreditValue(gemValue);

            _coinHubView.OnInit();
            _gemHubView. OnInit();
        }

        public void RefreshAsset()
        {
            _coinHubView.RefreshCreditValue(UserSaveDataManager.UserSaveData.Coin);
            _gemHubView. RefreshCreditValue(UserSaveDataManager.UserSaveData.Gem);
        }
    }
}