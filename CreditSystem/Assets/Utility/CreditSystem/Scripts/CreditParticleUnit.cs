// ----- C#
using System;
using System.Collections;
using System.Collections.Generic;

// ----- Unity
using UnityEngine;

namespace Utility.ForCredit.UI
{
    public class CreditParticleUnit : MonoBehaviour
    {
        // --------------------------------------------------
        // Components
        // --------------------------------------------------
        [SerializeField] private RectTransform _rectTransform = null;

        // --------------------------------------------------
        // Variables
        // --------------------------------------------------
        private Coroutine _coMove   = null;
        private Coroutine _coAbsorb = null;

        // --------------------------------------------------
        // Properties
        // --------------------------------------------------
        public RectTransform RectTransform => _rectTransform;

        // --------------------------------------------------
        // Functions - Nomal
        // --------------------------------------------------
        public void Move(Vector2 startVec, Vector2 endVec, float duration, Action doneCallBack = null) 
        { 
            if (_coMove == null)
            {
                _coMove = StartCoroutine(_Co_Move(startVec, endVec, duration, doneCallBack));
                return;
            }

            StopCoroutine(_coMove);
            _coMove = StartCoroutine(_Co_Move(startVec, endVec, duration, doneCallBack));
        }

        public void Absorb(Vector2 endVec, float duration, Action doneCallBack = null)
        {
            if (_coAbsorb == null)
            {
                _coAbsorb = StartCoroutine(_Co_Absorb(endVec, duration, doneCallBack));
                return;
            }

            StopCoroutine(_coAbsorb);
            _coAbsorb = StartCoroutine(_Co_Absorb(endVec, duration, doneCallBack));
        }

        // --------------------------------------------------
        // Functions - Coroutine
        // --------------------------------------------------
        private IEnumerator _Co_Move(Vector2 startVec, Vector2 endVec, float duration, Action doneCallBack) 
        {
            var sec      = 0.0f;
                
            while (sec < duration) 
            {
                sec += Time.deltaTime;

                _rectTransform.anchoredPosition = Vector2.Lerp(startVec, endVec, sec / duration );
                yield return null;
            }

            _rectTransform.anchoredPosition = endVec;

            doneCallBack?.Invoke();
        }
        private IEnumerator _Co_Absorb(Vector2 endVec, float duration, Action doneCallBack)
        {
            yield return new WaitForSeconds(0.25f);

            var sec      = 0.0f;
            var startVec = _rectTransform.anchoredPosition;

            while (sec < duration)
            {
                sec += Time.deltaTime;

                _rectTransform.anchoredPosition = Vector2.Lerp(startVec, endVec, sec / duration);
                yield return null;
            }

            _rectTransform.anchoredPosition = endVec;

            doneCallBack?.Invoke();
        }
    }
}