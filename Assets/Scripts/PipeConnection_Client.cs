using System;
using System.IO;
using System.Text;
using System.IO.Pipes;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeConnection_Client : MonoBehaviour
{
    public static PipeConnection_Client Instasnce { get; private set; }

    private Thread clientReadThread;
    private Thread clientWriteThread;

    private StreamString streamReadString;
    private StreamString streamWriteString;

    private Queue<string> readQueue;
    private Queue<string> writeQueue;
    private Queue<string> consoleQueue;

    private object readLock;
    private object writeLock;
    private object consoleLock;

    private ShapeCommand shapeCommand = new ShapeCommand(1f);
    private ConsoleManager consoleManager;

    public bool isAlloved = true;

    private void Start()
    {
        Instasnce = this;

        if (!isAlloved) return;
        consoleManager = GameObject.Find("GameManager").GetComponent<ConsoleManager>();

        clientReadThread = new Thread(ClientThread_Read);
        clientReadThread.Start();
        clientWriteThread = new Thread(ClientThread_Write);
        clientWriteThread.Start();

        readQueue = new Queue<string>();
        writeQueue = new Queue<string>();
        consoleQueue = new Queue<string>();

        readLock = new object();
        writeLock = new object();
        consoleLock = new object();

        //writeQueue.Enqueue(JsonUtility.ToJson(shapeCommand));
    }

    private void Update()
    {
        if (!isAlloved) return;

        if (readQueue != null)
        {
            ReadMessages();
        }
        if (consoleQueue != null)
        {
            ReadConsole();
        }
    }

    private void ClientThread_Write()
    {
        NamedPipeClientStream pipeWriteClient = new NamedPipeClientStream(".", "ServerRead_ClientWrite", PipeDirection.Out);

        //try to connect
        while (!pipeWriteClient.IsConnected)
        {
            Debug.Log("Connecting write pipe to the server...");
            consoleQueue.Enqueue("Connecting write pipe to the server...");
            try
            {
                pipeWriteClient.Connect();
            }
            catch
            {
                Debug.Log("Connection Failed!");
                consoleQueue.Enqueue("Connection Failed!");
            }
            Thread.Sleep(5000);
        }
        Debug.Log("Client Write Connected!");
        consoleQueue.Enqueue("Client Write Connected!");

        try
        {
            streamWriteString = new StreamString(pipeWriteClient);
            writeQueue.Enqueue("Hello from the client!");
            consoleQueue.Enqueue("Hello from the client!");

            while (true)
            {
                string messageQueue = null;

                //lock for threadsafe
                lock (writeLock)
                {
                    if (writeQueue.Count > 0)
                    {
                        messageQueue = writeQueue.Dequeue();
                    }
                }

                if (messageQueue != null)
                {
                    //Debug.Log("SND: " + messageQueue);
                    consoleQueue.Enqueue("SND: " + messageQueue);
                    streamWriteString.WriteString(messageQueue);
                }

                Thread.Sleep(10);
            }
        }
        catch (Exception e)
        {
            Debug.Log("ERROR: " + e);
            consoleQueue.Enqueue("ERROR: " + e);
        }

        Debug.Log("Write Client pipe Closed!");
        consoleQueue.Enqueue("Write Client pipe Closed!");
        pipeWriteClient.Close();
    }
    private void ClientThread_Read()
    {
        NamedPipeClientStream pipeReadClient = new NamedPipeClientStream(".", "ServerWrite_ClientRead", PipeDirection.In);

        //try to connect
        while (!pipeReadClient.IsConnected)
        {
            Debug.Log("Connecting read pipe to the server...");
            consoleQueue.Enqueue("Connecting read pipe to the server...");
            try
            {
                pipeReadClient.Connect();
            }
            catch
            {
                Debug.Log("Connection Failed!");
                consoleQueue.Enqueue("Connection Failed!");
            }
            Thread.Sleep(3000);
        }
        Debug.Log("Client Read Connected!");
        consoleQueue.Enqueue("Client Read Connected!");
        try
        {
            streamReadString = new StreamString(pipeReadClient);

            while (true)
            {
                string message = streamReadString.ReadString();
                Debug.Log("RCV: " + message);
                consoleQueue.Enqueue("RCV: " + message);

                //lock to make it thread safe
                lock (readLock)
                {
                    readQueue.Enqueue(message);
                }
                Thread.Sleep(10);
            }
        }
        catch (Exception e)
        {
            Debug.Log("ERROR: " + e);
            consoleQueue.Enqueue("ERROR: " + e);
        }

        Debug.Log("Pipe Read closed!");
        consoleQueue.Enqueue("Pipe Read closed!");
        pipeReadClient.Close();
    }
    public class StreamString
    {
        private Stream ioStream;
        private UnicodeEncoding streamEncoding;

        public StreamString(Stream ioStream)
        {
            this.ioStream = ioStream;
            streamEncoding = new UnicodeEncoding();
        }

        public string ReadString()
        {
            int len = 0;

            len = ioStream.ReadByte() * 256;
            len += ioStream.ReadByte();
            byte[] inBuffer = new byte[len];
            ioStream.Read(inBuffer, 0, len);

            return streamEncoding.GetString(inBuffer);
        }

        public int WriteString(string outString)
        {
            byte[] outBuffer = streamEncoding.GetBytes(outString);
            int len = outBuffer.Length;
            if (len > UInt16.MaxValue)
            {
                len = (int)UInt16.MaxValue;
            }
            ioStream.WriteByte((byte)(len / 256));
            ioStream.WriteByte((byte)(len & 255));
            ioStream.Write(outBuffer, 0, len);
            ioStream.Flush();

            return outBuffer.Length + 2;
        }
    }
    public new void SendMessage(string command)
    {
        writeQueue.Enqueue(command);
    }

    public void DestroySlef()
    {
        if (!enabled) return;
        clientReadThread.Abort();
        clientWriteThread.Abort();
        Debug.Log("Client threads aborted!");
        consoleManager.PrintTcecConsole("Client threads aborted!");
    }
    private void OnApplicationQuit()
    {
        if (isAlloved)
        {
            DestroySlef();
        }
    }

    private void ReadMessages()
    {
        //hook onto the event to read messages
        lock (readLock)
        {
            if (readQueue.Count > 0)
            {
                string message = readQueue.Dequeue();
                consoleManager.PrintTceConsole(message);
            }
        }
    }
    private void ReadConsole()
    {
        //hook onto the event to read messages
        lock (consoleLock)
        {
            if (consoleQueue.Count > 0)
            {
                string message = consoleQueue.Dequeue();
                consoleManager.PrintTcecConsole(message);
            }
        }
    }
}
