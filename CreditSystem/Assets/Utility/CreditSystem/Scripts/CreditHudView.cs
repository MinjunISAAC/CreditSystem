// ----- C#
using System;
using System.Collections;
using System.Collections.Generic;

// ----- Unity
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Utility.ForCredit.UI
{
    public class CreditHudView : MonoBehaviour
    {
        // --------------------------------------------------
        // Enum
        // --------------------------------------------------
        public enum ESpreadCount
        {
            Unknown = 0,
            Low     = 1,
            Medium  = 2,
            High    = 3,
        }

        public enum ESpreadDistance
        {
            Unknown = 0,
            Low     = 1,
            Medium  = 2,
            High    = 3,
        }

        public enum ESpreadSpeed
        {
            Unknown = 0,
            Low     = 1,
            Medium  = 2,
            High    = 3,
        }

        public enum EAbsorbSpeed
        {
            Unknown = 0,
            Low     = 1,
            Medium  = 2,
            High    = 3,
        }

        // --------------------------------------------------
        // Components
        // --------------------------------------------------
        [SerializeField] private Button                   _BTN_Test        = null;


        [Header("UI")]
        [SerializeField] private CanvasGroup              _canvasGroup     = null;
        [SerializeField] private RectTransform            _RECT_canvas     = null;
        [SerializeField] private RectTransform            _RECT_showPoint  = null;
        [SerializeField] private RectTransform            _RECT_hidePoint  = null;

        [Space] [Header("재화값")]
        [SerializeField] private TextMeshProUGUI          _TMP_value       = null;
        
        [Space] [Header("파티클 정보")]
        [SerializeField] private List<CreditParticleUnit> _particleList    = null;

        [Header("파티클 옵션")]
        [SerializeField] private ESpreadCount             _spreadCount     = ESpreadCount.Unknown;
        [SerializeField] private ESpreadDistance          _spreadDistance  = ESpreadDistance.Unknown;
        [SerializeField] private ESpreadSpeed             _spreadSpeed     = ESpreadSpeed.Unknown;
        [SerializeField] private EAbsorbSpeed             _absorbSpeed     = EAbsorbSpeed.Unknown;
        [SerializeField] private bool                     _spreadDelay     = false;
        [Range(0f, 0.01f)] 
        [SerializeField] private float                    _delayTime       = 0.05f;

        // --------------------------------------------------
        // Variables
        // --------------------------------------------------
        // ----- Value Group
        private const int   MAX_ANGLE           = 1000000;
        private const float MAX_SPREAD_SEC      = 0.2f;
        private const float MIN_SPREAD_SEC      = 0.1f;
        private const float MAX_ABSORB_SEC      = 0.225f;
        private const float MIN_ABSORB_SEC      = 0.125f;
        private const float MAX_DISTANCE_FACTOR = 4f;
        private const float MIN_DISTANCE_FACTOR = 10f;

        private float _maxDistance = 0f;
        private float _minDistance = 0f;

        // ----- Coroutine 
        private Coroutine   _co_particle = null;

        // --------------------------------------------------
        // Functions - Nomal
        // --------------------------------------------------
        // ----- Public
        public void OnInit()
        {
            _maxDistance    = _RECT_canvas.rect.width / MAX_DISTANCE_FACTOR;
            _minDistance    = _RECT_canvas.rect.width / MIN_DISTANCE_FACTOR;

            _canvasGroup.alpha = 0;
        
            // TEST
            _BTN_Test.onClick.AddListener(() => { SpreadParticle(_spreadCount, _spreadDistance, _spreadSpeed, _absorbSpeed, null); });
        }

        public void RefreshCreditValue(int value, bool isMax = false)
        {
            if (isMax)
            {
                _TMP_value.text = $"MAX";
                return;
            }

            _TMP_value.text = _FormatAssetValue(value);
        }

        public void SpreadParticle(ESpreadCount spreadCountType, ESpreadDistance spreadDistanceType, ESpreadSpeed spreadSpeedType, EAbsorbSpeed apsorbSpeedType, Action doneCallBack) 
        {
            if (_co_particle == null)
            {
                _co_particle = StartCoroutine(_Co_Spread(spreadCountType, spreadDistanceType, spreadSpeedType, apsorbSpeedType, doneCallBack));
                return;
            }

            StopCoroutine(_co_particle);
            _co_particle = StartCoroutine(_Co_Spread(spreadCountType, spreadDistanceType, spreadSpeedType, apsorbSpeedType, doneCallBack));
        }

        // ----- Private
        private string _FormatAssetValue(int value)
        {
            string[] units          = { "", "K", "M", "G", "T", "P", "E", "Z", "Y" };
            string   formattedValue = $"{value:0.##}";

            if (value >= 1000)
            {
                int orderOfMagnitude = (int)Math.Floor(Math.Log10(value));
                int unitIndex        = orderOfMagnitude / 3;

                if (unitIndex < units.Length)
                    formattedValue = $"{(value / Math.Pow(10, unitIndex * 3)):0.##}{units[unitIndex]}";
            }

            return formattedValue;
        }

        // --------------------------------------------------
        // Functions - Coroutine
        // --------------------------------------------------
        private IEnumerator _Co_Spread(ESpreadCount spreadCountType, ESpreadDistance spreadDistanceType, ESpreadSpeed spreadSpeedType, EAbsorbSpeed absorbSpeedType, Action doneCallBack) 
        {
            int ReturnSpreadCount(ESpreadCount type)
            {
                switch (type) 
                {
                    case ESpreadCount.High:   return _particleList.Count;
                    case ESpreadCount.Low:    return _particleList.Count / 3;
                    default:                  return _particleList.Count / 2;
                }
            }

            float ReturnSpreadDistance(ESpreadDistance type)
            {
                switch (type)
                {
                    case ESpreadDistance.High: return _maxDistance;
                    case ESpreadDistance.Low:  return _minDistance;
                    default:                   return (_maxDistance + _minDistance) / 2f;
                }
            }

            float ReturnSpreadSec(ESpreadSpeed type)
            {
                switch (type)
                {
                    case ESpreadSpeed.High: return MIN_SPREAD_SEC;
                    case ESpreadSpeed.Low:  return MAX_SPREAD_SEC;
                    default:                return (MIN_SPREAD_SEC + MAX_SPREAD_SEC) / 2f;
                }
            }

            float ReturnAbsorbSec(EAbsorbSpeed type)
            {
                switch (type)
                {
                    case EAbsorbSpeed.High: return MIN_ABSORB_SEC;
                    case EAbsorbSpeed.Low:  return MAX_ABSORB_SEC;
                    default:                return (MIN_ABSORB_SEC + MAX_ABSORB_SEC) / 2f;
                }
            }

            var centerPos     = _RECT_showPoint.anchoredPosition;
            var endPos        = _RECT_hidePoint.anchoredPosition;
            var count         = ReturnSpreadCount(spreadCountType);
            var moveDistance  = ReturnSpreadDistance(spreadDistanceType);
            var moveSpreadSec = ReturnSpreadSec(spreadSpeedType);
            var moveAbsorbSec = ReturnAbsorbSec(absorbSpeedType);

            for (int i = 0; i < count; i++) 
            { 
                var particleUnit   = _particleList[i];
                particleUnit.gameObject.SetActive(true);
                
                var randomAngle    = UnityEngine.Random.Range(-MAX_ANGLE, MAX_ANGLE);
                var randomDistance = UnityEngine.Random.Range(3f, moveDistance);
                var targetPosition = new Vector2(centerPos.x + Mathf.Cos(Mathf.PI / 180 * randomAngle) * randomDistance,
                                                 centerPos.y + Mathf.Sin(Mathf.PI / 180 * randomAngle) * randomDistance);
                
                particleUnit.Move
                (
                    centerPos, 
                    targetPosition, 
                    moveSpreadSec, 
                    () => 
                    {
                        particleUnit.Absorb(_RECT_hidePoint.anchoredPosition, moveAbsorbSec, () => particleUnit.gameObject.SetActive(false));
                    }
                );

                if (_spreadDelay) yield return new WaitForSeconds(_delayTime);
            }

            doneCallBack?.Invoke();
            yield return null;
        }
    }
}