using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.UI;

public class PredictionResponse
{
    // Prediction outputs
    public float[] max, min;

    public float scalar = 100.0f;

    private float scale(float output, float min, float max)
    {
        return ((output - min) / (max - min)) * scalar;
    }

    public float ScalePollutant(int num, float value)
    {
        return scale(value, min[num], max[num]);
    }
}

public class PredictionInputs
{
    // These are the model's inputs
    public float[] pcpArray = new float[31];
    public float[] fcArray = new float[31];
}

public class WatershedAPI : MonoBehaviour
{
    private string WatershedURL = "https://unitywatershedvr.herokuapp.com/prediction";
    //private string WatershedURL = "https://afternoon-lake-66212.herokuapp.com/prediction";

    public PredictionInputs inputs;
    public float[] predictions;

    public GameObject errorMessage;

    string pollutant;

    int pollutantNum;

    int subbasin;

    PredictionResponse response;

    void Awake()
    {
        inputs = new PredictionInputs();
        response = new PredictionResponse();
        predictions = new float[31];

        response.max = new float[15];
        response.min = new float[15];

        response.max[0] = 12054.4888067f;
        response.max[1] = 96575.1897031f;
        response.max[2] = 2785.52653122f;
        response.max[3] = 10665.583f;
        response.max[4] = 24.135647600000002f;
        response.max[5] = 7.273015842f;
        response.max[6] = 94.7428272f;
        response.max[7] = 116.53953662f;
        response.max[8] = 361.859638849f;
        response.max[9] = 91.65428622600001f;
        response.max[10] = 1281.15524804f;
        response.max[11] = 1106.86503567f;
        response.max[12] = 6837.604740000001f;
        response.max[13] = 125.672086815f;
        response.max[14] = 203.17574228799998f;

        response.min[0] = 2329.39450322f;
        response.min[1] = 6006.97724123f;
        response.min[2] = 281.716818034f;
        response.min[3] = 2310.40047f;
        response.min[4] = 3.91857522f;
        response.min[5] = 0.45715384f;
        response.min[6] = 14.83112554f;
        response.min[7] = 12.009719096f;
        response.min[8] = 44.248495612f;
        response.min[9] = 10.214591054f;
        response.min[10] = 83.79102402f;
        response.min[11] = 148.055664844f;
        response.min[12] = 1580.75826f;
        response.min[13] = 12.585968496f;
        response.min[14] = 21.05105519f;
    }

    public void GetValues(float[] pcp, float[] fc, string pollutant, int pollutantNum)
    {
        for (int i = 0; i < inputs.pcpArray.Length; i++)
        {
            inputs.pcpArray = pcp;
        }

        for (int i = 0; i < inputs.fcArray.Length; i++)
        {
            inputs.fcArray = fc;
        }

        this.pollutant = pollutant;
        this.pollutantNum = pollutantNum;

        StartCoroutine(SyncPredictions());
    }

    IEnumerator SyncPredictions() {
        WWWForm formData = new WWWForm();

        for (int i = 0; i < inputs.pcpArray.Length; i++)
        {
            formData.AddField("PCP" + (i + 1), inputs.pcpArray[i].ToString());
        }

        for (int i = 0; i < inputs.fcArray.Length; i++)
        {
            formData.AddField("FOREST_COVER" + (i + 1), inputs.fcArray[i].ToString());
        }

        UnityWebRequest www = UnityWebRequest.Post(WatershedURL, formData);
        www.method = "POST";
        yield return www.SendWebRequest();
 
        if(www.isNetworkError || www.isHttpError) {
            errorMessage.SetActive(true);

            Debug.Log(www.error);
        }
        else {

            JSONNode j = JSONNode.Parse (www.downloadHandler.text);

            for(int i = 0; i < predictions.Length; i++)
            {
                predictions[i] = j[pollutant + " " + (i + 1)];
            }

            float[] scaledPredictions = new float[31];

            for(int i = 0; i < scaledPredictions.Length; i++)
            {
                scaledPredictions[i] = response.ScalePollutant(pollutantNum, predictions[i]);                   
            }

            GetComponent<HandlePollution>().SetSizes(scaledPredictions);

            errorMessage.SetActive(false);
        }
    }

    public void SetUpLegend(Text[] legendTexts)
    {
        float dif = response.max[pollutantNum] - response.min[pollutantNum];

        legendTexts[0].text = "Between " + Mathf.Round(response.min[pollutantNum]) + " and " + Mathf.Round((dif * 0.25f) + response.min[pollutantNum]);
        legendTexts[1].text = "Between " + Mathf.Round((dif * 0.25f) + response.min[pollutantNum]) + " and " + Mathf.Round((dif * 0.5f) + response.min[pollutantNum]);
        legendTexts[2].text = "Between " + Mathf.Round((dif * 0.5f) + response.min[pollutantNum]) + " and " + Mathf.Round((dif * 0.75f) + response.min[pollutantNum]);
        legendTexts[3].text = "Between " + Mathf.Round((dif * 0.75f) + response.min[pollutantNum]) + " and " + Mathf.Round(response.max[pollutantNum]);
    }
}
