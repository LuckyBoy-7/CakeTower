using UnityEngine;

namespace Lucky.Console.Scripts
{
    public class CommandTest : MonoBehaviour
    {
        public enum ConsoleCommandExampleEnums
        {
            A,
            B,
            C
        }

        [Command("test.empty", "This will output an empty test message.")]
        public static void TestFuction1()
        {
            Debug.Log("Test(Empty)");
        }

        [Command("test.int")]
        public static void TestFuction1(int i)
        {
            Debug.Log("Test(int): " + i);
        }

        [Command("test.float")]
        public static void TestFuction1(float f)
        {
            Debug.Log("Test(float): " + f);
        }

        [Command("test.bool")]
        public static void TestFuction1(bool b)
        {
            Debug.Log("Test(bool): " + b);
        }


        [Command("test.enum")]
        public static void TestFuction1(ConsoleCommandExampleEnums e)
        {
            Debug.Log("Test(Enum): " + e);
        }

        // =============================================


        [Command("scale")]
        public void Scale()
        {
            transform.localScale = Vector3.one * 3;
        }

        [Command("test")]
        public static void TestFuction()
        {
            Debug.Log("Test(Empty)");
        }

        [Command("test")]
        public static void TestFuction(int i)
        {
            Debug.Log("Test(int): " + i);
        }

        // [ConsoleCommand("test")]
        // public static void TestFuction(string s)
        // {
        //     Debug.Log("Test(string): " + s);
        // }

        [Command("test")]
        public static void TestFuction(float f)
        {
            Debug.Log("Test(float): " + f);
        }

        [Command("test")]
        public static void TestFuction(bool b)
        {
            Debug.Log("Test(bool): " + b);
        }


        [Command("test")]
        public static void TestFuction(ConsoleCommandExampleEnums e)
        {
            Debug.Log("Test(Enum): " + e);
        }
    }
}