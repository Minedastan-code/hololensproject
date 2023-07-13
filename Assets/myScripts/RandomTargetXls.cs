///summary
// 25/11/2021 Michele Fiorentino
// FUCTIONS: 
// 1-Randomize the Activation of the Target Objects 
// 2 -current target Object is sent  to the Positional Manager to compute distance and visual
// 3- When specific  Action (e.g. when users clicks) saves timestamp and distances in file
// 4- Automatic excel file naming in Application.persistentDataPath + Risultati- no timeout
// 5- "IDTarget;taskmemillisec;distancemagnitude;distvectposx;distvectposy;distvectposz;deltagnmag;deltaangx;deltaangy;deltaangz"
/*
 * 
 * Android: Application.persistentDataPath points to /storage/emulated/0/Android/data/<packagename>/files on most devices
 * (some older phones might point to location on SD card if present), 
 * the path is resolved using android.content.Context.getExternalFilesDir
 * */
// FIND files here Questo PC\Quest 2\Internal shared storage\Android\data\com.PolitecnicodiBari.oculusPrism\files\Risultati

using System;
using System.Collections;
using System.IO;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[System.Serializable]
public class onMyDebugLogEvent : UnityEvent<string>
{
}

public class RandomTargetXls : MonoBehaviour
{
    // publics

    [Tooltip("List of targets - they will be shuffled at start")]
    public List<GameObject> Targetlist;

    [Header("-------Custom Action-------")]

    [Tooltip("Put here the action related to the positionreached ")]
    public InputActionReference mypostionReachedAction = null;

    [Tooltip("This the object conataining as component the positional manager (sparated to be more versatile)")]
    public GameObject positionmaagerobject;

    [Header("-------Custom Events-------")]

    [Tooltip("Set custom Event at keypress(eg feedback sound)")]
    public UnityEvent positionReachedCustomEvent; // the events to be run

    [Tooltip("Set custom Event at keypress(eg feedback sound)")]
    public UnityEvent endEperimentCustomEvent; // the events to be run

    [Header("-------Output File -------")]


    [Tooltip("Separator for output")]
    public string outpseparator = ";";

    [Tooltip("Prefix output")]
    public string outputFileTratment = "TreatmentA";

    [Tooltip("File extension")]
    public string outputFileExtension = ".csv";

    [Header("-------Public only for debug-------")]
    // current target on display 
    public int currentTargetIndex = 0;
    // Output Filename
    private string outfilepath;

    // privates
    private DateTime targetStartTime;
    private bool experimentOn = false;

    [Tooltip("Positional manger")]
    private PositionManagerv2 posmanref;

    [Header("Subscribe To my debug LOG Events")]
    [SerializeField] public onMyDebugLogEvent onmydebug;

    void Start()
    {

        Randomutils.Shuffle(Targetlist);

        if (!positionmaagerobject) Debug.LogError("No positionmaagerobject found");
        posmanref = (PositionManagerv2)positionmaagerobject.GetComponent(typeof(PositionManagerv2));
        if (!posmanref) Debug.LogError("No positional manager found");

        InizializeFile("-targets");
        // loads first target
        TargetLoader();
    }
    // from https://www.youtube.com/watch?v=jOn0YWoNFVY
    private void Awake()
    {
        mypostionReachedAction.action.started += onPositionReached;
    }

    private void OnDestroy()
    {
        mypostionReachedAction.action.started += onPositionReached;
    }


    private void InizializeFile(string p_title)
    {
        // opens file in user path 
        string userdirectory = Application.persistentDataPath + "/Risultati/";

        //Create folder if not exist creates it
        if (!Directory.Exists(userdirectory))
        {
            Debug.Log("Created directory:" + userdirectory);
            Directory.CreateDirectory(userdirectory);
        }

        //Create Filename and if it doesn't exist folder creates it
        outfilepath = userdirectory + outputFileTratment + p_title + System.DateTime.Now.ToString("-yyyy-MM-dd-HH-mm-ss") + outputFileExtension;

        if (!File.Exists(outfilepath))
        {
            TextWriter tw = new StreamWriter(outfilepath, false);
            tw.WriteLine("IDTarget;taskmemillisec;distancemagnitude(mm);distvectposx;distvectposy;distvectposz;deltagnmag;deltaangx;deltaangy;deltaangz");
            tw.Close();
            Debug.Log("Created file log: " + outfilepath);
            onmydebug.Invoke("Created file log: " + outfilepath);

        }
    }
    void saveEntry(string res, double p_delta = 0)
    {
        string risultato = currentTargetIndex + outpseparator + p_delta.ToString("f0") + outpseparator + res + "\n";
        File.AppendAllText(outfilepath, risultato);
        Debug.Log("saved:" + risultato);
    }

    public void onPositionReached(InputAction.CallbackContext context)
    {

        if (experimentOn)
        {
            Debug.Log("Button pressed on" + currentTargetIndex);
            double delta = (DateTime.Now - targetStartTime).TotalMilliseconds;
            if (positionReachedCustomEvent != null)
            {
                positionReachedCustomEvent.Invoke();
            }
            saveEntry(posmanref.getFullerrorStringSeparatedINmm(), delta);
            currentTargetIndex++;
            TargetLoader();
        }
    }

    private void disableAllTargets()
    {
        // Non sapendo quale è disattivo tutti e accendo solo quello giusto
        foreach (GameObject o in Targetlist)
        {
            o.SetActive(false);
        }
    }

    void TargetLoader()
    {
        if (currentTargetIndex < Targetlist.Count)
        {
            disableAllTargets();
            // Showing next Target
            Targetlist[currentTargetIndex].SetActive(true);
            // assing in positional manager
            if (posmanref) posmanref.my_target = Targetlist[currentTargetIndex];

            targetStartTime = DateTime.Now; // set time
            experimentOn = true;

            string myprintouttemp = mainExperimentLoader.Instance.GetSituation();
            myprintouttemp += ("\n " + "Target: " + currentTargetIndex + " of:" + Targetlist.Count);

            Debug.Log(myprintouttemp);
            onmydebug.Invoke(myprintouttemp);
        }
        else // finished
        {
            if (endEperimentCustomEvent != null)
            {
                endEperimentCustomEvent.Invoke();
            }

            Debug.Log("Targets finished");
            onmydebug.Invoke("Finshed Saved file log: " + outfilepath);
            mainExperimentLoader.Instance.addFile(outfilepath); // add file to the list (maybe send email)
            experimentOn = false; // block
        }
    }
}
