using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utility.ForCredit;
using Utility.ForData.UserSave;

namespace Test
{
    public class TestView : MonoBehaviour
    {
        // --------------------------------------------------
        // Components
        // --------------------------------------------------
        [SerializeField] private Button _BTN_AcquireToCoin = null;
        [SerializeField] private Button _BTN_ConsumeToCoin = null;

        [SerializeField] private Button _BTN_AcquireToGem  = null;
        [SerializeField] private Button _BTN_ConsumeToGem  = null;

        // --------------------------------------------------
        // Functions - Event
        // --------------------------------------------------
        private void Start()
        {
            UserSaveDataManager.Load();

            Debug.Log($"Coin : {UserSaveDataManager.UserSaveData.Coin} / Gem : {UserSaveDataManager.UserSaveData.Gem}");
            
            _BTN_AcquireToCoin.onClick.AddListener(() => UserSaveDataManager.AcquireToCoin(100));
            _BTN_ConsumeToCoin.onClick.AddListener(() => UserSaveDataManager.ConsumeCoin(100));

            _BTN_AcquireToGem.onClick.AddListener(() => UserSaveDataManager.AcquireToGem(100));
            _BTN_ConsumeToGem.onClick.AddListener(() => UserSaveDataManager.ConsumeGem(100));
        }
    }
}