using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//Probability distribution function.
//Exponentially weighted mean of sample PDFs.
[CreateAssetMenu(fileName = "New PDF", menuName = "Math/PDF")]
public class PDF : ScriptableObject
{
    public string Name;
    public float[] pdf = null;
    public float alpha;



    public bool updatePDF(float[] data)
    {
        float difference = 0;
        if (pdf == null || pdf.Length==0)
        {
            pdf = data;
            return true;
        }

        if (data.Length != pdf.Length) return false;
        for (int i = 0; i < pdf.Length; i++)
        {
            float temp = pdf[i];
            pdf[i] = alpha*pdf[i] + (1-alpha)*data[i];
            difference += (temp - pdf[i]) * (temp - pdf[i]);
        }
        Debug.Log(String.Format("RMSE: {0}",Mathf.Sqrt(difference)));
        return true;
    }

    public void normalizePDF()
    {
        normalize(pdf);
    }

    public bool normalize(float[] arr)
    {
        float s = sum(arr);
        if (s == 0) return false;
        for(int i = 0; i < arr.Length; i++)
        {
            arr[i] = arr[i] / s;
        }
        return true;
    }


    public float sum(float[] arr)
    {
        float s = 0;
        foreach (float i in arr) s += i;
        return s;
    }

    public int levelCurve(float target)
    {
        for (int i=0; i < pdf.Length; i++)
        {
            if (target <= 0) return i;
            target -= pdf[i];
        }
        return pdf.Length;
    }

    public float levelCurve(float target, float factor) { return levelCurve(target) / factor; }

    public int[] levelCurves(float[] targets)
    {
        int[] curves = new int[targets.Length];
        for (int i = 0; i < curves.Length; i++) curves[i] = -1;

        float sum = 0;
        for (int i = 0; i < pdf.Length; i++)
        {
            for (int j = 0; j < targets.Length; j++)
            {
                if (curves[j]==-1 && targets[j] <= sum) curves[j]=i;
            }
            sum += pdf[i];
        }

        for (int i = 0; i < curves.Length; i++)
        {
            if (curves[i] == -1) curves[i] = pdf.Length;
        }

        return curves;
    }

    public float[] levelCurves(float[] targets, float factor)
    {
        int[] curves = levelCurves(targets);
        float[] to_return = new float[curves.Length];
        for (int i = 0; i < curves.Length; i++)
        {
            to_return[i] = curves[i] / factor;
        }
        return to_return;
    }

}
