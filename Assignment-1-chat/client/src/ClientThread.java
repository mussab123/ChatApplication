import java.io.*;
import java.net.Socket;
import java.util.LinkedList;
import java.util.Scanner;

/**
 * This helper class receives messages from server
 * and displays them
 *
 */
public class ClientThread extends Thread{

	/**
	 * socket in
	 */
	private BufferedReader socketIn;
	
	/**
	 * running flag
	 */
	volatile boolean isRunning = true;
	
	/**
	 * constructor
	 * @param socketIn
	 */
    public ClientThread(BufferedReader socketIn){
    	this.socketIn = socketIn;
    }


    @Override
    public void run(){
    	
    	try {

	    	//run until client stops
	    	while (isRunning) {
	    		
	    		String line;	    		 
	            
	    		//read line by line and print it
				while ((line = socketIn.readLine()) != null) {
				    System.out.println(line);
				}				
	    		
	    	}
    	
    	} catch (IOException e) {
    		isRunning = false;
		}
    }
}