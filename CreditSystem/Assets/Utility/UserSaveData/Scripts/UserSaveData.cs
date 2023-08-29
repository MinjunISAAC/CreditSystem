// ----- C#
using System;

// ----- Unity
using UnityEngine;

// ----- User Defined
//using Utility.ForAsset;

namespace Utility.ForData.UserSave
{
    [Serializable]
    public class UserSaveData
    {
        // --------------------------------------------------
        // Components
        // --------------------------------------------------
        [SerializeField] private int _coin = 0;
        [SerializeField] private int _gem  = 0;

        // --------------------------------------------------
        // Properties
        // --------------------------------------------------
        // ----- Int
        public int Coin => _coin;

        public int Gem  => _gem;

        // --------------------------------------------------
        // Functions - Nomal
        // --------------------------------------------------
        public void AcquireToCoin(int value) => _coin += value;
        public void AcquireToGem(int value)  => _gem += value;
        public bool TryToConsumeCoin(int value)
        {
            if (_coin < value)
                return false;

            _coin -= value;
            return true;
        }

        public bool TryToConsumeGem(int value)
        {
            if (_gem < value)
                return false;

            _gem -= value;
            return true;
        }
    }
}