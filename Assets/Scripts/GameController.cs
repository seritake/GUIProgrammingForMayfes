using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ComponentType
{
    Stop,
    Move,
    Wait,
    SonicSensorIf,
    Relay,
    End
}


public class ProgrammingComponent
{
    public GameObject component = null;

    public virtual string execute()
    {
        //そのコンポーネントが持つ処理を示すコマンドを返す
        return "empty";
    }

    public virtual int nextComponent()
    {
        //次に実行されるコンポーネントの番号を返す
        return 0;
    }
}

public class StopCar : ProgrammingComponent
{
    public override string execute()
    {
        //車を止める処理
        Debug.Log("stop");
        return "s,0";
    }

    public override int nextComponent()
    {
        return this.component.GetComponent<runningAreaObject>().lowerDot.GetComponent<connectDot>().connected.transform.parent.gameObject.GetComponent<runningAreaObject>().selfNumber;
    }
}

public class MoveWheel : ProgrammingComponent
{
    public enum WheelSide
    {
        Right,
        Left
    }

    public enum Direction
    {
        Forward,
        Backward
    }

    public enum Strength
    {
        Strong,
        Midium,
        Weak,
        Stop
    }


    private WheelSide side;
    private Strength str;
    private Direction dir;
    public string direction;
    public string strength;

    public MoveWheel(WheelSide side, Strength str, Direction dir)
    {
        this.side = side;
        this.str = str;
        this.dir = dir;
    }

    public override string execute()
    {
        //車輪のどちらかを動かす処理
        string speed = "";
        string command = "";
        switch (this.side)
        {
            case WheelSide.Left:
                command = "l";
                break;
            case WheelSide.Right:
                command = "r";
                break;
        }

        switch (this.dir)
        {
            case Direction.Forward:
                switch (this.str)
                {
                    case Strength.Strong:
                        speed = "0";
                        break;
                    case Strength.Midium:
                        speed = "1";
                        break;
                    case Strength.Weak:
                        speed = "2";
                        break;
                    case Strength.Stop:
                        speed = "3";
                        break;
                }
                break;
            case Direction.Backward:
                switch (this.str)
                {
                    case Strength.Strong:
                        speed = "6";
                        break;
                    case Strength.Midium:
                        speed = "5";
                        break;
                    case Strength.Weak:
                        speed = "4";
                        break;
                    case Strength.Stop:
                        speed = "3";
                        break;
                }
                break;
        }

        return command + speed;
    }

    public override int nextComponent()
    {
        return this.component.GetComponent<runningAreaObject>().lowerDot.GetComponent<connectDot>().connected.transform.parent.gameObject.GetComponent<runningAreaObject>().selfNumber;
    }
}

public class Wait : ProgrammingComponent
{
    /*
	public override string execute() {
		//ここでは何もせず、待機する処理は呼び出し元のコルーチンに書く
		Debug.Log ("wait");
	}
	*/

    public override int nextComponent()
    {
        return this.component.GetComponent<runningAreaObject>().lowerDot.GetComponent<connectDot>().connected.transform.parent.gameObject.GetComponent<runningAreaObject>().selfNumber;
    }
}

public class SonicSensorIf : ProgrammingComponent
{
    public enum Comparison
    {
        Bigger,
        Smaller
    }

    private float sensorValue;
    private float borderValue;
    private Comparison com;


    public SonicSensorIf(float borderValue, Comparison com, float sensorValue)
    {
        this.borderValue = borderValue;
        this.com = com;
        this.sensorValue = sensorValue;
    }

    /*
	public override void execute() {
		//測距センサーを読み取り、変数に格納
	}
	*/

    public override int nextComponent()
    {
        int result = 0;
        Debug.Log(this.sensorValue);
        switch (this.com)
        {
            case Comparison.Bigger:
                if (sensorValue >= borderValue)
                {
                    result = this.component.GetComponent<runningAreaObject>().lowerTrueDot.GetComponent<connectDot>().connected.transform.parent.gameObject.GetComponent<runningAreaObject>().selfNumber;
                }
                else
                {
                    result = this.component.GetComponent<runningAreaObject>().lowerFalseDot.GetComponent<connectDot>().connected.transform.parent.gameObject.GetComponent<runningAreaObject>().selfNumber;
                }
                break;
            case Comparison.Smaller:
                if (sensorValue <= borderValue)
                {
                    result = this.component.GetComponent<runningAreaObject>().lowerTrueDot.GetComponent<connectDot>().connected.transform.parent.gameObject.GetComponent<runningAreaObject>().selfNumber;
                }
                else
                {
                    result = this.component.GetComponent<runningAreaObject>().lowerFalseDot.GetComponent<connectDot>().connected.transform.parent.gameObject.GetComponent<runningAreaObject>().selfNumber;
                }
                break;
        }
        //sensorValueとborderValueを比較し、次のコンポーネントを動的に指定。
        return result;
    }
}

public class Relay : ProgrammingComponent
{
    public override int nextComponent()
    {
        return this.component.GetComponent<runningAreaObject>().lowerDot.GetComponent<connectDot>().connected.transform.parent.gameObject.GetComponent<runningAreaObject>().selfNumber;
    }
}

