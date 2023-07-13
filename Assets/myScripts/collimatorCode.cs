/// collimatorCode
/// Michele Fiorentino 2022 -project dastan
/// FUNCTION:
/// 1 -Position top texture in X=0 and move Y to fit collimation   (top drives)
/// 2- 
/// 3- 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// future have different easings
// https://forum.unity.com/threads/logarithmic-interpolation.354344/
//https://github.com/lordofduct/spacepuppy-unity-framework/blob/master/SpacepuppyBase/Tween/Easing.cs


public class collimatorCode : MonoBehaviour
{
    [Tooltip("Top collimator")]
    public GameObject topCollimator;
    [Tooltip("Bottom collimator")]
    public GameObject bottomCollimator;
     public enum InputChooseValueEnum { POSX, POSY, POSZ, EUL1, EUL2, EUL3 };
    [Header("-------Collimator Mapping parameters-------")]

    [Tooltip("3 materials ")]
    public Material normalMat;
    public Material outboundsMat;
    //public Material outreach;


    [Tooltip("Chose the input")]
    public InputChooseValueEnum myvalue;
    [Tooltip("Distance when collimator appears")]
    public float maxDistance = 0.05f; // default 5 cm
    [Tooltip("Distance when collimator change color")]
    public float targetDistance = 0.002f; // default 2mm  ?to test

    [Tooltip("Multiplier with distance")]
    public float multiplier = 50f; // 1mm = 5cm
    [Header("-------Collimator Mapping parameters-------")]

    private Vector3 topCollimatorPos = new Vector3();
    private Vector3 bottomCollimatorPos = new Vector3();

    public float inputvalueRaw = 0;
    // Start is called before the first frame update

    void Start()
    {
        // copy initial pos
        topCollimatorPos =  (topCollimator.transform.localPosition);
 

        bottomCollimatorPos = (bottomCollimator.transform.localPosition);
        topCollimator.GetComponent<MeshRenderer>().material = normalMat;
        bottomCollimator.GetComponent<MeshRenderer>().material = normalMat;
    }


    public void onDistanceChange(Vector3 p_position, Vector3 p_angle)
    {

        switch (myvalue)
        {
            case InputChooseValueEnum.POSX:
                {
                    // your code 
                    // for plus operator
                    inputvalueRaw = p_position.x;
                    break;
                }
            case InputChooseValueEnum.POSY:
                {
                    // your code 
                    // for plus operator
                    inputvalueRaw = p_position.y;
                    break;
                }
            case InputChooseValueEnum.POSZ:
                {
                    // your code 
                    // for plus operator
                    inputvalueRaw = p_position.z;
                    break;
                }
            case InputChooseValueEnum.EUL1:
                {
                    // your code 
                    // for plus operator
                    inputvalueRaw = p_angle.x;
                    break;
                }
            case InputChooseValueEnum.EUL2:
                {
                    // your code 
                    // for plus operator
                    inputvalueRaw = p_angle.y;
                    break;
                }
            case InputChooseValueEnum.EUL3:
                {
                    // your code 
                    // for plus operator
                    inputvalueRaw = p_angle.z;
                    break;
                }
            default: break;
        }
        // Coversion


        if (Mathf.Abs(inputvalueRaw) > maxDistance)
        {
            topCollimator.SetActive(true);
            bottomCollimator.SetActive(true);
            topCollimator.GetComponent<MeshRenderer>().material = outboundsMat;
            bottomCollimator.GetComponent<MeshRenderer>().material = outboundsMat;
            topCollimatorPos = new Vector3(-maxDistance * multiplier, topCollimatorPos.y);
            bottomCollimatorPos = new Vector3(maxDistance * multiplier, -topCollimatorPos.y);

        }
        else // Inside teh viusalizatin area
        {

            topCollimator.SetActive(true);
            bottomCollimator.SetActive(true);
            topCollimator.GetComponent<MeshRenderer>().material = normalMat;
            bottomCollimator.GetComponent<MeshRenderer>().material = normalMat;
            //loat tempAperture = Mathf.Lerp(0, maximum, t); // not necessary
            // was localposition
            topCollimatorPos = new Vector3(-inputvalueRaw * multiplier, topCollimatorPos.y);
            bottomCollimatorPos = new Vector3(inputvalueRaw * multiplier, -topCollimatorPos.y);

            if (Mathf.Abs(inputvalueRaw) < targetDistance)
            {
                topCollimator.SetActive(false);
                bottomCollimator.SetActive(false);
            }
            else
            {
                topCollimator.GetComponent<MeshRenderer>().material = normalMat;
                bottomCollimator.GetComponent<MeshRenderer>().material = normalMat;
            }
        }

    }


    // Update is called once per frame
    void Update()
    {
           topCollimator.transform.localPosition = topCollimatorPos;
           bottomCollimator.transform.localPosition = bottomCollimatorPos;

    }
}
