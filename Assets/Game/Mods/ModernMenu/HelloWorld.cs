using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Utility;
using DaggerfallConnect.Utility;

namespace ModernMenu
{
    public class HelloWorld : MonoBehaviour
    {
        Rect characterSheetWindow = new Rect(5, 5, 300, 400);
        Rect inventoryWindow = new Rect(5, 310, 400, 300);
        Rect magicWindow = new Rect(500, 5, 200, 300);
        bool isVisible = false;
        const string baseTextureName = "INVE00I0.IMG";

        void Start()
        {
            Debug.Log("Modern menu Start() called.");
        }

        void OnGUI()
        {
            if (isVisible)
            {
                int windowId = 0;
                characterSheetWindow = GUI.Window(windowId++, characterSheetWindow, DoTestWindow, "Character");
                GUI.depth = 1;
                inventoryWindow = GUI.Window(windowId++, inventoryWindow, DoTestWindow, "Inventory");
                GUI.depth = -1;
                Texture2D baseTexture = ImageReader.GetTexture(baseTextureName);
                Rect wagonButtonRect = new Rect(226, 14, 31, 14);
                DFSize baseSize = new DFSize(320, 200);
                Texture2D buttonTexture = ImageReader.GetSubTexture(baseTexture, wagonButtonRect, baseSize);
                GUI.Button(new Rect(inventoryWindow.x + 10, inventoryWindow.y + 20, 100f, 100f), new GUIContent(buttonTexture, "tooltip"));
                magicWindow = GUI.Window(windowId, magicWindow, DoTestWindow, "Magic");
                GUI.Label(new Rect(Input.mousePosition.x, Input.mousePosition.y, 100, 40), GUI.tooltip);
            }
        }

        void DoTestWindow(int windowId)
        {
            GUI.DragWindow(new Rect(0, 0, 10000, 40));
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (!isVisible)
                {
                    GameManager.Instance.PauseGame(true);
                    isVisible = true;
                }
                else
                {
                    GameManager.Instance.PauseGame(false);
                    isVisible = false;
                }
            }
        }

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
    }
}
