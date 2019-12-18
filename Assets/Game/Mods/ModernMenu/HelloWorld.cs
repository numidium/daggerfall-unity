using UnityEngine;
using System.Collections;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows; //required for pop-up window
using DaggerfallWorkshop.Game.Utility.ModSupport;   //required for modding features

namespace DaggerfallModdingTutorials.Part2
{
    public class HelloWorld : MonoBehaviour
    {
        static DaggerfallMessageBox helloWorldMessage;
        public bool check = false;

        IEnumerator Start()
        {
            //delay for a few seconds then display the Hello World message
            yield return new WaitForSeconds(2);
            //push the message box
            DisplayMessage();
        }


        void Update()
        {
            //display another message when the player dies
            //open the console (~) and type suicide to test
            if (GameManager.Instance.StateManager.CurrentState == StateManager.StateTypes.Death)
            {
                if (check)
                {
                    check = false;
                    DisplayMessage("hahahaha, you died.");
                }
            }
            else if (check == false)
                check = true;
        }

        //this method will be called automatically by the modmanager after the main game scene is loaded.
        //The following requirements must be met to be invoked automatically by the ModManager during setup for this to happen:
        //1. Marked with the [Invoke] custom attribute
        //2. Be public & static class method
        //3. Take in an InitParams struct as the only parameter
        [Invoke(StateManager.StateTypes.Game, 0)]
        public static void Init(InitParams initParams)
        {
            Debug.Log("main init");

            //just an example of how to add a mono-behavior to a scene.
            GameObject helloGo = new GameObject("hello");
            HelloWorld hello = helloGo.AddComponent<HelloWorld>();

            //after finishing, set the mod's IsReady flag to true.
            ModManager.Instance.GetMod(initParams.ModTitle).IsReady = true;
        }

        //You can have as many of these methods as you want to call by the ModManager
        //If you pass an integer to the Invoke constructor, you can control the order in which they load
        //lower loads sooner.  Note that this is per MOD not per-class.
        [Invoke(StateManager.StateTypes.Game, 1)]
        public static void Init2(InitParams initParams)
        {
            Debug.Log("init 2");

        }

        //the 2nd paramater allows you to control which state the ModManager will invoke this method - the Start state
        //-that's when the scene is first loaded, or the game state - when a game is started (either a new game, or a save is loaded)
        [Invoke(StateManager.StateTypes.Start)]
        public static void InitStart(InitParams initParams)
        {
            Debug.Log("init at Start");
        }

        /// <summary>
        /// pushes the hello world message box onto the stack
        /// </summary>
        public static void DisplayMessage(string message = "Hello World!")
        {
            if (helloWorldMessage == null)
            {
                helloWorldMessage = new DaggerfallMessageBox(DaggerfallWorkshop.Game.DaggerfallUI.UIManager);
                helloWorldMessage.AllowCancel = true;
                helloWorldMessage.ClickAnywhereToClose = true;
                helloWorldMessage.ParentPanel.BackgroundColor = Color.clear;
            }

            helloWorldMessage.SetText(message);
            DaggerfallUI.UIManager.PushWindow(helloWorldMessage);
        }


    }
}
