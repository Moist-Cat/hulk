using System;

using Interpreter;

class Massa {
    static string msg = "H.U.L.K. REPL\nInterpreter version: 0.0.0\nREPL version: 0.0.0\n";
    
    public static void Main(string[] args) {
        Console.Write(msg);
        Lexer l = new Lexer(";");
        Parser p = new Parser(l);
        Interpreter.Interpreter i = new Interpreter.Interpreter(p);
        while (true) {
            Console.Write(">>> ");
            
            try {
                l = new Lexer(Console.ReadLine());
                p = new Parser(l);
                i.parser = p;
                Console.WriteLine(i.Interpret());
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }
    }
}
