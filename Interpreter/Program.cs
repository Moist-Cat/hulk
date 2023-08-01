using System;
namespace Interpreter;


class Massa {
    public static void Main(string[] args) {
        Console.Write(">>> ");
         Lexer l = new Lexer(Console.ReadLine());
         Console.WriteLine(l.GetNextToken());
         Console.WriteLine(l.GetNextToken());
         Console.WriteLine(l.GetNextToken());
         Console.WriteLine(l.GetNextToken());
         Console.WriteLine(l.GetNextToken());
    }
}
