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
        // Properties
        // --------------------------------------------------
        // ----- Int
        public int Coin
        {
            get;
            private set;
        } = 0;

        public int Gem
        {
            get;
            private set;
        } = 0;

        // --------------------------------------------------
        // Functions - Nomal
        // --------------------------------------------------
        public void AddToCoin(int value) => Coin += value;
        public void AddToGem(int value)  => Gem  += value;
        public bool TryToConsumeCoin(int value)
        {
            if (Coin < value)
                return false;

            Coin -= value;
            return true;
        }

        public bool TryToConsumeGem(int value)
        {
            if (Gem < value)
                return false;

            Gem -= value;
            return true;
        }
    }
}