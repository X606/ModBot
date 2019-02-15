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
            Vector3 ButtonOffset = new Vector3(0f, -1f, 0f);

            GameUIRoot.Instance.TitleScreenUI.RootButtonsContainer.transform.GetChild(5).transform.position += ButtonOffset;
            GameUIRoot.Instance.TitleScreenUI.RootButtonsContainer.transform.GetChild(6).transform.position += ButtonOffset;
            GameUIRoot.Instance.TitleScreenUI.RootButtonsContainer.transform.GetChild(7).transform.position += ButtonOffset;
            GameUIRoot.Instance.TitleScreenUI.RootButtonsContainer.transform.GetChild(8).transform.position += ButtonOffset;

            GameObject ModsButton = GameObject.Instantiate(GameUIRoot.Instance.TitleScreenUI.RootButtonsContainer.transform.GetChild(6).gameObject, GameUIRoot.Instance.TitleScreenUI.RootButtonsContainer.transform);
            ModsButton.transform.localPosition = new Vector3(0f, -146f, 0f);

            ModsButton.GetComponentInChildren<Text>().text = "MODS";
            
            GameObject prefab = AssetLoader.getObjectFromFile("modswindow", "ModsMenu", "Clone Drone in the Danger Zone_Data/");
            ModsWindow = GameObject.Instantiate(prefab);

            moddedObject spawnedWindowModdedObject = ModsWindow.GetComponent<moddedObject>();
            
            ModsWindow.SetActive(false);

            ModsButton.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            ModsButton.GetComponent<Button>().onClick.AddListener(OpenModsMenu);
            ((Button)spawnedWindowModdedObject.objects[1]).onClick.AddListener(CloseModsMenu);

            UpdateModItems(spawnedWindowModdedObject);
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

            UpdateModItems(ModsWindow.GetComponent<moddedObject>());
        }

        private void AddModToList(Mod mod, GameObject parent, int ID)
        {
            GameObject modItemPrefab = AssetLoader.getObjectFromFile("modswindow", "ModItemPrefab", "Clone Drone in the Danger Zone_Data/");
            GameObject modItem = GameObject.Instantiate(modItemPrefab, parent.transform);

            string modName = mod.GetModName() == null ? "This mod does not have a name, contact the creator to add it in the Mod class" : mod.GetModName();
            Texture2D modImage = mod.GetModImage();

            ((Text)modItem.GetComponent<moddedObject>().objects[0]).text = modName;
            ((Text)modItem.GetComponent<moddedObject>().objects[1]).text = mod.GetModDescription();

            if (modImage != null)
                ((RawImage)modItem.GetComponent<moddedObject>().objects[2]).texture = modImage;

            ((Button)modItem.GetComponent<moddedObject>().objects[3]).onClick.AddListener(delegate { DisableMod(ID); });

            modItem.transform.localPosition -= new Vector3(0f, 100 * ModsAddedToList, 0f);

            ModsAddedToList++;
        }

        private void UpdateModItems(moddedObject modsWindowMO)
        {
            ModsAddedToList = 0;

            foreach (Transform child in ((GameObject)modsWindowMO.objects[0]).transform)
            {
                Destroy(child.gameObject); //TODO: Remove this disgusting fucking pest from the code base, but make sure you replace it by something that works though...
            }

            ((GameObject)modsWindowMO.objects[0]).GetComponent<RectTransform>().sizeDelta += new Vector2(0f, 100f * ModsManager.Instance.mods.Count);

            for (int i = 0; i < ModsManager.Instance.mods.Count; i++)
            {
                AddModToList(ModsManager.Instance.mods[i], (GameObject)modsWindowMO.objects[0], i);
            }
        }

        private GameObject ModsWindow;

        private int ModsAddedToList;

        private const int ModItemHeight = 100;
    }

    // LevelEditor: 5
    // Options: 6
    // Credits: 7
    // Quit: 8

    // MODITEM: //
    // Title: 0
    // Description: 1
    // Image: 2
    // Disable button: 3
}
