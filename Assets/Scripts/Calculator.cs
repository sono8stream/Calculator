using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//入力された2つの小数を計算するプログラムです
public class Calculator : MonoBehaviour
{
    const int MAXdigit = 8;//最大桁数

    [SerializeField]
    int[] value1, value2;//2つの入力値
    int digitCounter1, digitCounter2;//現在の桁数カウンター
    int dotPosition1, dotPosition2;//小数点の位置
    Operator operatorType;
    bool onFirstValue;//value1の入力中かどうか

    Text valueText;

    // Use this for initialization
    void Start()
    {
        value1 = new int[MAXdigit];
        value2 = new int[MAXdigit];
        operatorType = 0;

        valueText = GetComponent<Text>();

        InitializeCalculation();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //何も入力がない状態を-1として初期化するメソッド
    public void InitializeCalculation()
    {
        value1[0] = 0;
        value2[0] = 0;
        for (int i = 1; i < MAXdigit; i++)
        {
            value1[i] = -1;
            value2[i] = -1;
        }

        digitCounter1 = 0;
        digitCounter2 = 0;

        dotPosition1 = -1;//未設定であれば-1に
        dotPosition2 = -1;

        operatorType = 0;
        onFirstValue = true;

        valueText.text = "0";
    }

    //配列から数値に変換するメソッド
    double ArrayToValue(int[] valueArray, int digitCounter, int dotPosition)
    {
        double value = 0;
        for (int i = 0; i < digitCounter; i++)
        {
            value = value * 10 + valueArray[i];
        }

        if (dotPosition != -1)
        {
            value /= Mathf.Pow(10, digitCounter - dotPosition - 1);//小数点の位置で桁を補正する
        }
        return value;
    }

    int[] ValueTextToArray(ref int dotPosition, ref int digitCounter)
    {
        int[] array = new int[MAXdigit];
        array[0] = 0;
        for (int i = 1; i < MAXdigit; i++)
        {
            array[i] = -1;
        }
        string text = valueText.text;

        double val;
        if (!double.TryParse(text, out val)) return array;

        int index = text.IndexOf('.');
        if (index > 0)//小数点が表示されているとき
        {
            dotPosition = index - 1;
            text = text.Replace(".", "");
        }
        int length = text.Length;
        if (MAXdigit < length)
        {
            digitCounter = MAXdigit;
        }
        else
        {
            digitCounter = length;
        }
        Debug.Log(digitCounter);

        for (int i = 0; i < digitCounter; i++)
        {
            array[i] = text[i] - '0';
        }
        return array;
    }

    //数字ボタンを押したとき
    public void InputNumber(int number)
    {
        if (onFirstValue)
        {
            if (digitCounter1 >= MAXdigit
                || (digitCounter1 == 0 && number == 0)) return;
            value1[digitCounter1] = number;

            digitCounter1++;
            valueText.text = ArrayToValue(value1, digitCounter1, dotPosition1).ToString("G");
        }
        else
        {
            if (digitCounter2 >= MAXdigit
                || (digitCounter2 == 0 && number == 0)) return;
            value2[digitCounter2] = number;

            digitCounter2++;
            valueText.text = ArrayToValue(value2, digitCounter2, dotPosition2).ToString("G");
        }
    }

    public void DeleteNumber()
    {
        if (onFirstValue)
        {
            if (digitCounter1 == 0) return;//何も入力されていなければ中断
            value1[digitCounter1] = -1;
            digitCounter1--;
            if (digitCounter1 < dotPosition1)
            {
                dotPosition1 = -1;
            }

            valueText.text = ArrayToValue(value1, digitCounter1, dotPosition1).ToString("G");
        }
        else
        {
            if (digitCounter2 == 0) return;//何も入力されていなければ中断
            value2[digitCounter2] = -1;
            digitCounter2--;
            if (digitCounter2 < dotPosition2)
            {
                dotPosition2 = -1;
            }

            valueText.text = ArrayToValue(value2, digitCounter2, dotPosition2).ToString("G");
        }
    }

    //四則演算子ボタンを押したとき
    public void SetOperator(int operatorNo)
    {
        if (!onFirstValue) return;//value2の入力後は受け付けない

        operatorType = (Operator)System.Enum.ToObject(typeof(Operator), operatorNo);
        onFirstValue = false;
        value2 = new int[MAXdigit];
        value2[0] = 0;
        for(int i = 0; i < MAXdigit; i++)
        {
            value2[i] = -1;
        }
    }

    //計算処理
    public void Calculate()
    {
        double v1 = ArrayToValue(value1, digitCounter1, dotPosition1);
        double v2 = ArrayToValue(value2, digitCounter2, dotPosition2);
        double result = 0;

        switch (operatorType)
        {
            case Operator.Plus:
                result = v1 + v2;
                break;
            case Operator.Minus:
                result = v1 - v2;
                break;
            case Operator.Multiply:
                result = v1 * v2;
                break;
            case Operator.Divide:
                if (v2 == 0) break;//0で割るとエラーするので回避

                result = v1 / v2;
                break;
        }

        //InitializeCalculation();

        string s;
        if (System.Math.Log10(result) > MAXdigit)
        {
            s = "overFlow";
        }
        else
        {
            s = result.ToString();
        }
        valueText.text = s;
        value1 = ValueTextToArray(ref dotPosition1, ref digitCounter1);
    }

    public void SetDotPoint()
    {
        if (onFirstValue)
        {
            if (digitCounter1 == 0)
            {
                dotPosition1 = 0;
                digitCounter1++;
            }
            else
            {
                dotPosition1 = digitCounter1 - 1;
            }
        }
        else
        {
            if (digitCounter2 == 0)
            {
                dotPosition2 = 0;
                digitCounter2++;
            }
            else
            {
                dotPosition2 = digitCounter2 - 1;
            }
        }
    }

    public void Sine()
    {
        double value;

        if (onFirstValue)
        {
            value = ArrayToValue(value1, digitCounter1, dotPosition1);
            value *= System.Math.PI / 180;
            value = System.Math.Sin(value);
            valueText.text = value.ToString();
            value1 = ValueTextToArray(ref dotPosition1, ref digitCounter1);
        }
        else
        {
            value = ArrayToValue(value2, digitCounter2, dotPosition2);
            value *= System.Math.PI / 180;
            value = System.Math.Sin(value);
            valueText.text = value.ToString();
            value1 = ValueTextToArray(ref dotPosition2, ref digitCounter2);
        }
    }
}

public enum Operator
{
    Plus, Minus, Multiply, Divide
}