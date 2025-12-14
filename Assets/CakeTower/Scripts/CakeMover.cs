using System;
using Lucky.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace CakeTower.Scripts
{
    public class CakeMover : MonoBehaviour
    {
        [SerializeField] private Vector3 normalizedMoveDirection = Vector3.back;
        [SerializeField] private float startPositionH;
        [SerializeField] private float startPositionV;

        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private Vector3 targetPosition;
        [SerializeField] private CakeTower cakeTower;


        private void Start()
        {
            NextTurn();
        }

        public void NextTurn()
        {
            float startY = cakeTower.MaxLayerHeight + startPositionV;

            // switch direction
            if (normalizedMoveDirection == Vector3.back)
                normalizedMoveDirection = Vector3.left;
            else if (normalizedMoveDirection == Vector3.left)
                normalizedMoveDirection = Vector3.back;

            transform.position = -normalizedMoveDirection * startPositionH + Vector3.up * startY;

            targetPosition = normalizedMoveDirection * startPositionH + Vector3.up * startY;
        }

        private void Update()
        {
            if (GameManager.Instance.GameOver)
                return;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if (transform.position == targetPosition)
                targetPosition = new Vector3(-targetPosition.x, targetPosition.y, -targetPosition.z);
            // if (Input.GetKeyDown(KeyCode.LeftArrow))
            // MoveForward();
            // else if (Input.GetKeyDown(KeyCode.RightArrow))
            // MoveBackward();
        }

        private void MoveForward()
        {
            transform.position += normalizedMoveDirection;
        }

        private void MoveBackward()
        {
            transform.position -= normalizedMoveDirection;
        }
    }
}