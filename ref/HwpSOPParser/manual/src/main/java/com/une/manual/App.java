package com.une.manual;

/**
 * Hello world!
 *
 */
public class App 
{
	public static void main(String[] args) {
		// TODO Auto-generated method stub
		
		if(args.length != 2) {
			System.out.println("You must input 2 parameters! ");
			System.out.println("-------------------- ");
			System.out.println("- Command : ");
			System.out.println("- java -jar manual.jar firemanual.htm firemanual.json");			//input file, output file		
			System.out.println("-------------------- ");
			System.exit(-1);
		}
		
		SemiAutoImporter main = new SemiAutoImporter(args[0], args[1]);
		
	}
}
