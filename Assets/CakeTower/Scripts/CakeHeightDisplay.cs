using System;
using TMPro;
using UnityEngine;

namespace CakeTower.Scripts
{
    public class CakeHeightDisplay : MonoBehaviour
    {
        [SerializeField] private CakeTower cakeTower;
        [SerializeField] private TMP_Text text;

        private void Update()
        {
            text.text = $"{cakeTower.MaxLayer}";
        }
    }
}