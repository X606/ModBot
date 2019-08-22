using ModLibrary;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InternalModBot
{
    public class ModsPanelManager : MonoBehaviour
    {
        private void Start()
        {
            Vector3 buttonOffset = new Vector3(0f, -1f, 0f); // The offset to give buttons to make space for the Mods button

            GameObject titleScreenContainer = GameUIRoot.Instance.TitleScreenUI.RootButtonsContainer;
            titleScreenContainer.transform.GetChild(5).transform.position += buttonOffset; // Level editor button
            titleScreenContainer.transform.GetChild(7).transform.position += buttonOffset; // Options button
            titleScreenContainer.transform.GetChild(8).transform.position += buttonOffset; // Credits button
            titleScreenContainer.transform.GetChild(9).transform.position += buttonOffset; // Quit button

            // Copy the options button to make into the Mods button
            GameObject modsButtonPrefab = titleScreenContainer.transform.GetChild(7).gameObject;
            GameObject modsButton = Instantiate(modsButtonPrefab, titleScreenContainer.transform);

            modsButton.transform.localPosition = new Vector3(0f, -146f, 0f); // Set position of button
            modsButton.GetComponentInChildren<Text>().text = "MODS"; // Set title

            GameObject modsWindowPrefab = AssetLoader.GetObjectFromFile("modswindow", "ModsMenu", "Clone Drone in the Danger Zone_Data/");
            ModsWindow = Instantiate(modsWindowPrefab);
            
            ModsWindowModdedObject = ModsWindow.GetComponent<ModdedObject>();
            ModsWindow.SetActive(false);

            modsButton.GetComponent<Button>().onClick = new Button.ButtonClickedEvent(); // This is used to remove the persistent listeners that the options button has
            modsButton.GetComponent<Button>().onClick.AddListener(OpenModsMenu); // Add open menu callback
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
            ModOptionsWindowBuilder builder = new ModOptionsWindowBuilder();
            builder.AddSlider(0, 10, 0, "test stuff0");
            builder.AddSlider(0, 10, 1, "test stuff1");
            builder.AddCheckbox(false, "test checkbox1");
            builder.AddCheckbox(true, "test checkbox2");
            builder.AddSlider(0, 10, 2, "test stuff2");
            builder.AddSlider(0, 10, 3, "test stuff3");
            builder.AddSlider(0, 10, 4, "test stuff4");
            builder.AddSlider(0, 10, 5, "test stuff5");
            builder.AddSlider(0, 10, 6, "test stuff6");
            builder.AddSlider(0, 10, 7, "test stuff7");
            builder.AddSlider(0, 10, 8, "test stuff8");
            builder.AddSlider(0, 10, 9, "test stuff9");
            
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
                SetImageFromURL(url);
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
            
        }

        private void SetImageFromURL(string url)
        {
            if (string.IsNullOrEmpty(url))
                return;

            ModImageNetworkConnections.Add(new WWW(url));
        }

        private void Update()
        {
            if (ModImageNetworkConnections.Count == 0)
            {
                return;
            }

            for (int i = 0; i < ModImageNetworkConnections.Count;)
            {
                if (ModImageNetworkConnections[i] != null && ModImageNetworkConnections[i].isDone)
                {
                    Texture2D modImage = new Texture2D(1, 1);
                    ModImageNetworkConnections[i].LoadImageIntoTexture(modImage);

                    if (modImage != null)
                    {
                        ModdedObject modItemModdedObject = ModItems[i].GetComponent<ModdedObject>();
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

        private List<WWW> ModImageNetworkConnections = new List<WWW>();

        private List<GameObject> ModItems = new List<GameObject>();

        private GameObject ModsWindow;

        private ModdedObject ModsWindowModdedObject;

        private const int MOD_ITEM_HEIGHT = 100;
    }
}
