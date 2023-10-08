using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ServerSide : MonoBehaviour
{
    public InputField IpField;
    public InputField PortField;
    public Text LogText;
    public GameObject cube;


    private NetworkStream UserStream;
    private TcpClient User;

    private TcpListener server;
    private bool isListening = false;
    private bool isConnected = false;

    private StreamReader Reader;
    private StreamWriter Writer;
    public void OnCLickListen()
    {

        int port = int.Parse(PortField.text);
        string ip = IpField.text;

        server = new TcpListener(IPAddress.Any, port);

        LogText.text = "Listenning...";
        isListening = true;
        server.Start();


    }




    private async void Response(int code, string comm)
    {
        try
        {
            switch (code)
            {
                case -1:
                    await Writer.WriteLineAsync("@N," + comm);
                    await Writer.FlushAsync();
                    break;
                case 0:
                    await Writer.WriteLineAsync("@A," + comm);
                    await Writer.FlushAsync();
                    break;
                case 1:

                    await Writer.WriteLineAsync("@F," + comm);
                    await Writer.FlushAsync();
                    break;
            }
        }
        catch
        {
            closeCon();
        }
    }

    public void closeCon()
    {
        server = null;
        isListening = false;
        isConnected = false;
        LogText.text = "Stoped";
    }

    private async void ToManage(string Command)
    {
        if (Command[0] != '@')
        {
            Response(-1, "n/a");
        }
        foreach (string com in Command.Split("@"))
        {

            if (com == "")
            {
                continue;
            }
            string[] Vals = com.Split(',');


            switch (Vals[0])
            {
                case "X":

                    try
                    {
                        int X = int.Parse(Vals[1]);
                        Vector3 pos = cube.transform.position;
                        cube.transform.position = new Vector3(pos.x + X, pos.y, pos.z);
                        Response(0, "X");
                    }
                    catch
                    {
                        Response(-1, "X");

                        break;
                    }
                    break;

                case "Y":

                    try
                    {
                        int Y = int.Parse(Vals[1]);
                        Vector3 pos = cube.transform.position;
                        cube.transform.position = new Vector3(pos.x, pos.y + Y, pos.z);
                        Response(0, "Y");
                    }
                    catch
                    {
                        Response(-1, "Y");

                        break;
                    }
                    break;

                case "R":

                    try
                    {
                        int R = int.Parse(Vals[1]);
                        cube.transform.Rotate(Vector3.left, R);
                        Response(0, "R");
                    }
                    catch
                    {
                        Response(-1, "R");
                    }
                    break;

                case "S":
                    try
                    {
                        int S = int.Parse(Vals[1]);
                        cube.transform.Rotate(Vector3.up, S);
                        Response(0, "S");
                    }
                    catch
                    {
                        Response(-1, "S");
                    }
                    break;

                case "T":
                    try
                    {
                        int S = int.Parse(Vals[1]);
                        cube.transform.Rotate(Vector3.forward, S);
                        Response(0, "S");
                    }
                    catch
                    {
                        Response(-1, "S");
                    }
                    break;
                case "L":
                    try
                    {
                        int X = int.Parse(Vals[1]);
                        int Y = int.Parse(Vals[2]);
                        int Z = int.Parse(Vals[3]);
                        cube.transform.Rotate(X, Y, Z);
                        Response(0, "L");
                    }
                    catch
                    {
                        Response(-1, "L");
                    }
                    break;

                case "M":
                    try
                    {
                        int X = int.Parse(Vals[1]);
                        int Y = int.Parse(Vals[2]);
                        int Z = int.Parse(Vals[3]);
                        Vector3 pos = cube.transform.position;
                        cube.transform.position = new Vector3(pos.x + X, pos.y + Y, pos.z + Z);
                        Response(0, "M");
                    }
                    catch
                    {
                        Response(-1, "M");
                    }
                    break;
                default:
                    Response(1, Vals[0]);
                    break;
            }


        }



    }

    void Start()
    {

    }

    private async void ManageStream()
    {
        string receivedData = await Reader.ReadLineAsync();
        if (receivedData != "")
        {
            ToManage(receivedData);

            Debug.Log(receivedData);
        }


    }

    private void ImCommingCon()
    {
        User = server.AcceptTcpClient();
        UserStream = User.GetStream();
        Writer = new StreamWriter(UserStream, Encoding.UTF8);
        Reader = new StreamReader(UserStream, Encoding.UTF8);
        isConnected = true;
        LogText.text = "Connection stabliced";
    }

    void Update()
    {
        try
        {
            if (isListening)
            {
                if (!isConnected)
                {
                    if (server.Pending() && !isConnected)
                    {
                        ImCommingCon();
                    }
                }

                if (isConnected)
                {
                    if (UserStream.DataAvailable)
                    {
                        ManageStream();
                    }
                }
            }
        }
        catch
        {
            closeCon();
        }

    }
}