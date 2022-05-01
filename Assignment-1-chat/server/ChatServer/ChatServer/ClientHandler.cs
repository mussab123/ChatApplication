using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Threading;

/// Client handler for one client connection

public class ClientHandler
{
    
    /// TCP client socket
    TcpClient clientSocket;

    
    /// client name
    public string clientName;

    
    /// reference to server
    private Server server;

    
    /// current room
    private string currentRoom = "";

   
    /// constructor
    /// <param name="inClientSocket"></param>
    public ClientHandler(TcpClient inClientSocket, Server server)
    {
        this.clientSocket = inClientSocket;
        this.server = server;
        Thread ctThread = new Thread(doCommand);
        ctThread.Start();
    }

    
    /// receive command from client and process it
    private void doCommand()
    {
        //input stream
        NetworkStream networkStream = clientSocket.GetStream();

        //buffer
        byte[] bytesFrom = new byte[65536];

        //read name
        //read from client
        networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
        clientName = System.Text.Encoding.ASCII.GetString(bytesFrom);
        clientName = clientName.Substring(0, clientName.IndexOf("\n")).Trim();
        Console.WriteLine(clientName);

        while (true)
        {
            try
            {              

                //read from client
                networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                string dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("\n")).Trim();

                Console.WriteLine(dataFromClient);

                if (dataFromClient.StartsWith("CREATE"))
                {
                    doCreateRoom(dataFromClient);

                }else if (dataFromClient.StartsWith("JOIN"))
                {
                    doJoinRoom(dataFromClient);
                }
                else if (dataFromClient.StartsWith("CHAT"))
                {
                    doChat(dataFromClient);
                }
                else if (dataFromClient.StartsWith("ROOMS"))
                {
                    listChatRooms();
                }

                else if (dataFromClient.StartsWith("leave"))
                {
                    leaveChat();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                break;
            }
        }//end while
    }//end doCommand

    
    /// send message to client
    /// <param name="message"></param>
    public void send(String message)
    {
        //input stream
        NetworkStream networkStream = clientSocket.GetStream();

        //buffer
        byte[] bytesTo = Encoding.ASCII.GetBytes(message + "\r\n");
        networkStream.Write(bytesTo, 0, bytesTo.Length);
        networkStream.Flush();
    }

   
    /// create room commands
    /// <param name="command"></param>
    private void doCreateRoom(string command)
    {
        string room = command.Substring(command.IndexOf(" ") + 1);
        if (server.createRoom(room))
        {
            send("Created room " + room + " successfully");
        }
        else
        {
            send("Could not create room " + room + " - Room existing");
        }
    }

    
    /// join room
    /// <param name="command"></param>
    private void doJoinRoom(string command)
    {
        string room = command.Substring(command.IndexOf(" ") + 1);
        if (server.joinRoom(room, clientName))
        {
            currentRoom = room;
            send("Joined room " + room + " successfully");
        }
        else
        {
            send("Could not join room " + room + " - Room not existing");
        }
    }

    
    /// chat in room
    /// <param name="command"></param>
    private void doChat(string command)
    {
        string message = command.Substring(command.IndexOf(" ") + 1);

        //this method broadcast
        server.chat(currentRoom, clientName, message);
    }

    
    /// list chat rooms
    private void listChatRooms()
    {
        send(server.listRooms());
    }

    private void leaveChat()
    {
        send("Client left " + clientName);
    }
}

