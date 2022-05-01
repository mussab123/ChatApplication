import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.Socket;
import java.util.Scanner;

/**
 * client represents a chat client
 *
 */
public class Client{

	private static final String SERVER_HOST = "127.0.0.1";
	private static final int SERVER_PORT = 8888;

	/**
	 * user name
	 */
	private String userName;

	/**
	 * server host
	 */
	private String serverHost;

	/**
	 * server port
	 */
	private int serverPort;

	/**
	 * scanner to read from standard input
	 */
	private static Scanner userInputScanner = new Scanner(System.in);
	
	/**
	 * socket output stream
	 */
	private PrintWriter socketOut;
	
	/**
	 * socket input stream
	 */
	private BufferedReader socketIn;

	/**
	 * main method
	 * 
	 * @param args
	 */
	public static void main(String[] args) {

		// read chatter name
		String readName = readNotEmptyString("Please enter username: ");

		// create client and start it
		Client client = new Client(readName, SERVER_HOST, SERVER_PORT);
		client.run();
	}

	/**
	 * read not empty string
	 * 
	 * @param prompt prompt
	 * @return string
	 */
	private static String readNotEmptyString(String prompt) {

		String value = "";

		// prompt for name
		System.out.print(prompt);

		// validate name
		while (value.trim().equals("")) {
			// null, empty, whitespace(s) not allowed.
			value = userInputScanner.nextLine();
			if (value.trim().equals("")) {
				System.out.println("Invalid. Please enter again:");
			}
		}

		return value;
	}

	/**
	 * constructor
	 * 
	 * @param userName
	 * @param host
	 * @param portNumber
	 */
	public Client(String userName, String host, int portNumber) {
		this.userName = userName;
		this.serverHost = host;
		this.serverPort = portNumber;
	}

	/**
	 * called by thread, start
	 */
	public void run() {
		try {

			// create socket
			Socket socket = new Socket(serverHost, serverPort);

			// socket in/out stream
			socketOut = new PrintWriter(socket.getOutputStream(), true);
			socketIn = new BufferedReader(new InputStreamReader(socket.getInputStream()));

		
			ClientThread clientThreadHealper = new ClientThread(socketIn);
			clientThreadHealper.start();
			
			//set name
			socketOut.println(userName);

			// display menu and read user selection
			int selection = menu();
			while (selection != 0) {

				switch (selection) {

				case 1: // Create chat room
					createChatRoom();
					break;
				case 2:// List all existing rooms
					listChatRooms();
					break;
				case 3:// Join existing chat rooms
					joinChatRoom();
					break;
				case 4:// Leave a chat-room
					leaveChatRoom();
					break;
				default:
					System.out.println("Invalid selection");
				}

				System.out.println();

				// display menu and read user selection
				selection = menu();
			}
			
			clientThreadHealper.isRunning = false;
		} catch (IOException ex) {
			System.err.println("Fatal Connection error!");
			ex.printStackTrace();
		} catch (Exception ex) {
			System.out.println(ex.getMessage());
		}
		
		System.exit(0);
	}
	
	/**
	 * join chat room
	 */
	public void joinChatRoom() {
		
		String roomName = readNotEmptyString("Please enter the chat room: ");
		socketOut.println("JOIN " + roomName);
		
		String message = "";//chat message
		
		//chat until user enter empty line to exit
		while (true) {
			
			System.out.println("Enter text: ");
			message = userInputScanner.nextLine();
			
			if (message.equals("4")) {
				break;
			}
			
			socketOut.println("CHAT " + message);
		}
	}
	
	/**
	 * create chat room
	 */
	public void createChatRoom() {
		
		String roomName = readNotEmptyString("Please enter the chat room: ");
		socketOut.println("CREATE " + roomName);
	}
	
	/**
	 * display menu and return the selection
	 * 
	 * @return selection
	 */
	public static int menu() {

		
		System.out.println("1. Create chat room");
		System.out.println("2. List all existing rooms");
		System.out.println("3. Join existing chat rooms");
		System.out.println("4. Leave a chat-room");

		int selection = readInt();
		return selection;
	}

	/**
	 * read an integer from standard input
	 * 
	 * @return integer
	 */
	public static int readInt() {
		int val = 0;
		try {
			val = Integer.parseInt(userInputScanner.nextLine());
		} catch (Exception e) {

		}
		return val;
	}
	
	/**
 * list chat room
 */
private void listChatRooms() {
    socketOut.println("ROOMS");
}

    private void leaveChatRoom(){

        socketOut.println("Client " + this.userName + " has left");

        System.exit(0);

    }




}