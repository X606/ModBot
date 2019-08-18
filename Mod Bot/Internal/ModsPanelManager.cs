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

        private void DisableMod(int ID)
        {
            ModsManager.Instance.Mods.RemoveAt(ID);

            ReloadModItems();
        }

        private void AddModToList(Mod mod, GameObject parent)
        {
            GameObject modItemPrefab = AssetLoader.GetObjectFromFile("modswindow", "ModItemPrefab", "Clone Drone in the Danger Zone_Data/");
            GameObject modItem = Instantiate(modItemPrefab, parent.transform);

            string modName = mod.GetModName();
            string url = mod.GetModImageURL();

            if (string.IsNullOrEmpty(url))
            {
                ModItems.Add(modItem);
                SetImageFromURL(url);
            }

            ModdedObject modItemModdedObject = modItem.GetComponent<ModdedObject>();

            modItemModdedObject.GetObject<Text>(0).text = modName; // Set title
            modItemModdedObject.GetObject<Text>(1).text = mod.GetModDescription(); // Set description

            
            int ModID = ModsAddedToList;
            modItemModdedObject.GetObject<Button>(3).onClick.AddListener(delegate { DisableMod(ModID); }); // Add disable button callback

            modItem.transform.localPosition -= new Vector3(0f, MOD_ITEM_HEIGHT * ModsAddedToList, 0f);

            ModsAddedToList++;
        }

        private void SetImageFromURL(string url)
        {
            ModImageNetworkConnections.Add(new WWW(url));
        }

        private void Update()
        {
            if (ModImageNetworkConnections.Count != ModItems.Count)
            {
                debug.Log("Error: Amount of mod images to download does not match amount of registered mods!", Color.red);
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
                    ModItems.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }
                
        private void ReloadModItems()
        {
            ModsAddedToList = 0;
            
            // Remove all mods from list
            foreach (Transform child in ((GameObject)ModsWindowModdedObject.objects[0]).transform)
            {
                Destroy(child.gameObject);
            }

            // Set the Content panel (ModdedObjectModsWindow.objects[0]) to appropriate height
            ModsWindowModdedObject.GetObject<GameObject>(0).GetComponent<RectTransform>().sizeDelta += new Vector2(0f, MOD_ITEM_HEIGHT * ModsManager.Instance.Mods.Count);

            // Add all mods back to list
            for (int i = 0; i < ModsManager.Instance.Mods.Count; i++)
            {
                AddModToList(ModsManager.Instance.Mods[i], ModsWindowModdedObject.GetObject<GameObject>(0));
            }
        }

        private List<WWW> ModImageNetworkConnections = new List<WWW>();

        private List<GameObject> ModItems = new List<GameObject>();

        private GameObject ModsWindow;

        private ModdedObject ModsWindowModdedObject;

        private int ModsAddedToList;

        private const int MOD_ITEM_HEIGHT = 100;
    }
}
