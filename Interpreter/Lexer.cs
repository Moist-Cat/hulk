using System;

namespace Interpreter;

public class Tokens {
    public static string ID = "ID";
    public static string INTEGER = "INTEGER";
    public static string FLOAT = "FLOAT";
    public static string STRING = "STRING";
    public static string PLUS = "+";
    public static string  MINUS = "-";
    public static string MULT = "*";
    public static string DIV = "/";
    public static string MODULO = "%";
    public static string EXP = "^";
    public static string LPAREN = "(";
    public static string RPAREN = ")";
    public static string HIGHER = ">";
    public static string EQUALS = "==";
    public static string LOWER = "<";
    public static string ASSIGN = "=";
    public static string FINLINE = "=>";
    public static string DOT = ".";
    public static string END = ";";
    public static string EOF = "";
    public static string VAR = "var";
    public static string LET = "let";
    public static string IN = "in";
    public static string IF = "if";
    public static string  ELSE = "else";
    public static string  FUNCTION = "function";
    public static string QUOTATION = "\"";
    public static string  COMMA = ",";
}

public class Token {
    string type;
    string val;

    public Token(string type_, string val = "") {
        this.type = type_;
        if (val == "") {
            this.val = this.type;
        }
        else {
            this.val = val;
        }
    }

    public override string ToString() {
        return "<Token(" + this.type + ", " + this.val + ")>";
    }
}

public class Lexer {
    public string text;
    public int pos;
    public string current_char;
    public int line;
    public int column;

    public static HashSet<string> LITERALS = new HashSet<string>{Tokens.STRING, Tokens.INTEGER, Tokens.FLOAT};
    public static HashSet<string> CONDITIONALS = new HashSet<string>{Tokens.IF, Tokens.ELSE};
    public static HashSet<string> RESERVED_KEYWORDS = new HashSet<string>{
        Tokens.IN, Tokens.LET, Tokens.VAR, Tokens.FUNCTION, Tokens.IF, Tokens.ELSE
    };

    public Lexer(string text) {
        this.text = text;
        this.pos = 0;
        this.current_char = self.text[self.pos];

        this.line = 0;
        this.column = 0;
    }

    public void Error(Exception exception):
        Console.WriteLine(
            "Error lexing line " + (string) this.line + " col " + (string) this.column
        );
        Console.WriteLine(this.text);
        for (int i = 0; i < this.column; i++) {
            Console.Write(" ");
        }
        Console.Write("^");
        Console.WriteLine();
        throw exception;

    public string GetResult(string condition) {
        result = "";
        // XXX condition
    }
