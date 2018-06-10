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
    List<double> values;//入力値の配列
    List<int> operatorNumbers;//四則演算子の配列

    double inputValue;//現在の入力値、各桁を配列で管理
    int nowDigitCounter;//入力値の桁数カウンター
    int dotPosition;
    bool inMinus;

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

    void InitializeInputValue()
    {
        inputValue = 0;
        nowDigitCounter = 0;
        dotPosition = -1;//小数点未設定時は-1
        inMinus = false;
    }

    public void InitializeCalculation()
    {
        values = new List<double>();
        operatorNumbers = new List<int>();

        InitializeInputValue();

        valueText.text = "0";
    }

    //valueArrayを数値に変換し、valuesに格納するメソッド
    void AddSection()
    {
        values.Add(inputValue);

        InitializeInputValue();
    }

    //double値を最大桁数内で画面に表示
    void ShowValue(double value)
    {
        string s = "";
        if (value < 0)//負の値ならば正に変換しておく
        {
            s += "-";
            value *= -1;
        }
        int digitNumber = (int)Math.Floor(Math.Log10(value)) + 1;//最大桁数を取得

        if (digitNumber > MAXdigit)//許容可能な最大桁数以上の場合、エラーを吐いて終了
        {
            s = "FLOW";
        }
        else
        {

            int nowNumber;//現在桁の値
                          //値を割り当て
            for (int i = 0; i < MAXdigit; i++)
            {
                if (digitNumber == 0)//1の位であれば小数点を設定
                {
                    s += ".";
                }
                nowNumber = (int)Math.Floor(value / Math.Pow(10, digitNumber - 1));//最大桁の値
                s += nowNumber.ToString();

                value -= nowNumber * Math.Pow(10, digitNumber - 1);//最大桁を1つ下げる
                digitNumber--;
            }

            //右端から見てゆき、小数点以下かつ0ならば消す
            for (int i = MAXdigit - 1; i >= 0; i--)
            {
                if (digitNumber <= 0 && s[s.Length - 1] == '0')
                {
                    s.Substring(0, s.Length - 1);
                    if (digitNumber == 0)//小数第一位まで入力がなければ小数点を未設定に
                    {
                        s.Substring(0, s.Length - 1);
                    }
                    digitNumber++;
                }
                else
                {
                    break;
                }
            }
        }

        valueText.text = s;
    }

    //数字ボタンを押したとき
    public void InputNumber(int number)
    {
        if (nowDigitCounter >= MAXdigit || (nowDigitCounter == 0 && number == 0)) return;

        if (inMinus)
        {
            number *= -1;
        }

        if (dotPosition==-1)//小数未設定のとき
        {
            inputValue = inputValue * 10 + number;
        }
        else
        {
            inputValue += number * Math.Pow(10, dotPosition - nowDigitCounter);
        }
        nowDigitCounter++;
        ShowValue(inputValue);
    }

    public void DeleteNumber()
    {
        if (nowDigitCounter == 0) return;

        int index;//指数
        if (dotPosition == -1)
        {
            index = -1;
        }
        else
        {
            index = nowDigitCounter - dotPosition - 2;
        }
        inputValue *= Math.Pow(10, index);
        Math.Truncate(inputValue);//小数点以下丸め込み
        inputValue *= Math.Pow(10, -index);

        nowDigitCounter--;
        if (nowDigitCounter == dotPosition + 1)
        {
            dotPosition = -1;
        }
        ShowValue(inputValue);
    }

    //四則演算子ボタンを押したとき
    public void SetOperator(int operatorNo)
    {
        operatorNumbers.Add(operatorNo);
        AddSection();
        InitializeInputValue();
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
    }

    public void CalculateAll()
    {
        if (values.Count == 0) return;

        double result = values[0];
        for (int i = 1; i < values.Count && i < operatorNumbers.Count; i++)
        {
            Calculate(result, values[i], operatorNumbers[i]);
        }

        ShowValue(result);
        values[ = ValueTextToArray(ref dotPosition1, ref digitCounter1);
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