using Xunit;
using Interpreter;

namespace Tests;

public class TestParser
{
    public Interpreter.Interpreter _Prepare(string text) {
        Lexer lexer = new Lexer(text);
        Parser parser = new Parser(lexer);

        return new Interpreter.Interpreter(parser);
    }

    public dynamic _Interpret(string text) {
        return this._Prepare(text).Interpret();
    }

    [Fact]
    public void TestLiteral() {
        var result = this._Interpret("5;");
        Assert.Equal(5, result);

        result = this._Interpret("\"blob\";");
        Assert.Equal("blob", result);

        result = this._Interpret("7.03;");
        Assert.Equal(7.03, result);
    }

    [Fact]
    public void TestBasicOperation() {
        var result = this._Interpret("5 + 10;");
        Assert.Equal(15, result);

        result = this._Interpret("5 + 10.1;");
        Assert.Equal(15.1, result);

        result = this._Interpret("5 - 10;");
        Assert.Equal(-5, result);

        result = this._Interpret("10 / 5;");
        Assert.Equal(2, result);

        result = this._Interpret("5 % 10;");
        Assert.Equal(5, result);

        result = this._Interpret("5 * 10;");
        Assert.Equal(50, result);

        result = this._Interpret("5 ^ 2;");
        Assert.Equal(25, result);
    }

    [Fact]
    public void TestComplexOperation() {
        var result = this._Interpret("((5 * 2) + 10);");
        Assert.Equal(20, result);

        result = this._Interpret("(5 ^ 2) + (10 / 2);");
        Assert.Equal(30, result);
    }

    [Fact]
    public void TestBuiltins() {
        return;
        var result = this._Interpret("log(2);");
        Assert.Equal(System.Math.Log(2).ToString(), result);
    }

    [Fact]
    public void TestAssignment() {
        var result = this._Interpret("var a = 5; a;");
        Assert.Equal(5, result);
    }

    [Fact]
    public void TestLambda() {
        var result = this._Interpret("7 + (let x = 2 in x * x);;");
        Assert.Equal(11, result);

        result = this._Interpret("\"blob\";");
        Assert.Equal("blob", result);

        result = this._Interpret("7.03;");
        Assert.Equal(7.03, result);
    }

    [Fact]
    public void TestFinline() {
        var result = this._Interpret("function blob(x) => x*x; blob(5);");
        Assert.Equal(25, result);
    }

    [Fact]
    public void TestConditional() {
        var result = this._Interpret("if (0) \"blob\" else \"doko\";");
        Assert.Equal("doko", result);
    }

    [Fact]
    public void TestBoolean() {
        var result = this._Interpret("if (1 > 0) \"blob\" else \"doko\";");
        Assert.Equal("blob", result);

        result = this._Interpret("if (1 == 0) \"blob\" else \"doko\";");
        Assert.Equal("doko", result);
    }

    [Fact]
    public void TestRecursive() {
        var result = this._Interpret("function fib(n) => if (n > 1) fib(n-1) + fib(n-2) else 1;(fib(5));");
        Assert.Equal(8, result);
    }

    [Fact]
    public void TestFnEmptyArgs() {
        var result = this._Interpret("function blob() => \"doko\"; blob();");
        Assert.Equal("doko", result);
    }

}