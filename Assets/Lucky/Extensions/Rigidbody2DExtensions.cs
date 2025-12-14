// using UnityEngine;
//
// namespace Lucky.Extensions
// {
//     public static class Rigidbody2DExtensions
//     {
//         public static void SetSpeedX(this Rigidbody2D orig, float x)
//         {
//             orig.linearVelocity = orig.linearVelocity.WithX(x);
//         }
//
//         public static void SetSpeedY(this Rigidbody2D orig, float y)
//         {
//             orig.linearVelocity = orig.linearVelocity.WithY(y);
//         }
//
//         public static void AddSpeedX(this Rigidbody2D orig, float x)
//         {
//             orig.linearVelocity = orig.linearVelocity.WithX(orig.linearVelocity.x + x);
//         }
//
//         public static void AddSpeedY(this Rigidbody2D orig, float y)
//         {
//             orig.linearVelocity = orig.linearVelocity.WithY(orig.linearVelocity.y + y);
//         }
//         
//         public static void MulSpeedX(this Rigidbody2D orig, float k)
//         {
//             orig.linearVelocity = orig.linearVelocity.WithX(orig.linearVelocity.x * k);
//         }
//         
//         public static void MulSpeedY(this Rigidbody2D orig, float k)
//         {
//             orig.linearVelocity = orig.linearVelocity.WithY(orig.linearVelocity.y * k);
//         }
//     }
// }