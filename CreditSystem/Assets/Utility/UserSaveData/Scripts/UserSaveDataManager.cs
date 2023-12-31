// ----- C#
using System;
using System.IO;

// ----- Unity
using UnityEngine;
using UnityEditor;
using Utility.ForCredit;

// ----- User Defined

namespace Utility.ForData.UserSave
{
    public static class UserSaveDataManager
    {
        // --------------------------------------------------
        // Variables
        // --------------------------------------------------
        // ----- Const
        private const string FILE_NAME = "UserSaveData.json";

        // --------------------------------------------------
        // Properties
        // --------------------------------------------------
        public static UserSaveData UserSaveData
        {
            get;
            private set;
        } = new UserSaveData();

        // --------------------------------------------------
        // Functions - Nomal
        // --------------------------------------------------
        // ----- Public
        public static void Load()
        {
            if (!_TryLoad(FILE_NAME, out string fileContents))
            {
                UserSaveData = new UserSaveData();
                return;
            }

            try
            {
                var pendData = JsonUtility.FromJson<UserSaveData>(fileContents);
                if (pendData == null)
                {
                    Debug.LogError($"[UserSaveDataManager.Load] {FILE_NAME} 파일을 로드하는데 실패했습니다.");
                    return;
                }

                UserSaveData = pendData;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return;
            }

            return;
        }

        public static void Save()
        {
            var jsonContents = JsonUtility.ToJson(UserSaveData);

            if (!_TrySave(FILE_NAME, jsonContents))
            {
                Debug.LogError($"[UserSaveDataManager.Save] File을 저장하지 못했습니다.");
                return;
            }
        }

        // ----- Data
        public static void AcquireToCoin(int value)
        {
            int prevCredit = UserSaveDataManager.UserSaveData.Coin;

            UserSaveData.AcquireToCoin(value);
            CreditSystem.Instance.RefreshFillCredit(ECreditType.Coin, value);
            Save();
        }

        public static void AcquireToGem(int value)
        {
            int prevCredit = UserSaveDataManager.UserSaveData.Gem;

            UserSaveData.AcquireToGem(value);
            CreditSystem.Instance.RefreshFillCredit(ECreditType.Gem, value);
            Save();
        } 

        public static void ConsumeCoin(int value)
        {
            if (UserSaveData.TryToConsumeCoin(value))
            {
                Save();
                CreditSystem.Instance.RefreshCredit();
                return;
            }

            // [TODO] Toast Message Show
        }

        public static void ConsumeGem(int value)
        {
            if (UserSaveData.TryToConsumeGem(value))
            {
                Save();
                CreditSystem.Instance.RefreshCredit();
                return;
            }

            // [TODO] Toast Message Show
        }

        // ----- Private
        private static bool _TryLoad(string fileName, out string fileContents)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                fileContents = string.Empty;
                return false;
            }

            var filePath = $"{Application.persistentDataPath}/{fileName}";

            if (!File.Exists(filePath))
            {
                fileContents = string.Empty;
                return false;
            }

            try
            {
                fileContents = File.ReadAllText(filePath);

                return true;
            }
            catch (Exception e)
            {
                fileContents = $"{e}";
                return false;
            }
        }

        private static bool _TrySave(string fileName, string saveDataContents)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                Debug.LogError("UserSaveDataManager.Save] 파일명이 비어있습니다.");
                return false;
            }

            if (UserSaveData == null)
            {
                Debug.LogError($"[UserSaveDataManager.Save] User Save Data가 생성되지 않았습니다.");
                return false;
            }

            if (string.IsNullOrEmpty(saveDataContents))
            {
                Debug.LogWarning("[UserSaveDataManager.Save] 저장할 컨텐츠가 비어있습니다.");
                return false;
            }

            try
            {
                var fileContents = JsonUtility.ToJson(UserSaveData);

                var filePath = $"{Application.persistentDataPath}/{fileName}";

                try
                {
                    fileContents = saveDataContents;
                    File.WriteAllText(filePath, fileContents);
                    return true;
                }
                catch (Exception e)
                {
                    Debug.LogError($"<color=red>[UserSaveDataManager._TrySave] {e}</color>");
                    return false;
                }
            }
            catch (Exception e)
            {
                Debug.Log($"<color=red>[UserSaveDataManager._TrySave] {e}</color>");
                return false;
            }
        }

#if UNITY_EDITOR
        [MenuItem("UserData/Delete User Save Data")]
        private static void ClearUserSaveData()
        {
            string filePath = $"{Application.persistentDataPath}/{FILE_NAME}";

            if (File.Exists(filePath)) File.Delete(filePath);
        }
#endif
    }
}