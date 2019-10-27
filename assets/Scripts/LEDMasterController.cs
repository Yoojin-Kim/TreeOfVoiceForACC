using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using UnityEngine.UI;

using System.IO.Ports;


public class LEDMasterController : MonoBehaviour
{

    //////////////////////////////////
    //
    // Private Variable
    //
    //////////////////////////////////
    /// <summary>
    /// 
    bool m_serialConnectionRefreshed = false;

   // public string m_portName = "COM3";// should be specified in the inspector

    SerialPort m_serialPort;

    string m_portName;


    /// </summary>
    //SerialToArduinoMgr m_SerialToArduinoMgr; 
    Thread m_Thread;

    //public SerialPort m_port;

    public byte[] m_LEDArray; // for test: 160 // 200  LEDs

    float m_Delay;
    public const int m_LEDCount = 160; // for test: 40*4 // m_LEDCount = 200

    //////////////////////////////////
    //
    // Function
    //
    //////////////////////////////////

    // Setting for events from LEDColorGenController
    //LEDColorGenController m_ledColorGenController;

    private void Awake()
    { // init me

        // Serial Communication between C# and Arduino
        //http://www.hobbytronics.co.uk/arduino-serial-buffer-size = how to change the buffer size of arduino
        //https://www.codeproject.com/Articles/473828/Arduino-Csharp-and-Serial-Interface
        //Don't forget that the Arduino reboots every time you open the serial port from the computer - 
        //and the bootloader runs for a second or two gobbling up any serial data you send its way..
        //This takes a second or so, and during that time any data that you send is lost.
        //https://forum.arduino.cc/index.php?topic=459847.0


        // Set up the serial Port

        m_serialConnectionRefreshed = Refresh();

        Debug.Log("connection=" + m_serialConnectionRefreshed);

        //m_serialPort = new SerialPort(m_portName, 115200); // bit rate= 567000 bps


        ////m_SerialPort.ReadTimeout = 50;
        //m_serialPort.ReadTimeout = 1000;  // sets the timeout value before reporting error
        //                                  //  m_SerialPort1.WriteTimeout = 5000??
        //m_serialPort.Open();


        //m_SerialToArduinoMgr = new SerialToArduinoMgr();

        //m_SerialToArduinoMgr.Setup();

        //m_port = m_SerialToArduinoMgr.port;

        m_LEDArray = new byte[m_LEDCount * 3]; //for test : 160*3 // 280*3 = 840 < 1024



    }

    void Start()
    {


        //m_ledColorGenController = gameObject.GetComponent<LEDColorGenController>();
        ////It is assumed that all the necessary components are already attached to CommHub gameObject, which  is referred to by
        //// gameObject field variable. gameObject.GetComponent<LEDColorGenController>() == this.gameObject.GetComponent<LEDColorGenController>();
        //if (m_ledColorGenController == null)
        //{
        //    Debug.LogError("The global Variable  m_ledColorGenController is not  defined");
        //    Application.Quit();
        //}

        //m_ledColorGenController.m_ledSenderHandler += UpdateLEDArray;

        // public delegate LEDSenderHandler (byte[] LEDArray); defined in LEDColorGenController
        // public event LEDSenderHandler m_ledSenderHandler;


        // define an action
        Action updateArduino = () => {

            var port = m_serialPort;
            var ledArray = m_LEDArray;

            // Write(byte[] buffer, int offset, int count);
            port.Write(ledArray, 0, ledArray.Length);
            // The WriteBufferSize of the Serial Port is 1024, whereas that of Arduino is 64
            //https://stackoverflow.com/questions/22768668/c-sharp-cant-read-full-buffer-from-serial-port-arduino
            // 버퍼를 비운다.
            Array.Clear(ledArray, 0, ledArray.Length);
        };


        m_Thread = null;
        if (m_serialConnectionRefreshed)

        { // create and start a thread for the action updateArduino
           // m_Thread = new Thread(new ThreadStart(updateArduino)); // ThreadStart() is a delegate (pointer type)
            //m_Thread.Start();
        }

        else
        {
            Debug.Log("Serial Port is connected");
            Application.Quit();
        }

    } // Start()


    public void UpdateLEDArray( byte[] ledArray)
    {
        for (int i=0; i<m_LEDCount; i++)
        {
            Debug.Log("MasterController"+i+"th LED");
            for (int j = 0; j < 2; j++) {
                m_LEDArray[i*3+j] = ledArray[i*3+j];
                Debug.Log("MasterController"+m_LEDArray[i*3+j]); //for test
            }
        }

        //var port = m_serialPort;
       // var ledArray = m_LEDArray;

        // Write(byte[] buffer, int offset, int count);
        m_serialPort.Write(m_LEDArray, 0, m_LEDArray.Length);



    }

    public bool Refresh()
    {
        if (m_serialPort!= null)
        {
            if (m_serialPort.IsOpen == true)
                return true;
            else
                m_portName = null;
        }

        var portNames = SerialPort.GetPortNames();

        for (int i =0; i < portNames.Length; i++)
        {
            Debug.Log("The Found Serial Port Name" + portNames[i]);

        }

        if (portNames.Length != 0)
        {
            m_portName = portNames[0];
            m_serialPort = new SerialPort(m_portName, 115200);
            m_serialPort.Open();
            return true;
        }
        return false;
    }




    //void Update()
    //{
    //    UpdateLEDArray(m_LEDArray);
    //}

}//public class LEDMasterController 

