using System;
using System.Collections;
using DG.Tweening;
using Lucky.Audio;
using Lucky.Extensions;
using UnityEngine;
using UnityEngine.UI;
using Ease = Lucky.Utilities.Ease;

namespace CakeTower.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        [SerializeField] private CakeMover mover;
        [SerializeField] private CakeTower cakeTower;
        [SerializeField] private CakePlacer cakePlacer;
        [SerializeField] private UIFader uiFader;
        [SerializeField] private Transform cameraAxis;
        [SerializeField] private Image blackImage;

        public int perfectPlacement;
        public int perfectPlacementStreakMax;
        public bool GameOver { get; private set; }

        private void Awake()
        {
            Instance = this;
            blackImage.DOColor(Color.black.WithA(0), 2f);
        }

        private void OnEnable()
        {
            cakeTower.OnPlacement += OnPerfectPlacement;
        }

        private void OnDisable()
        {
            cakeTower.OnPlacement -= OnPerfectPlacement;
        }

        private void OnPerfectPlacement(CakeTower.PlacementResult result)
        {
            if (result == CakeTower.PlacementResult.Failed)
            {
                AudioController.Instance.PlaySound2D("explosion");
                BeginGameOver();


                return;
            }

            mover.NextTurn();
            if (result == CakeTower.PlacementResult.Normal)
            {
                cakePlacer.Restart();
                perfectPlacement = 0;
                AudioController.Instance.PlaySound2D("hit");
                return;
            }

            AudioController.Instance.PlaySound2D("perfect_hit");
            perfectPlacement += 1;
            if (perfectPlacement >= perfectPlacementStreakMax)
            {
                cakePlacer.GetPerfectPlacementStreakBonus();
            }
            else
            {
                cakePlacer.Restart();
            }
        }

        public void BeginGameOver()
        {
            GameOver = true;
            StartCoroutine(GameOverCoroutine());
            StartCoroutine(RotateCameraRoutine());
        }

        private IEnumerator RotateCameraRoutine()
        {
            while (true)
            {
                cameraAxis.Rotate(Vector3.up, 30f * Time.deltaTime);
                yield return null;
            }
        }

        private IEnumerator GameOverCoroutine()
        {
            uiFader.FadeOut();
            yield return new WaitForSeconds(0.5f);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

            blackImage.DOColor(Color.black, 2);
            yield return new WaitForSeconds(2f);
            RestartScene();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartScene();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        private static void RestartScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
    }
}