public class GameController : MonoBehaviour
{

    private List<GameObject> componentList = new List<GameObject>();
    public GameObject startDot;
    public GameObject endDot;
    public GameObject httpObject;
    private Http http;
    public Button startButton;
    public Button stopButton;

    public GameObject errorPanel;
    public Text errorText;

    private Color tempCol;
    private GameObject obj = null;


    // Use this for initialization
    void Start()
    {
        this.http = httpObject.GetComponent<Http>();
        stopButton.enabled = false;
        startButton.enabled = true;
        errorPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public int addComponent(GameObject obj)
    {
        componentList.Add(obj);
        return componentList.Count - 1;
    }

    public void startControll()
    {
        startButton.enabled = false;
        stopButton.enabled = true;
        StartCoroutine("control");
    }

    public void stopControl()
    {
        StopCoroutine("control");
        obj.GetComponent<Image>().color = tempCol;
        startButton.enabled = true;
        stopButton.enabled = false;
        http.setCommand("s,0");
    }

    public void updateSensor(string s)
    {

    }

    public void closeErrorPanel()
    {
        errorPanel.SetActive(false);
    }

    IEnumerator control()
    {
        ProgrammingComponent pc = null;
        try
        {
            obj = startDot.GetComponent<connectDot>().connected.transform.parent.gameObject;
        }
        catch (Exception ex)
        {
            errorText.text = "つながっていない点があります。\n「ストップ」ブロックまで点がつながっているか確かめてください。";
            errorPanel.SetActive(true);
            startButton.enabled = true;
            stopButton.enabled = false;
            yield break;
        }

        while (true)
        {
            runningAreaObject running = obj.GetComponent<runningAreaObject>();
            tempCol = obj.GetComponent<Image>().color;
            obj.GetComponent<Image>().color = Color.red;
            switch (running.type)
            {
                case ComponentType.Stop:
                    pc = new StopCar();
                    break;
                case ComponentType.Move:
                    MoveWheel.Direction dir;
                    MoveWheel.Strength str = MoveWheel.Strength.Stop;

                    if (running.dropDown1.captionText.text == "前")
                    {
                        dir = MoveWheel.Direction.Forward;
                    }
                    else
                    {
                        dir = MoveWheel.Direction.Backward;
                    }

                    if (running.dropDown2.captionText.text == "強")
                    {
                        str = MoveWheel.Strength.Strong;
                    }
                    else if (running.dropDown2.captionText.text == "中")
                    {
                        str = MoveWheel.Strength.Midium;
                    }
                    else if (running.dropDown2.captionText.text == "弱")
                    {
                        str = MoveWheel.Strength.Weak;
                    }
                    else if (running.dropDown2.captionText.text == "止")
                    {
                        str = MoveWheel.Strength.Stop;
                    }

                    MoveWheel.WheelSide side = running.wheelSide;
                    pc = new MoveWheel(side, str, dir);
                    break;
                case ComponentType.SonicSensorIf:
                    float borderValue = 0;
                    try
                    {
                        borderValue = float.Parse(running.inputField.textComponent.text);
                    }
                    catch (Exception e)
                    {
                        print("Exception");
                        errorText.text = "ちゃんと数字を入力していますか？\nわからなければ係員を呼んでください。";
                        errorPanel.SetActive(true);
                        stopControl();
                    }
                    SonicSensorIf.Comparison com;
                    if (running.dropDown1.captionText.text == "以上")
                    {
                        com = SonicSensorIf.Comparison.Bigger;
                    }
                    else
                    {
                        com = SonicSensorIf.Comparison.Smaller;
                    }
                    pc = new SonicSensorIf(borderValue, com, http.getSensor());
                    break;
                case ComponentType.Wait:
                    //ここに時間待ち処理を直書きする
                    float waitTime = 0;
                    try
                    {
                        waitTime = float.Parse(running.inputField.textComponent.text) / 1000f;
                    }
                    catch (Exception e)
                    {
                        print("Exception");
                        errorText.text = "ちゃんと数字を入力していますか？\nわからなければ係員を呼んでください。";
                        errorPanel.SetActive(true);
                        stopControl();
                    }

                    pc = new Wait();
                    yield return new WaitForSeconds(waitTime);
                    break;
                case ComponentType.End:
                    print("end");
                    http.setCommand("s,0");
                    obj.GetComponent<Image>().color = tempCol;
                    startButton.enabled = true;
                    stopButton.enabled = false;
                    yield break;
                case ComponentType.Relay:
                    pc = new Relay();
                    break;
                default:
                    print("error");
                    break;
            }
            pc.component = obj;
            string command = pc.execute();

            http.setCommand(command);
            yield return new WaitForSeconds(0.3f);

            obj.GetComponent<Image>().color = tempCol;
            try
            {
                obj = componentList[pc.nextComponent()];
            }
            catch (Exception ex)
            {
                errorText.text = "つながっていない点があります。\n「ストップ」ブロックまで点がつながっているか確かめてください。";
                errorPanel.SetActive(true);
                startButton.enabled = true;
                stopButton.enabled = false;
                http.setCommand("s,0");
                yield break;
            }

        }
    }
}
