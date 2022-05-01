using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Collections.Generic;

/// Server class
public class Server
{
	/// list of clients
	List<ClientHandler> clients = new List<ClientHandler>();

	/// list of rooms
	List<Room> rooms = new List<Room>();

	/// run the server
	public void run()
    {
		IPAddress ip = IPAddress.Parse("127.0.0.1");
		TcpListener server = new TcpListener(ip, 8888);

		Byte[] bytes = new Byte[256];
		server.Start();

		Console.WriteLine("Server version 5 running...");

		while (true)
		{
			TcpClient client = server.AcceptTcpClient();

			//create a object to server client
			ClientHandler handler = new ClientHandler(client, this);
			clients.Add(handler);
		}
	}

	/// create a room
	/// <param name="room"></param>
	public bool createRoom(string room)
    {
		lock (rooms)
		{
			if (getRoomByName(room) != null)
			{
				return false;
			}
		}

		//many clients creates 
		lock (rooms)
		{
			rooms.Add(new Room(room));
		}

		return true;
	}

	/// list all rooms
	/// <returns></returns>
	public string listRooms()
    {
		//all rooms
		string allRooms = "";

        lock (rooms)
        {
			foreach (var item in rooms)
			{
				allRooms += item.roomName + " ";
			}
		}
		return allRooms;
    }

	/// join a room
	/// 
	/// <param name="room"></param>
	/// <param name="user"></param>
	public bool joinRoom(string room, string user)
    {
		Room aRoom = null;
		lock (rooms)
		{
			aRoom = getRoomByName(room);
		}

        if (aRoom == null)
        {
			return false;
        }

		//many clients creates 
		lock (aRoom)
		{
			aRoom.users.Add(user);
		}

		return true;
    }

	/// set message to all clients in the room
	/// <param name="aRoom"></param>
	/// <param name="message"></param>
	public void broadcast(Room aRoom, Message message)
    {               
		foreach (var item in aRoom.users)
        {
			ClientHandler client = getClientHandlerByName(item);
			client.send(message.user + ": " + message.content);
        }	
	}

	/// chat
	/// <param name="room"></param>
	/// <param name="user"></param>
	/// <param name="message"></param>
	/// <returns></returns>
	public bool chat(string room, string user, string message)
    {
		Room aRoom = null;
		lock (rooms)
		{
			aRoom = getRoomByName(room);
		}

		if (aRoom == null)
		{
			return false;
		}

		//many clients creates 
		lock (aRoom)
		{
            if (aRoom.users.Contains(user))
            {
				Message msg = new Message(message, user);
				aRoom.messages.Add(msg);

				//broad cast in this room
				broadcast(aRoom, msg);
			}
		}

		return true;
	}

	/// get room by name
	/// <param name="room"></param>
	/// <returns></returns>
	private Room getRoomByName(string room)
    {
        foreach (var item in rooms)
        {
            if (item.roomName == room)
            {
				return item;
            }
        }
		return null; //not found
    }

	/// get client by name
	/// <param name="name"></param>
	/// <returns></returns>
	private ClientHandler getClientHandlerByName(string name)
	{
		foreach (var item in clients)
		{
			if (item.clientName == name)
			{
				return item;
			}
		}
		return null; //not found
	}

	public string leaveChatRooms(string room, string user){
	
		Room aRoom = null;
		lock (rooms)
		{
			aRoom = getRoomByName(room);
		}

        if (aRoom == null)
        {
			return user + "Coun't leave the chat";
        }

		//many clients creates 
		lock (aRoom)
		{
			aRoom.users.Remove(user);
		}

		return user + "succesfully left the room";
		
	}

	static void Main(string[] args)
	{
		Server server = new Server();
		server.run();
	}
}

	

