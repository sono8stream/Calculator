using System;//追加しました
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//入力された複数の値(小数含む)を計算するプログラムです
public class Calculator : MonoBehaviour
{
    const int MAXsection = 100;
    const int MAXoperator = 50;
    const int MAXdigit = 8;//最大桁数

    [SerializeField]
    int[,] values;//入力値の配列
    int[] dotPositions;//入力値の小数点の位置
    int sectionCounter;//現在の項数
    int[] operatorNumbers;//四則演算子の配列

    int[] valueArray;//現在の入力値、各桁を配列で管理
    int digitCounter;//入力値の桁数カウンター
    int dotPosition;//入力値の小数点の位置

    Text valueText;

    // Use this for initialization
    void Start()
    {
        values = new int[MAXsection, MAXdigit];
        dotPositions = new int[MAXsection];

        valueArray = new int[MAXdigit];
        operatorNumbers = new int[MAXoperator];

        valueText = GetComponent<Text>();

        InitializeCalculation();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void InitializeValueArray()
    {
        valueArray[0] = 0;
        for (int i = 1; i < MAXdigit; i++)//何も入力がない状態を-1として初期化
        {
            valueArray[i] = -1;
        }
        digitCounter = 0;
        dotPosition = -1;//未設定であれば-1に
    }

    public void InitializeCalculation()
    {
        for (int sectionI = 0; sectionI < MAXsection; sectionI++)
        {
            for (int digitI = 0; digitI < MAXdigit; digitI++)
            {
                values[sectionI, digitI] = -1;
            }
            dotPositions[sectionI] = -1;
        }
        sectionCounter = 0;

        for (int i = 0; i < MAXoperator; i++)
        {
            operatorNumbers[i] = 0;
        }

        InitializeValueArray();

        valueText.text = "0";
    }

    //valueArrayを数値に変換し、valuesに格納するメソッド
    void AddSection()
    {
        for(int i = 0; i < MAXdigit; i++)//値コピー(参照コピーは避ける)
        {
            values[sectionCounter, i] = valueArray[i];
        }
        dotPositions[sectionCounter] = dotPosition;

        /*valuesがdoubleのとき
        double value = 0;
        for (int i = 0; i < digitCounter; i++)
        {
            value = value * 10 + valueArray[i];
        }

        if (dotPosition != -1)
        {
            value /= Mathf.Pow(10, digitCounter - dotPosition - 1);//小数点の位置で桁を補正する
        }
        values[sectionCounter] = value;
        */
        sectionCounter++;

        InitializeValueArray();
    }

    //doubleの計算値を配列に変換してvalueArrayに格納するメソッド
    void SetValueToArray(double value)
    {
        int digitNumber = (int)Math.Floor(Math.Log10(value)) + 1;//最大桁数を取得
        int nowNumber;//現在桁の値
        //値を割り当て
        for(int i = 0; i < MAXdigit; i++)
        {
            if (digitNumber == 1)//1の位であれば小数点を設定
            {
                dotPosition = i;
            }

            nowNumber = (int)Math.Floor(value / Math.Pow(10, digitNumber - 1));//最大桁の値
            valueArray[i] = nowNumber;
            value -= nowNumber * Math.Pow(10, digitNumber - 1);//最大桁を1つ下げる
            digitNumber--;
        }

        //最小桁から見てゆき、小数点以下かつ0ならば未入力状態(-1)に
        for (int i = MAXdigit; i >= 0; i--)
        {
            if (digitNumber <= 0 && valueArray[i] == 0)
            {
                valueArray[i] = -1;
                if (digitNumber == 0)
                {
                    dotPosition = -1;
                }
                else
                {
                    digitNumber++;
                }
            }
            else
            {
                break;
            }
        }
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