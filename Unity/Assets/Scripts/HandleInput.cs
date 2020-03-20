using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class HandleInput : MonoBehaviour
{
    public Slider precipitationSlider, coverageSlider;
    public Text precipitationText, coverageText;
    private int pollutant = 0, subbasin = 1;
    public Text pollutantText, subbasinText;
    private string[] pollutantNames;
    private float[] pcpValues, coverageValues;
    private WatershedAPI watershedAPI;
    public Text[] legendTexts;
    private HandlePollution handlePollution;
    public Transform cam;
    private float camTargetPosition = 0;
    public Slider zoomInSlider;
    private Button prevButton, nextButton;
    private bool selection = false;
    public GameObject selectionImage;
    private Stopwatch stopwatch;

    void Awake()
    {
        pollutantNames = new string[15];
        pollutantNames[0] = "CBOD_OUTkg";
        pollutantNames[1] = "CHLA_OUTkg";
        pollutantNames[2] = "DISOX_OUTkg";
        pollutantNames[3] = "EVAPcms";
        pollutantNames[4] = "FLOW_OUTcms";
        pollutantNames[5] = "MINP_OUTkg";
        pollutantNames[6] = "NH4_OUTkg";
        pollutantNames[7] = "NO2_OUTkg";
        pollutantNames[8] = "NO3_OUTkg";
        pollutantNames[9] = "ORGN_OUTkg";
        pollutantNames[10] = "ORGP_OUTkg";
        pollutantNames[11] = "SEDCONCmgL";
        pollutantNames[12] = "SED_OUTtons";
        pollutantNames[13] = "TLOSScms";
        pollutantNames[14] = "TOTNkg";

        stopwatch = new Stopwatch();

        stopwatch.Start();

        handlePollution = GetComponent<HandlePollution>();

        pcpValues = new float[31];

        coverageValues = new float[31];

        for(int i = 0; i < pcpValues.Length; i++)
        {
            pcpValues[i] = 756;

            coverageValues[i] = 0;
        }

        prevButton = subbasinText.transform.parent.GetChild(1).GetComponent<Button>();
        nextButton = subbasinText.transform.parent.GetChild(2).GetComponent<Button>();

        watershedAPI = GetComponent<WatershedAPI>();
    }

    void Start()
    {
        ChangeSelection();

        watershedAPI.SetUpLegend(legendTexts);

        zoomInSlider.minValue = camTargetPosition - 150;

        zoomInSlider.maxValue = camTargetPosition + 150;

        zoomInSlider.value = camTargetPosition;

        Calculate();

        InvokeRepeating("Calculate", 1.1f, 1.1f);
    }

    private void LateUpdate()
    {
        Vector3 temp = cam.transform.position;
        temp.z = camTargetPosition - 270f;

        cam.transform.position = Vector3.Lerp(cam.transform.position, temp, Time.deltaTime * 5);
    }

    public void UpdatePrecSlider(float f)
    {
        precipitationText.text = f + "";
        pcpValues[subbasin - 1] = f;
    }

    public void UpdateCoverageSlider(float f)
    {
        coverageText.text = f + "%";
        coverageValues[subbasin - 1] = f;
    }

    public void UpdateZoomIn(float f)
    {
        camTargetPosition = f;
    }

    public void Calculate()
    {
        watershedAPI.GetValues(pcpValues, coverageValues, pollutantNames[pollutant], pollutant);
    }

    public void ChangePollutant(bool next)
    {
        if (next)
        {
            pollutant++;

            if (pollutant > 14)
            {
                pollutant = 0;
            }
        }
        else
        {
            pollutant--;

            if (pollutant < 0)
            {
                pollutant = 14;
            }
        }

        pollutantText.text = pollutantNames[pollutant];

        watershedAPI.SetUpLegend(legendTexts);
    }

    public void ChangeSubbasin(bool next)
    {
        if (next)
        {
            subbasin++;

            if (subbasin > 31)
            {
                subbasin = 1;
            }
        }
        else
        {
            subbasin--;

            if (subbasin < 1)
            {
                subbasin = 31;
            }
        }

        HideSubbasins(subbasin - 1);

        subbasinText.text = subbasin + "";

        precipitationSlider.value = pcpValues[subbasin - 1];
        coverageSlider.value = coverageValues[subbasin - 1];

        precipitationText.text = pcpValues[subbasin - 1] + "";
        coverageText.text = coverageValues[subbasin - 1] + "%";
    }

    public void ChangeSelection()
    {
        if (stopwatch.ElapsedMilliseconds > 400)
        {
            stopwatch.Restart();

            selection = !selection;

            selectionImage.SetActive(selection);

            subbasinText.gameObject.SetActive(!selection);
            prevButton.interactable = !selection;
            nextButton.interactable = !selection;

            if (selection)
            {
                HideSubbasins(-1);

                for (int i = 0; i < pcpValues.Length; i++)
                {
                    pcpValues[i] = pcpValues[subbasin - 1];

                    coverageValues[i] = coverageValues[subbasin - 1];
                }
            }
            else
            {
                HideSubbasins(subbasin - 1);
            }
        }        
    }

    private void HideSubbasins(int num)
    {
        if (num >= 0)
        {
            for (int i = 0; i < handlePollution.subbasins.childCount; i++)
            {
                if (i != num)
                {
                    handlePollution.subbasins.GetChild(i).gameObject.SetActive(false);
                }
                else
                {
                    handlePollution.subbasins.GetChild(i).gameObject.SetActive(true);
                }
            }
        }
        else
        {
            for (int i = 0; i < handlePollution.subbasins.childCount; i++)
            {
                handlePollution.subbasins.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
}
