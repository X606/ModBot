using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using ModLibrary;

namespace InternalModBot
{
    public class ModsPanelManager : MonoBehaviour
    {
        private void Start()
        {
            Vector3 buttonOffset = new Vector3(0f, -1f, 0f); // The offset to give buttons to make space for the Mods button

            GameObject titleScreenContainer = GameUIRoot.Instance.TitleScreenUI.RootButtonsContainer;

            titleScreenContainer.transform.GetChild(5).transform.position += buttonOffset; // Level editor button
            titleScreenContainer.transform.GetChild(6).transform.position += buttonOffset; // Options button
            titleScreenContainer.transform.GetChild(7).transform.position += buttonOffset; // Credits button
            titleScreenContainer.transform.GetChild(8).transform.position += buttonOffset; // Quit button

            // Copy the options button to make into the Mods button
            GameObject modsButtonPrefab = titleScreenContainer.transform.GetChild(6).gameObject;
            GameObject modsButton = GameObject.Instantiate(modsButtonPrefab, titleScreenContainer.transform);

            modsButton.transform.localPosition = new Vector3(0f, -146f, 0f); // Set position of button

            modsButton.GetComponentInChildren<Text>().text = "MODS"; // Set title
            
            GameObject modsWindowPrefab = AssetLoader.getObjectFromFile("modswindow", "ModsMenu", "Clone Drone in the Danger Zone_Data/");
            ModsWindow = GameObject.Instantiate(modsWindowPrefab);

            ModdedObjectModsWindow = ModsWindow.GetComponent<moddedObject>();
            
            ModsWindow.SetActive(false);

            modsButton.GetComponent<Button>().onClick = new Button.ButtonClickedEvent(); // This is used to remove the persistent listeners that the options button has
            modsButton.GetComponent<Button>().onClick.AddListener(OpenModsMenu); // Add open menu callback

            ((Button)ModdedObjectModsWindow.objects[1]).onClick.AddListener(CloseModsMenu); // Add close menu button callback

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
            ModsManager.Instance.mods.RemoveAt(ID);

            ReloadModItems();
        }

        private void AddModToList(Mod mod, GameObject parent)
        {
            GameObject modItemPrefab = AssetLoader.getObjectFromFile("modswindow", "ModItemPrefab", "Clone Drone in the Danger Zone_Data/");
            GameObject modItem = GameObject.Instantiate(modItemPrefab, parent.transform);

            string modName = mod.GetModName() == null ? "This mod does not have a name, contact the creator to add it in the Mod class" : mod.GetModName();
            Texture2D modImage = mod.GetModImage();

            ((Text)modItem.GetComponent<moddedObject>().objects[0]).text = modName; // Title
            ((Text)modItem.GetComponent<moddedObject>().objects[1]).text = mod.GetModDescription(); // Description

            if (modImage != null)
                ((RawImage)modItem.GetComponent<moddedObject>().objects[2]).texture = modImage; // Image

            int ModID = ModsAddedToList;
            ((Button)modItem.GetComponent<moddedObject>().objects[3]).onClick.AddListener(delegate { DisableMod(ModID); }); // Disable button

            modItem.transform.localPosition -= new Vector3(0f, ModItemHeight * ModsAddedToList, 0f);

            ModsAddedToList++;
        }

        private void ReloadModItems()
        {
            ModsAddedToList = 0;
            
            // Remove all mods from list
            foreach (Transform child in ((GameObject)ModdedObjectModsWindow.objects[0]).transform)
            {
                Destroy(child.gameObject); //TODO: Remove this disgusting fucking pest from the code base, but make sure you replace it by something that works though...
            }

            // Set the Content panel (ModdedObjectModsWindow.objects[0]) to appropriate height
            ((GameObject)ModdedObjectModsWindow.objects[0]).GetComponent<RectTransform>().sizeDelta += new Vector2(0f, ModItemHeight * ModsManager.Instance.mods.Count);

            // Add all mods back to list
            for (int i = 0; i < ModsManager.Instance.mods.Count; i++)
            {
                AddModToList(ModsManager.Instance.mods[i], (GameObject)ModdedObjectModsWindow.objects[0]);
            }
        }

        private GameObject ModsWindow;

        private moddedObject ModdedObjectModsWindow;

        private int ModsAddedToList;

        private const int ModItemHeight = 100;
    }
}
