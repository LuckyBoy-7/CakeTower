using System;
using System.Collections.Generic;
using Lucky.Utilities;
using UnityEngine;

namespace Lucky.Animation.SineWaveAnimation
{
    public class SineFloat : MonoBehaviour
    {
        [Serializable]
        public class SineData
        {
            public float speed;
            public float amount;

            public float randomSpeedDelta;
            public float randomAmountDelta;

            [HideInInspector] public float finalSpeed;
            [HideInInspector] public float finalAmount;
        }

        public Vector3 localAnchorPosition;
        public List<SineData> xSineDatas;
        public List<SineData> ySineDatas;

        private void Start()
        {
            localAnchorPosition = transform.localPosition;
            InitSineDatasWithRandom(xSineDatas);
            InitSineDatasWithRandom(ySineDatas);
        }

        public void InitSineDatasWithRandom(List<SineData> sineDatas)
        {
            for (var i = 0; i < sineDatas.Count; i++)
            {
                sineDatas[i].finalSpeed = sineDatas[i].speed + RandomUtils.NextSignedFloat() * sineDatas[i].randomSpeedDelta;
                sineDatas[i].finalAmount = sineDatas[i].amount + RandomUtils.NextSignedFloat() * sineDatas[i].randomAmountDelta;
            }
        }

        public float GetDeltaBySineDatas(List<SineData> sineDatas)
        {
            float res = 0;
            foreach (var sineData in sineDatas)
            {
                res += Mathf.Sin(Mathf.PI * 2 * sineData.finalSpeed * Time.time) * sineData.finalAmount;
            }

            return res;
        }

        private void FixedUpdate()
        {
            float deltaX = GetDeltaBySineDatas(xSineDatas);
            float deltaY = GetDeltaBySineDatas(ySineDatas);

            transform.localPosition = localAnchorPosition + new Vector3(deltaX, deltaY);
        }
    }
}