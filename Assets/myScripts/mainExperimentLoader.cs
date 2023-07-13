/// Michele Fiorentino
/// USAGE: Singleton that  manage the loadin of scenes for the experiment
/// 


using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainExperimentLoader : MonoBehaviour
{
    public static mainExperimentLoader Instance;

    public List<string> treatmentA;
    public List<string> treatmentB;
    private List<string> currenttreatment;
    private List<string> filelist;

    // later private
    public int counter = -1;

    private void Awake()
    {
        // If exist -singleston
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void nextTest()
    {
        counter++;
        if (counter < currenttreatment.Count)
        {

            Debug.Log("sounter:" + counter + "Loading scene:" + currenttreatment[counter]);
            SceneManager.LoadScene(currenttreatment[counter]);
        } else
        {
            Debug.Log("End Of experiment");
        }
    }

    // Start is called before the first frame update
    void Start()
    {

        Resetexperiment();
       // SendEmail();
    }


    public void LoadA()
    {
        Resetexperiment();
        currenttreatment = treatmentA;
        nextTest();
    }
    public void LoadB()
    {
        Resetexperiment();
        currenttreatment = treatmentB;
        nextTest();
    }

    public void Resetexperiment()
        {
        counter = -1;
        filelist = new List<string>();
        currenttreatment = new List<string>();

    }

    public string GetSituation()
    {
        string temp= ("Test: " + (counter+1) + "of " + (currenttreatment.Count-1));
        return temp;
    }
    public void addFile(string p_filename)
    {
        filelist.Add(p_filename);
    }

    public void SendEmail()
    {
        string[] targets = { "michele.fiorentino@poliba.it", "michele.fiorentino@poliba.it" };
        if (SendGmail("ciao", "ciao", targets, "vr3lab@gmail.com")) Debug.Log("email OK");
        else Debug.Log("email error");

    }
    private bool SendGmail(string subject, string content, string[] recipients, string from)
    {
        if (recipients == null || recipients.Length == 0)
            throw new System.ArgumentException("recipients");

        var gmailClient = new System.Net.Mail.SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new System.Net.NetworkCredential("vr3lab@gmail.com", "ARIndustry4.0")
        };

        using (var msg = new System.Net.Mail.MailMessage(from, recipients[0], subject, content))
        {
            for (int i = 1; i < recipients.Length; i++)
                msg.To.Add(recipients[i]);

            try
            {
                gmailClient.Send(msg);
                return true;
            }
            catch (System.Exception)
            {
                // TODO: Handle the exception
                Debug.LogError("email error");
                return false;
            }
        }
    }

}
