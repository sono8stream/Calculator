using System;//追加しました
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//入力された複数の値(小数含む)を計算するプログラムです
public class Calculator : MonoBehaviour
{
    const int MAXdigit = 8;//最大桁数

    [SerializeField]
    List<int[]> values;//入力値の配列
    List<int> dotPositions;//入力値の小数点の位置
    List<int> operatorNumbers;//四則演算子の配列

    int[] valueArray;//現在の入力値、各桁を配列で管理
    int digitCounter;//入力値の桁数カウンター
    int dotPosition;//入力値の小数点の位置

    Text valueText;

    // Use this for initialization
    void Start()
    {
        valueText = GetComponent<Text>();
        InitializeCalculation();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void InitializeValueArray()
    {
        valueArray = new int[MAXdigit];
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
        values = new List<int[]>();
        dotPositions = new List<int>();
        operatorNumbers = new List<int>();

        InitializeValueArray();

        valueText.text = "0";
    }

    //valueArrayを数値に変換し、valuesに格納するメソッド
    void AddSection()
    {
        values.Add(valueArray);
        dotPositions.Add(dotPosition);

        InitializeValueArray();
    }

    //配列からdouble値に変換するメソッド
    double ValueArrayToValue(int[] array,int digCounter, int dotPos)
    {
        double value = 0;
        for (int i = 0; i < digCounter; i++)
        {
            value = value * 10 + valueArray[i];
        }

        if (dotPos != -1)
        {
            value /= Mathf.Pow(10, digCounter - dotPos - 1);//小数点の位置で桁を補正する
        }
        return value;
    }

    //doubleの計算値を配列に変換して返すメソッド
    void ValueToValueArray(double value, ref int[] array, ref int dotPos)
    {
        int digitNumber = (int)Math.Floor(Math.Log10(value)) + 1;//最大桁数を取得
        int nowNumber;//現在桁の値
        //値を割り当て
        for (int i = 0; i < MAXdigit; i++)
        {
            if (digitNumber == 1)//1の位であれば小数点を設定
            {
                dotPos = i;
            }
            nowNumber = (int)Math.Floor(value / Math.Pow(10, digitNumber - 1));//最大桁の値
            array[i] = nowNumber;
            value -= nowNumber * Math.Pow(10, digitNumber - 1);//最大桁を1つ下げる
            digitNumber--;
        }

        //最小桁から見てゆき、小数点以下かつ0ならば未入力状態(-1)に
        for (int i = MAXdigit - 1; i >= 0; i--)
        {
            if (digitNumber <= 0 && array[i] == 0)
            {
                array[i] = -1;
                if (digitNumber == 0)//小数第一位まで入力がなければ小数点を未設定に
                {
                    dotPos = -1;
                }
                digitNumber++;
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
        if (digitCounter >= MAXdigit || (digitCounter == 0 && number == 0)) return;

        valueArray[digitCounter]=number;
        digitCounter++;
        valueText.text = ValueArrayToValue(valueArray,digitCounter, dotPosition).ToString();
    }

    public void DeleteNumber()
    {
        if (digitCounter == 0) return;

        valueArray[digitCounter] = -1;
        digitCounter--;
        if (digitCounter < dotPosition)
        {
            dotPosition = -1;
        }
        valueText.text = ValueArrayToValue(valueArray, digitCounter, dotPosition).ToString();
    }

    //四則演算子ボタンを押したとき
    public void SetOperator(int operatorNo)
    {
        operatorNumbers.Add(operatorNo);
        InitializeValueArray();
    }

    //計算処理
    double Calculate(double v1,double v2,int operatorNo)
    {
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

        return result;

        //InitializeCalculation();
    }

    public void CalculateAll()
    {
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