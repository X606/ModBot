using InternalModBot;
using ModLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using TwitchChatter;
using UnityEngine;
using UnityEngine.UI;

namespace InjectionClasses
{
    public class fakeAction
    {
        public fakeAction(MethodInfo _method, object _instance)
        {
            method = _method;
            instance = _instance;
        }

        public fakeAction(MethodInfo _method, object _instance, object[] _args)
        {
            method = _method;
            instance = _instance;
            args = _args;
        }

        public void Invoke(object[] parms)
        {
            method.Invoke(instance, parms);
        }

        public void Invoke()
        {
            method.Invoke(instance, args);
        }

        public MethodInfo method;

        public object instance;

        public object[] args;
    }

    public class WaitThenCallClass : Singleton<WaitThenCallClass>
    {
        private void Update()
        {
            for (int i = 0; i < callbacks.Count; i++)
            {
                if (callbacks[i].time >= Time.time)
                {
                    callbacks[i].callback.Invoke();
                    callbacks.RemoveAt(i);
                    i--;
                }
            }
        }

        public void AddCallback(fakeAction callback, float offset)
        {
            callbacks.Add(new Callback(callback, Time.time + offset));
        }

        private List<Callback> callbacks = new List<Callback>();
    }

    public struct Callback
    {
        public Callback(fakeAction _callback, float _time)
        {
            callback = _callback;
            time = _time;
        }

        public fakeAction callback;

        public float time;
    }

    public class ModsManager : Singleton<ModsManager>
    {
        public void Start()
        {
            Instantiate(AssetLoader.GetObjectFromFile("twitchmode", "Canvas", "Clone Drone in the Danger Zone_Data/"));
            reloadMods();
            passOnMod.OnSceneChanged(GameMode.None);
        }

