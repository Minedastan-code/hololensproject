/// PositionManager
/// Michele Fiorentino 2021 -project dastan
/// FUNCTION:
/// 1 -Given a Target ( being public can be set programmatically) compute positional and angular erros between it and mytool
/// 2- By using Blurobject* it shows the distance using visualizeError function 
/// 3- current blur is o at 0 distance 0 (we cvan put a minimum ) then linear interpol  then maxvalue
///  ATTENTION: * change viusualization material of meshrenderer form first gaeobject attached to the tool

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// (check this)  Warning: we change teh material of first gaeobject attached to the tool

[System.Serializable]
public class PositionChange : UnityEvent<Vector3, Vector3>
{
}

public class PositionManagerv2 : MonoBehaviour
{
    [Header("-------Scene Settings (one time)---------")]

    [Tooltip("Tool object -poistion and rotation will be considered")]
    public GameObject my_tool;
    [Header("-------Debug (do not touch)  ---------")]
    [Tooltip("Target object -poistion and rotation will be considered")]
    public GameObject my_target;

   
    [Header("Subscribe To Events")]
    [SerializeField] public PositionChange OnErrorChange;

      // one day privates
    //private UnityEngine.UI.Text myTextelemnt;
    public Vector3 my_positional_error; //error in mm
    public Vector3 my_angular_error;

 
    void Start()
    {
        // zeros errors
        my_positional_error = new Vector3(0, 0, 0);
        my_angular_error = new Vector3(0, 0, 0);
     }

    // Update is called once per frame
    void Update()
    {
        computeErrorAndInvoke();
    }

    private void computeErrorAndInvoke()
    {
        my_positional_error = (my_target.transform.position - my_tool.transform.position);
        my_angular_error = AngleDifferenceUnder180( my_target.transform.rotation,  my_tool.transform.rotation); // This need check
        OnErrorChange.Invoke(my_positional_error, my_angular_error);
    }
      
    // Printing fuctionsfacilities
    string getVectorStringSeparated(Vector3 p_vec, string p_separator = ";", string p_precision = "F3")
    {
        string temp;

        temp = p_vec.magnitude.ToString(p_precision) + p_separator + p_vec.x.ToString(p_precision) + p_separator + p_vec.y.ToString(p_precision) + p_separator + p_vec.z.ToString(p_precision);
        return temp;

    }

    public string getFullerrorStringSeparated(string p_separator = ";", string p_precision = "F3")
    {
        string temp;
        temp = getVectorStringSeparated(my_positional_error, p_separator, p_precision) + p_separator + getVectorStringSeparated(my_angular_error, p_separator, p_precision);
        return temp;

    }

    public string getFullerrorStringSeparatedINmm(string p_separator = ";", string p_precision = "F3")
    {
        string temp;
        temp = getVectorStringSeparated(my_positional_error*1000, p_separator, p_precision) + p_separator + getVectorStringSeparated(my_angular_error, p_separator, p_precision);
        return temp;

    }

    private Vector3 AngleDifferenceUnder180(Quaternion Angle1, Quaternion Angle2)
    {

        Vector3 Difference = new Vector3();
        Difference = (Angle1 * Quaternion.Inverse(Angle2)).eulerAngles;
        if (Difference.x > 180) Difference.x -= 360;
        else if (Difference.x < -180) Difference.x += 360;
        if (Difference.y > 180) Difference.y -= 360;
        else if (Difference.y < -180) Difference.y += 360;
        if (Difference.z > 180) Difference.z -= 360;
        else if (Difference.z < -180) Difference.z += 360;
        return Difference;
    }

}
