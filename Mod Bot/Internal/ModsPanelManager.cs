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
            GameObject modsButton = GameObject.Instantiate(modsButtonPrefab, titleScreenContainer.transform);

            modsButton.transform.localPosition = new Vector3(0f, -146f, 0f); // Set position of button
            modsButton.GetComponentInChildren<Text>().text = "MODS"; // Set title

            GameObject modsWindowPrefab = AssetLoader.GetObjectFromFile("modswindow", "ModsMenu", "Clone Drone in the Danger Zone_Data/");
            modsWindow = GameObject.Instantiate(modsWindowPrefab);
            
            moddedObjectModsWindow = modsWindow.GetComponent<moddedObject>();
            modsWindow.SetActive(false);

            modsButton.GetComponent<Button>().onClick = new Button.ButtonClickedEvent(); // This is used to remove the persistent listeners that the options button has
            modsButton.GetComponent<Button>().onClick.AddListener(OpenModsMenu); // Add open menu callback
            ((Button)moddedObjectModsWindow.objects[1]).onClick.AddListener(CloseModsMenu); // Add close menu button callback

            ReloadModItems();
        }

        private void OpenModsMenu()
        {
            modsWindow.SetActive(true);
        }

        private void CloseModsMenu()
        {
            modsWindow.SetActive(false);
        }

        private void DisableMod(int ID)
        {
            ModsManager.Instance.mods.RemoveAt(ID);

            ReloadModItems();
        }

        private void AddModToList(Mod mod, GameObject parent)
        {
            GameObject modItemPrefab = AssetLoader.GetObjectFromFile("modswindow", "ModItemPrefab", "Clone Drone in the Danger Zone_Data/");
            GameObject modItem = GameObject.Instantiate(modItemPrefab, parent.transform);

            string modName = mod.GetModName();
            string url = mod.GetModImageURL();

            if (url != "")
            {
                modItems.Add(modItem);
                SetImageFromURL(url);
            }

            ((Text)modItem.GetComponent<moddedObject>().objects[0]).text = modName; // Title
            ((Text)modItem.GetComponent<moddedObject>().objects[1]).text = mod.GetModDescription(); // Description

            
            int ModID = modsAddedToList;
            ((Button)modItem.GetComponent<moddedObject>().objects[3]).onClick.AddListener(delegate { DisableMod(ModID); }); // Add disable button callback

            modItem.transform.localPosition -= new Vector3(0f, MOD_ITEM_HEIGHT * modsAddedToList, 0f);

            modsAddedToList++;
        }

        private void SetImageFromURL(string url)
        {
            imageNetworkConnections.Add(new WWW(url));
        }

        private void Update()
        {
            for (int i = 0; i < imageNetworkConnections.Count;)
            {
                if (imageNetworkConnections[i] != null && imageNetworkConnections[i].isDone)
                {
                    Texture2D modImage = new Texture2D(1, 1);
                    imageNetworkConnections[i].LoadImageIntoTexture(modImage);

                    if (modImage != null)
                    {
                        ((RawImage)modItems[i].GetComponent<moddedObject>().objects[2]).texture = modImage; // Image
                    }

                    imageNetworkConnections.RemoveAt(i);
                    modItems.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }
                
        private void ReloadModItems()
        {
            modsAddedToList = 0;
            
            // Remove all mods from list
            foreach (Transform child in ((GameObject)moddedObjectModsWindow.objects[0]).transform)
            {
                Destroy(child.gameObject);
            }

            // Set the Content panel (ModdedObjectModsWindow.objects[0]) to appropriate height
            ((GameObject)moddedObjectModsWindow.objects[0]).GetComponent<RectTransform>().sizeDelta += new Vector2(0f, MOD_ITEM_HEIGHT * ModsManager.Instance.mods.Count);

            // Add all mods back to list
            for (int i = 0; i < ModsManager.Instance.mods.Count; i++)
            {
                AddModToList(ModsManager.Instance.mods[i], (GameObject)moddedObjectModsWindow.objects[0]);
            }
        }

        private List<WWW> imageNetworkConnections = new List<WWW>();

        private List<GameObject> modItems = new List<GameObject>();

        private GameObject modsWindow;

        private moddedObject moddedObjectModsWindow;

        private int modsAddedToList;

        private const int MOD_ITEM_HEIGHT = 100;
    }
}