        public void reloadMods()
        {
            UpgradeCosts.Reset();
            UpgradePagesMangaer.Reset();
            CleanCache();
            AssetLoader.ClearCache();
            mods.Clear();
            passOnMod = new PassOnToModsManager();
            string[] files = Directory.GetFiles(AssetLoader.GetSubdomain(Application.dataPath) + "mods/");
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].EndsWith(".dll"))
                {
                    try
                    {
                        Type[] types = Assembly.Load(File.ReadAllBytes(files[i])).GetTypes();
                        Type type = null;
                        for (int j = 0; j < types.Length; j++)
                        {
                            if (types[j].Name.ToLower() == "main")
                            {
                                type = types[j];
                            }
                        }
                        if (type == null)
                        {
                            throw new Exception("could not find class 'main'");
                        }
                        object obj = Activator.CreateInstance(type);
                        mods.Add((Mod)obj);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("Mod '" + files[i] + "' is not working, make sure that it is set up right: " + ex.Message);
                    }
                }
            }
            passOnMod.OnModRefreshed();
        }

        public void Update()
        {
            if (Input.GetKey(KeyCode.F3) && Input.GetKeyDown(KeyCode.R))
            {
                reloadMods();
            }
        }

        public void LoadMod(byte[] assebly)
        {
            try
            {
                Type[] types = Assembly.Load(assebly).GetTypes();
                Type type = null;
                for (int i = 0; i < types.Length; i++)
                {
                    if (types[i].Name == "main")
                    {
                        type = types[i];
                    }
                }
                if (type == null)
                {
                    throw new Exception("could not find class 'main'");
                }
                object obj = Activator.CreateInstance(type);
                mods.Add((Mod)obj);
                ((Mod)obj).OnModRefreshed();
            }
            catch
            {
                Debug.LogError("The mod you are trying to load isn't valid");
            }
        }

        public static void CleanCache()
        {
            if (Caching.ClearCache())
            {
                Singleton<Logger>.Instance.log("Successfully cleaned the cache.");
                return;
            }
            Singleton<Logger>.Instance.log("Cache is being used.");
        }

        public List<Mod> mods = new List<Mod>();

        public Mod passOnMod = new PassOnToModsManager();
    }

    public class modSuggestingManager : Singleton<modSuggestingManager>
    {
        private void Start()
        {
            TwitchChatClient.singleton.AddChatListener(new ChatMessageNotificationDelegate(OnTwitchChatMessage));
        }

        private void Update()
        {
            if (!isInSuggestMode)
            {
                return;
            }
            if (Input.GetKeyDown(KeyCode.PageUp))
            {
                accept();
                isInSuggestMode = false;
            }
            if (Input.GetKeyDown(KeyCode.PageDown))
            {
                deny();
                isInSuggestMode = false;
            }
        }

        public void suggestMod()
        {
            ani.Play("suggestMod");
            creatorName.text = "Suggested By: " + suggester;
            modName.text = modNameString;
            isInSuggestMode = true;
        }

        public void accept()
        {
            TwitchManager.Instance.EnqueueChatMessage("Mod accepted. :)");
            ani.Play("AcceptMod");
            isInSuggestMode = false;
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(MyRemoteCertificateValidationCallback);
                byte[] response = new WebClient
                {
                    Headers =
                {
                    "User-Agent: Other"
                }
                }.DownloadData(url);
                Singleton<ModsManager>.Instance.LoadMod(response);
            }
            catch (Exception e)
            {
                Singleton<Logger>.Instance.log(e.Message, Color.red);
            }
        }

        public void deny()
        {
            TwitchManager.Instance.EnqueueChatMessage("Mod denied. :(");
            ani.Play("DenyMod");
            isInSuggestMode = false;
        }

        public void OnTwitchChatMessage(ref TwitchChatMessage msg)
        {
            string lowerText = msg.chatMessagePlainText;
            if (!lowerText.StartsWith("!"))
            {
                return;
            }
            string[] subCommands = lowerText.Split(' ');
            if (subCommands[0].ToLower() == "!modsuggest")
            {
                if (subCommands.Length >= 3)
                {
                    url = subCommands[2];
                    suggester = "<color=" + msg.userNameColor + ">" + msg.userName + "</color>";
                    modNameString = subCommands[1];
                    suggestMod();
                    TwitchManager.Instance.EnqueueChatMessage("Mod suggested!");
                }
                else
                {
                    TwitchManager.Instance.EnqueueChatMessage("Usage: !modsuggest <mod_name> <mod_link>");
                }
            }

        }

        public bool MyRemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool isOk = true;
            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                for (int i = 0; i < chain.ChainStatus.Length; i++)
                {
                    if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                        if (!chain.Build((X509Certificate2)certificate))
                        {
                            isOk = false;
                        }
                    }
                }
            }
            return isOk;
        }

        public Animator ani;

        public bool isInSuggestMode;

        public Text modName;

        public Text creatorName;

        public string url;

        public string suggester;

        public string modNameString;
    }

    public class Logger : Singleton<Logger>
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                Flip();
            }
            if (!Container.activeSelf)
            {
                return;
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                RunCommand(input.text);
                input.text = "";
            }
        }

        private void Flip()
        {
            if (Container.activeSelf)
            {
                animator.Play("hideConsole");
                return;
            }
            animator.Play("showConsole");
        }

        public void log(string whatToLog)
        {
            Text logText = LogText;
            logText.text = logText.text + "\n" + whatToLog;
        }

        public void log(string whatToLog, Color color)
        {
            string text = ColorUtility.ToHtmlStringRGB(color);
            Text logText = LogText;
            logText.text = logText.text + "\n<color=#" + text + ">" + whatToLog + "</color>";
        }

        public void RunCommand(string command)
        {
            try
            {
                ConsoleInputManager.OnCommandRan(command);
                Singleton<ModsManager>.Instance.passOnMod.OnCommandRan(command);
            }
            catch (Exception ex)
            {
                log("command '" + command + "' failed with the following error: " + ex.Message, Color.red);
                log(ex.StackTrace, Color.red);
            }
        }

        public Animator animator;

        public Text LogText;

        public GameObject Container;

        public InputField input;
    }

    public class FPSCount : MonoBehaviour
    {
        private void Start()
        {
            frames = new List<float>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F3))
            {
                counter.gameObject.SetActive(!counter.gameObject.activeSelf);
            }
            float num = 1f / Time.unscaledDeltaTime;
            frames.Add(num);
            if (frames.Count > 10)
            {
                frames.RemoveAt(0);
            }
            float FPS = 0f;
            for (int i = 0; i < frames.Count; i++)
            {
                FPS += frames[i];
            }
            FPS /= frames.Count;
            counter.text = ((int)FPS).ToString();
        }

        public Text counter;

        private List<float> frames;
    }

    public class moddedObject : MonoBehaviour
    {
        private void Awake()
        {
            Singleton<moddedObjectsManager>.Instance.addToModdedObjects(this);
        }

        private void OnDestroy()
        {
            Singleton<moddedObjectsManager>.Instance.removeFromModdedObjects(this);
        }

        public string ID;

        public List<UnityEngine.Object> objects;
    }

    public class moddedObjectsManager : Singleton<moddedObjectsManager>
    {
        public void addToModdedObjects(moddedObject obj)
        {
            moddedObjects.Add(obj);
        }

        public List<moddedObject> GetAllModdedObjects()
        {
            return moddedObjects;
        }

        private void Start()
        {
            Singleton<GlobalEventManager>.Instance.AddEventListener("LevelCleanupStarted", new Action(clear));
        }

        public void clear()
        {
            moddedObjects.Clear();
        }

        public void removeFromModdedObjects(moddedObject obj)
        {
            moddedObjects.Remove(obj);
        }

        private List<moddedObject> moddedObjects = new List<moddedObject>();
    }

    public class xButton : MonoBehaviour
    {
        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}