using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Message
{
    /// message text
    public string content { get; }

    /// from user
    public string user { get; }

    /// constructor
    /// <param name="content"></param>
    /// <param name="user"></param>
    public Message(string content, string user)
    {
        this.content = content;
        this.user = user;
    }
}

/// cat room
public class Room
{
    /// room name, unique
    public String roomName { get; }

    /// list of messages in room
    public List<Message> messages { get; }

    /// list of users in room
    public List<string> users { get; }

    /// constructor
    /// <param name="roomName"></param>
    public Room(string roomName)
    {
        this.roomName = roomName;
        this.users = new List<string>();
        this.messages = new List<Message>();
    }
}
