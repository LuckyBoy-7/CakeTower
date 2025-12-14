using System;
using TMPro;
using UnityEngine;

namespace CakeTower.Scripts
{
    public class PerfectPlacementDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;

        private void Update()
        {
            GameManager gameManager = GameManager.Instance;
            text.text = $"{gameManager.perfectPlacement}/{gameManager.perfectPlacementStreakMax}";
        }
    }
}