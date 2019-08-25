using ModLibrary;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InternalModBot
{
    public class ModsPanelManager : Singleton<ModsPanelManager>
    {
        private void Start()
        {
            Vector3 mainMenuButtonOffset = new Vector3(0f, -1f, 0f); // The offset to give buttons on the main menu to make space for the Mods button
            Vector3 pauseScreenButtonOffest = new Vector3(0f, 1.2f, 0f); 

            GameObject titleScreenContainer = GameUIRoot.Instance.TitleScreenUI.RootButtonsContainer;
            titleScreenContainer.transform.GetChild(5).transform.position += mainMenuButtonOffset; // Level editor button
            titleScreenContainer.transform.GetChild(7).transform.position += mainMenuButtonOffset; // Options button
            titleScreenContainer.transform.GetChild(8).transform.position += mainMenuButtonOffset; // Credits button
            titleScreenContainer.transform.GetChild(9).transform.position += mainMenuButtonOffset; // Quit button

            // Copy the options button to make into the Mods button
            GameObject modsButtonPrefab = titleScreenContainer.transform.GetChild(7).gameObject;
            GameObject mainMenuModsButton = Instantiate(modsButtonPrefab, titleScreenContainer.transform);

            mainMenuModsButton.transform.localPosition = new Vector3(0f, -146f, 0f); // Set position of button
            mainMenuModsButton.GetComponentInChildren<Text>().text = "MODS"; // Set title


            GameObject pauseScreenModsButton = Instantiate(GameUIRoot.Instance.EscMenu.SettingsButton.transform.gameObject, GameUIRoot.Instance.EscMenu.SettingsButton.transform.parent); // All of these lines edit the buttons on the pause menu
            GameUIRoot.Instance.EscMenu.ReturnToGameButton.transform.position += pauseScreenButtonOffest;
            GameUIRoot.Instance.EscMenu.SettingsButton.transform.position += pauseScreenButtonOffest;
            GameUIRoot.Instance.EscMenu.ExitButton.transform.position -= pauseScreenButtonOffest;
            GameUIRoot.Instance.EscMenu.ExitConfirmUI.transform.position -= pauseScreenButtonOffest;
            GameUIRoot.Instance.EscMenu.MainMenuButton.transform.position -= pauseScreenButtonOffest;
            GameUIRoot.Instance.EscMenu.MainMenuConfirmUI.transform.position -= pauseScreenButtonOffest;

            pauseScreenModsButton.transform.position -= pauseScreenButtonOffest;
            pauseScreenModsButton.GetComponentInChildren<Text>().text = "MODS";

            GameObject modsWindowPrefab = AssetLoader.GetObjectFromFile("modswindow", "ModsMenu", "Clone Drone in the Danger Zone_Data/");
            ModsWindow = Instantiate(modsWindowPrefab);
            
            ModsWindowModdedObject = ModsWindow.GetComponent<ModdedObject>();
            ModsWindow.SetActive(false);

            mainMenuModsButton.GetComponent<Button>().onClick = new Button.ButtonClickedEvent(); // This is used to remove the persistent listeners that the options button has
            mainMenuModsButton.GetComponent<Button>().onClick.AddListener(OpenModsMenu); // Add open menu callback
            pauseScreenModsButton.GetComponent<Button>().onClick = new Button.ButtonClickedEvent(); // This is used to remove the persistent listeners that the options button has
            pauseScreenModsButton.GetComponent<Button>().onClick.AddListener(OpenModsMenu); // Add open menu callback

            ModsWindowModdedObject.GetObject<Button>(1).onClick.AddListener(CloseModsMenu); // Add close menu button callback
            
            ReloadModItems();
        }

        private void OpenModsMenu()
        {
            ModsWindow.SetActive(true);
        }

        private void CloseModsMenu()
        {
            ModsWindow.SetActive(false);
        }

        private void OpenModsOptionsWindowForMod(Mod mod)
        {
            ModOptionsWindowBuilder builder = new ModOptionsWindowBuilder(ModsWindow, mod);
            mod.CreateSettingsWindow(builder);

        }

        private void ToggleIsModDisabled(int ID)
        {
            Mod mod = ModsManager.Instance.GetAllMods()[ID];
            bool? isNotActive = ModsManager.Instance.IsModDeactivated(mod);
            if (!isNotActive.HasValue)
                return;
            
            if (isNotActive.Value)
            {
                ModsManager.Instance.EnableMod(mod);
            } else
            {
                ModsManager.Instance.DisableMod(mod);
            }
            
            ReloadModItems();

        }

        private void AddModToList(Mod mod, GameObject parent)
        {
            bool? isModNotActive = ModsManager.Instance.IsModDeactivated(mod);
            if (!isModNotActive.HasValue)
                return;

            GameObject modItemPrefab = AssetLoader.GetObjectFromFile("modswindow", "ModItemPrefab", "Clone Drone in the Danger Zone_Data/");
            GameObject modItem = Instantiate(modItemPrefab, parent.transform);

            string modName = mod.GetModName();
            string url = mod.GetModImageURL();

            ModItems.Add(modItem);

            if (!string.IsNullOrEmpty(url))
            {
                SetImageFromURL(url, mod);
            }

            ModdedObject modItemModdedObject = modItem.GetComponent<ModdedObject>();

            modItemModdedObject.GetObject<Text>(0).text = modName; // Set title
            modItemModdedObject.GetObject<Text>(1).text = mod.GetModDescription(); // Set description
            modItemModdedObject.GetObject<Text>(5).text = "Mod ID: " + mod.GetUniqueID();

            if (isModNotActive.Value)
            {
                modItem.GetComponent<Image>().color = Color.red;
                Button disableButton = modItem.GetComponent<ModdedObject>().GetObject<Button>(3);
                disableButton.transform.GetChild(0).GetComponent<Text>().text = "Enable Mod";
                disableButton.colors = new ColorBlock() { normalColor = Color.green * 1.2f, highlightedColor = Color.green, pressedColor = Color.green * 0.8f, colorMultiplier = 1 };
            }

            int modId = ModsManager.Instance.GetAllMods().IndexOf(mod);
            modItemModdedObject.GetObject<Button>(3).onClick.AddListener(delegate { ToggleIsModDisabled(modId); }); // Add disable button callback
            modItemModdedObject.GetObject<Button>(4).onClick.AddListener(delegate { OpenModsOptionsWindowForMod(mod); }); // Add Mod Options button callback
            modItemModdedObject.GetObject<Button>(4).interactable = mod.ImplementsSettingsWindow();
        }

        private void SetImageFromURL(string url, Mod owner)
        {
            if (string.IsNullOrEmpty(url))
                return;

            ModImageNetworkConnections.Add(new DoubleValueHolder<Mod,WWW>(owner, new WWW(url)) );
        }

        private void Update()
        {
            if (ModsWindow.activeInHierarchy)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    CloseModsMenu();
                }
            }

           


            if (ModImageNetworkConnections.Count == 0)
            {
                return;
            }

            for (int i = 0; i < ModImageNetworkConnections.Count;)
            {
                if (ModImageNetworkConnections[i].SecondValue != null && ModImageNetworkConnections[i].SecondValue.isDone)
                {
                    Texture2D modImage = new Texture2D(1, 1);
                    ModImageNetworkConnections[i].SecondValue.LoadImageIntoTexture(modImage);

                    if (modImage != null)
                    {
                        ModdedObject modItemModdedObject = FindModItemWithName(ModImageNetworkConnections[i].FirstValue.GetUniqueID());
                        modItemModdedObject.GetObject<RawImage>(2).texture = modImage; // Set image
                    }

                    ModImageNetworkConnections.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }
        
        private ModdedObject FindModItemWithName(string id)
        {
            foreach(GameObject moddedObject in ModItems)
            {
                if (moddedObject.GetComponent<ModdedObject>().GetObject<Text>(5).text == "Mod ID: " + id)
                {
                    return moddedObject.GetComponent<ModdedObject>();
                }
            }
            return null;
        }

        private void ReloadModItems()
        {
            ModItems.Clear();

            // Remove all mods from list
            foreach (Transform child in ((GameObject)ModsWindowModdedObject.objects[0]).transform)
            {
                Destroy(child.gameObject);
            }

            List<Mod> mods = ModsManager.Instance.GetAllMods();

            // Set the Content panel (ModdedObjectModsWindow.objects[0]) to appropriate height
            RectTransform size = ModsWindowModdedObject.GetObject<GameObject>(0).GetComponent<RectTransform>();
            size.sizeDelta = new Vector2(size.sizeDelta.x, MOD_ITEM_HEIGHT * mods.Count);

            // Add all mods back to list
            for (int i = 0; i < mods.Count; i++)
            {
                AddModToList(mods[i], ModsWindowModdedObject.GetObject<GameObject>(0));
            }
        }

        private List<DoubleValueHolder<Mod,WWW>> ModImageNetworkConnections = new List<DoubleValueHolder<Mod, WWW>>();

        private List<GameObject> ModItems = new List<GameObject>();

        private GameObject ModsWindow;

        private ModdedObject ModsWindowModdedObject;

        private const int MOD_ITEM_HEIGHT = 100;
    }
}
