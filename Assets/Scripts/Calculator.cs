using System;//追加しました
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//入力された2つの小数を計算するプログラムです
public class Calculator : MonoBehaviour
{
    const int SECTIONS = 2;
    const int MAXdigit = 8;//最大桁数
    const int PASTlimit = 10;

    [SerializeField]
    int[,] values;//2つの入力値
    int[] digitCounters;//現在の桁数カウンター
    int[] dotPositions;//小数点の位置
    bool[] minus;
    int valueCounter;
    int operatorNo;

    List<double> pastValues;//過去の結果保持用
    int pastIndex;//過去の結果をさかのぼったときの位置

    Text valueText;

    // Use this for initialization
    void Start()
    {
        values = new int[SECTIONS, MAXdigit];
        digitCounters = new int[SECTIONS];
        dotPositions = new int[SECTIONS];
        minus = new bool[SECTIONS];

        pastValues = new List<double>();
        pastIndex = 0;

        valueText = GetComponent<Text>();

        InitializeCalculation();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void InitializeArray(int index)
    {
        values[index, 0] = 0;
        for (int i = 1; i < MAXdigit; i++)//何も入力がない状態を-1として初期化
        {
            values[index, i] = -1;
        }

        digitCounters[index] = 0;
        dotPositions[index] = -1;//未設定であれば-1に
        minus[index] = false;
    }

    //初期化メソッド(「C」に対応)
    public void InitializeCalculation()
    {
        for (int i = 0; i < SECTIONS; i++)
        {
            InitializeArray(i);
        }

        operatorNo = 0;
        valueCounter = 0;
        valueText.text = "0";
    }

    void ShowValueText(int valueIndex)
    {
        string s = "";
        if (minus[valueIndex])
        {
            s += "-";
        }

        for (int i = 0; i < digitCounters[valueIndex]; i++)
        {
            if (i != 0 && i == dotPositions[valueIndex] + 1)
            {
                s += ".";
            }
            s += values[valueIndex, i].ToString();
        }

        valueText.text = s;
    }

    //配列から数値に変換するメソッド
    double ArrayToValue(int index)
    {
        double value = 0;
        for (int i = 0; i < digitCounters[index]; i++)
        {
            value = value * 10 + values[index, i];
        }

        if (dotPositions[index] != -1)
        {
            value /= Mathf.Pow(10,
                digitCounters[index] - dotPositions[index] - 1);//小数点の位置で桁を補正する
        }
        if (minus[index])
        {
            value *= -1;
        }
        return value;
    }

    //引数のインデックスのvalueに割り当て
    void ValueToArray(double value, int outIndex)
    {
        minus[outIndex] = value < 0;
        value = Math.Abs(value);

        int digitNumber;//最大桁位置
        if (value < 1)
        {
            digitNumber = 1;
        }
        else
        {
            digitNumber = (int)Math.Floor(Math.Log10(value)) + 1;
        }

        int nowNumber;//現在桁の値
        //値を割り当て
        for (int i = 0; i < MAXdigit; i++)
        {
            if (digitNumber == 1)//1の位であれば小数点を設定
            {
                dotPositions[outIndex] = i;
            }
            nowNumber = (int)Math.Floor(value / Math.Pow(10, digitNumber - 1));//最大桁の値
            values[outIndex, i] = nowNumber;

            value -= nowNumber * Math.Pow(10, digitNumber - 1);//最大桁を1つ下げる
            digitNumber--;
        }

        //最小桁から見てゆき、小数点以下かつ0ならば未入力状態(-1)に
        for (int i = MAXdigit - 1; i >= 0; i--)
        {
            if (digitNumber < 0 && values[outIndex, i] == 0)
            {
                values[outIndex, i] = -1;
                if (digitNumber == -1)
                {
                    dotPositions[outIndex] = -1;
                }
                digitNumber++;
            }
            else
            {
                digitCounters[outIndex] = i + 1;//桁位置を設定
                break;
            }
        }
    }

    void AddPastValue(double value)
    {
        pastValues.Add(value);
        if (pastValues.Count > PASTlimit)
        {
            pastValues.RemoveAt(0);
        }
        pastIndex = pastValues.Count - 1;
    }

    //数字ボタンを押したとき
    public void InputNumber(int number)
    {
        if (digitCounters[valueCounter] >= MAXdigit
            || (digitCounters[valueCounter] == 0 && number == 0)) return;

        values[valueCounter, digitCounters[valueCounter]] = number;
        digitCounters[valueCounter]++;
        ShowValueText(valueCounter);
    }

    public void DeleteNumber()
    {
        if (digitCounters[valueCounter] == 0) return;

        values[valueCounter, digitCounters[valueCounter]] = -1;
        digitCounters[valueCounter]--;
        if (digitCounters[valueCounter] < dotPositions[valueCounter])
        {
            dotPositions[valueCounter] = -1;
        }
        ShowValueText(valueCounter);
    }

    //四則演算子ボタンを押したとき
    public void SetOperator(int no)
    {
        if (valueCounter == 1)//計算処理
        {
            Calculate();
        }

        //2入力目の受付
        operatorNo = no;
        valueCounter++;
        InitializeArray(valueCounter);
    }

    //計算処理
    public void Calculate()
    {
        double v1 = ArrayToValue(0);
        double v2 = ArrayToValue(1);
        double result = 0;

        switch (operatorNo)
        {
            case 0://和
                result = v1 + v2;
                break;
            case 1://差
                result = v1 - v2;
                break;
            case 2://積
                result = v1 * v2;
                break;
            case 3://商
                if (v2 == 0) break;//0で割るとエラーするので回避

                result = v1 / v2;
                break;
        }

        if (Math.Log10(result) > MAXdigit)
        {
            valueText.text = "overFlow";
            InitializeCalculation();
        }
        else
        {
            valueCounter = 0;
            ValueToArray(result, valueCounter);
            ShowValueText(valueCounter);
            AddPastValue(result);
        }
    }

    public void SetDotPoint()
    {
        if (digitCounters[valueCounter] == 0)
        {
            dotPositions[valueCounter] = 0;
            digitCounters[valueCounter]++;
        }
        else
        {
            dotPositions[valueCounter] = digitCounters[valueCounter] - 1;
        };
    }

    public void SinCosTan(int calcNo)
    {
        double value = ArrayToValue(valueCounter);
        value *= Math.PI / 180;
        Debug.Log(value);
        switch (calcNo)
        {
            case 0://sine
                value = Math.Sin(value);
                break;
            case 1://cosine
                value = Math.Cos(value);
                break;
            case 2://tangent
                value = Math.Tan(value);
                break;
        }

        ValueToArray(value, valueCounter);
        ShowValueText(valueCounter);
    }

    public void ReferPastValue(bool up)
    {
        if (pastValues.Count == 0) return;

        if (up && pastIndex > 0)
        {
            pastIndex--;
        }
        else if (!up && pastIndex < pastValues.Count - 1)
        {
            pastIndex++;
        }

        ValueToArray(pastValues[pastIndex], valueCounter);
        ShowValueText(valueCounter);
    }
}

public enum Operator
{
    Plus, Minus, Multiply, Divide
}