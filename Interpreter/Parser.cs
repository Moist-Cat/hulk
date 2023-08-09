using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Interpreter;

public class NameError: Exception {
    public NameError() {}
    public NameError(string message): base(message) {}
    public NameError(string message, Exception inner): base(message, inner) {}
}

public class UnexpectedToken: Exception {
    public UnexpectedToken() {}
    public UnexpectedToken(string message): base(message) {}
    public UnexpectedToken(string message, Exception inner): base(message, inner) {}
}

public class SyntaxError: Exception {
    public SyntaxError() {}
    public SyntaxError(string message): base(message) {}
    public SyntaxError(string message, Exception inner): base(message, inner) {}
}


public class Context : Dictionary<string, AST> {

    public new AST this[string key] {
        get {
            try {
                return base[key];
            }
            catch(System.Collections.Generic.KeyNotFoundException e) {
                throw new NameError(key + " is not defined");
            }
        }
        set {
            base[key] = value;
        }
    }
}

public class AST {
    public virtual dynamic Eval(Context ctx) {
        return "";
    }

    public override string ToString() {
       return this.Eval(new Context()).ToString();
    }
}

public class Literal: AST {

    public dynamic _val {
        get;
        set;
    }

    public override dynamic Eval(Context ctx) {
        return this._val;
    }
}

public class GenericLiteral: Literal {
    
    public GenericLiteral(float val) {
        this._val = val;
    }

    public GenericLiteral(int val) {
        this._val = val;
    }

    public GenericLiteral(bool val) {
        this._val = val;
    }

    public GenericLiteral(string val) {
        this._val = val;
    }
}

public class StringLiteral: Literal {
    string _val;

    public StringLiteral(string val) {
        this._val = val;
    }
}

public class IntLiteral: Literal {
    int _val;

    public IntLiteral(int val) {
        this._val = val;
    }
}

public class FloatLiteral: Literal {
    float _val;

    public FloatLiteral(float val) {
        this._val = val;
    }
}

public class BoolLiteral: Literal {
    bool _val;

    public BoolLiteral(bool val) {
        this._val = val;
    }
}

public class BinaryOperation: AST {
    public AST left;
    public AST right;

    public BinaryOperation(AST left, AST right) {
        this.left = left;
        this.right = right;
    }

    public override dynamic Eval(Context ctx) {
        return this.Operation(this.left.Eval(ctx), this.right.Eval(ctx));
    }

    public virtual dynamic Operation(float a, float b) {
        throw new Exception("Not implemented");
    }
}

public class Sum : BinaryOperation {

    public Sum(AST left, AST right): base(right, left) {}
   
    public override dynamic Operation(float a, float b) {
        return a + b;
    }
}

public class Substraction : BinaryOperation {

    public Substraction(AST left, AST right): base(right, left) {}
   
    public override dynamic Operation(float a, float b) {
        return a + -b;
    }
}
public class Division : BinaryOperation {

    public Division(AST left, AST right): base(right, left) {}
   
    public override dynamic Operation(float a, float b) {
        return a / b;
    }
}
public class Mult : BinaryOperation {
   
    public Mult(AST left, AST right): base(right, left) {}

    public override dynamic Operation(float a, float b) {
        return a * b;
    }
}
public class Modulo : BinaryOperation {
   
    public Modulo(AST left, AST right): base(right, left) {}

    public override dynamic Operation(float a, float b) {
        return a % b;
    }
}
public class Exp : BinaryOperation {

    public Exp(AST left, AST right): base(right, left) {}
   
    public override dynamic Operation(float a, float b) {
        return (float) System.Math.Pow(a, b);
    }
}
public class Equals : BinaryOperation {

    public Equals(AST left, AST right): base(right, left) {}
   
    public override dynamic Operation(float a, float b) {
        return a == b;
    }
}
public class Higher : BinaryOperation {

    public Higher(AST left, AST right): base(right, left) {}
   
    public override dynamic Operation(float a, float b) {
        return a > b;
    }
}
public class Lower : BinaryOperation {

    public Lower(AST left, AST right): base(right, left) {}
   
    public override dynamic Operation(float a, float b) {
        return a < b;
    }
}

public class VariableDeclaration : AST {

    public string name;
    public AST expression;

    public VariableDeclaration(string name, AST expression) {
        this.name = name;
        this.expression = expression;
    }

    public override dynamic Eval(Context ctx) {
        ctx[this.name] = new GenericLiteral(this.expression.Eval(ctx));
        return null;
    }

    public override string ToString() {
        return "<(Variable) [name: " + this.name + ", value: " + this.expression.ToString() + "]>";
    }
}

public class Variable : AST {

    public string name;

    public Variable(string name) {
        this.name = name;
    }

    public override dynamic Eval(Context ctx) {
        // should be a literal
        return ctx[this.name].Eval(ctx);
        return null;
    }

    public override string ToString() {
        return "<(Variable) [name: " + this.name + "]>";
    }
}

public class BlockNode: AST {

    public List<AST> blocks;

    public BlockNode(List<AST> blocks) {
        this.blocks = blocks;
    }

    public override dynamic Eval(Context ctx) {
        AST bl;
        int counter = blocks.Count();
        foreach(AST block in this.blocks) {
            bl = block.Eval(ctx);
            counter -= 1;
            if (counter == 0) {
                return bl;
            }
        }
        return null;
    }

    public override string ToString() {
        return this.blocks.ToString();
     }
}

public class FunctionDeclaration: AST {

    public string name;
    public BlockNode args;
    public AST block_node;

    public FunctionDeclaration(string name, BlockNode args, AST block_node) {
        this.name = name;
        this.args = args;
        this.block_node = block_node;
    }

    public override dynamic Eval(Context ctx) {
        ctx[this.name] = this;
        return null;
    }

    public override string ToString() {
        return $"<(FunctionDeclaration) [name: {this.name}, args: {this.args}, block_node: {this.block_node}]>";
     }
}

public class Function : AST {
    public string name;
    public BlockNode args;

    public Function(string name, BlockNode args) {
        this.name = name;
        this.args = args;
    }

    public override dynamic Eval(Context ctx) {
        FunctionDeclaration fun_decl = (FunctionDeclaration) ctx[this.name];
        BlockNode fun_args = fun_decl.args;

        Context fun_ctx = new Context();
        Variable arg;
        for (int i = 0; i < fun_args.blocks.Count(); i++) {
            arg = (Variable) fun_args.blocks[i];
            fun_ctx[arg.name] = this.args.blocks[i].Eval(ctx);
        }
        // allow recursivity
        fun_ctx[this.name] = fun_decl;

        return fun_decl.block_node.Eval(fun_ctx);
   }

    public override string ToString() {
        return $"<(Function) [name: {this.name}, args: {this.args}]>";
    }
}

public class Lambda : AST {
    // let-in expression

    public BlockNode variables;
    public AST block_statement;

    public Lambda(BlockNode variables, AST block_statement) {
        this.variables = variables;
        this.block_statement = block_statement;
    }

    public override dynamic Eval(Context ctx) {
        Context local_ctx = new Context();

        // https://github.com/matcom/programming/tree/main/projects/hulk#variables
        // "( ... ) Fuera de una expresión let-in las variables dejan de existir. ( ... )"
        // declare variables inside the scope of the lambda
        this.variables.Eval(local_ctx);

        var res = this.block_statement.Eval(local_ctx);

        return res;
    }
}

public class Conditional : AST {
    AST hipotesis;
    BlockNode tesis;
    BlockNode antithesis;

    public Conditional(
        AST hipotesis,
        BlockNode tesis,
        BlockNode antithesis
    ) {
        this.hipotesis = hipotesis;
        this.tesis = tesis;
        this.antithesis = antithesis;
   }

    public override dynamic Eval(Context ctx) {
        bool res = this.hipotesis.Eval(ctx).ToBool();
        if (res) {
            return this.tesis.Eval(ctx);
        }
        return this.antithesis.Eval(ctx);
    } 
}

public class Parser {
    Lexer lexer;
    Token current_token;

    public Parser(Lexer lexer) {
        this.lexer = lexer;
        this.current_token = this.lexer.GetNextToken();
    }

    public void Error(Exception exception) {
        Console.WriteLine($"Error parsing line {this.lexer.line} col {this.lexer.column}");
        // XXX write last line
        Console.WriteLine(this.lexer.text);
        for (int i = 0; i < this.lexer.column - 1; i++) {
            Console.Write(" "); 
        }
        Console.Write("^");
        Console.WriteLine(); 
        throw exception;
    }

    public void Eat(string token_type) {
        if (this.current_token.type == token_type) {
            this.current_token = this.lexer.GetNextToken();
        }
        else {
            this.Error(new UnexpectedToken($"Expected {token_type} found {this.current_token.type}."));
        }
    }

    public BlockNode Arguments() {
        if (this.current_token.type != Tokens.LPAREN) {
            return null;
        }

        this.Eat(Tokens.LPAREN);
        List<AST> args = new List<AST>();
        if (this.current_token.type != Tokens.LPAREN) {
            this.Eat(Tokens.RPAREN);
            return new BlockNode(new List<AST>());
        }

       args.Add(this.Expr());
       while (this.current_token.type == Tokens.COMMA) {
           this.Eat(Tokens.COMMA);
           args.Add(this.Expr());
       }
       this.Eat(Tokens.RPAREN);

       return new BlockNode(new List<AST>());
    }

    public AST Namespace() {
        string name = this.current_token.val;
        AST node;

        this.Eat(Tokens.ID);
        BlockNode args = this.Arguments();
        if (args is not null) {
            node = new Function(name, args);
        }
        else {
            node = new Variable(name);
        }
        return node;
    }

    public Lambda Letin() {
        // meme lambda-like aberration with bizarre use-cases
        this.Eat(Tokens.LET);
        BlockNode variables = this.Assignment();
        this.Eat(Tokens.IN);

        return new Lambda(variables, this.Expr());
    }

    public BlockNode Assignment() {
        List<string> names = new List<string>();
        List<AST> variables = new List<AST>();

        string name = this.current_token.val;
        names.Add(name);

        this.Eat(Tokens.ID);
        this.Eat(Tokens.ASSIGN);
        AST val = this.Expr();

        VariableDeclaration variable = new VariableDeclaration(name, val);
        variables.Add(variable);

        while (this.current_token.type == Tokens.COMMA) {
            this.Eat(Tokens.COMMA);
            name = this.current_token.val;
            names.Add(name);

            this.Eat(Tokens.ID);
            this.Eat(Tokens.ASSIGN);
            val = this.Expr();

            variable = new VariableDeclaration(name, val);
            variables.Add(variable);
        }

        BlockNode multi_decl = new BlockNode(variables);

        return multi_decl;
    }

    public BlockNode Declaration() {
        this.Eat(Tokens.VAR);

        return this.Assignment();
    }

    public AST Function() {
        this.Eat(Tokens.FUNCTION);
        string name = this.current_token.val;
        this.Eat(Tokens.ID);
        BlockNode args = this.Arguments();

        if (this.current_token.type == Tokens.FINLINE) {
            this.Eat(Tokens.FINLINE);
            // args are variables so we can use them here too
            return new FunctionDeclaration(name, args, this.Expr());
        }
        // XXX normal fun
        return null;
    }

    public Conditional ConditionalStmt() {
        this.Eat(Tokens.IF);

        this.Eat(Tokens.LPAREN);
        AST hipotesis = this.Expr();
        this.Eat(Tokens.RPAREN);

        AST tesis = this.Expr();

        this.Eat(Tokens.ELSE);

        AST antithesis = this.Expr();

        return new Conditional(
            hipotesis,
            new BlockNode(new List<AST>{tesis}),
            new BlockNode(new List<AST>{antithesis})
        );
    }

    public Literal LiteralNode() {
        Token token = this.current_token;
        if (token.type == Tokens.STRING) {
            this.Eat(Tokens.STRING);
            return new StringLiteral(token.val);
        }
        else if (token.type == Tokens.INTEGER) {
            this.Eat(Tokens.INTEGER);
            return new IntLiteral(Convert.ToInt32(token.val));
        }
        else if (token.type == Tokens.FLOAT) {
            this.Eat(Tokens.FLOAT);
            return new FloatLiteral(Convert.ToSingle(token.val));
        }
        // else
        this.Error(new Exception($"Invalid literal {token.val} {token.type}"));
        return null;
    }

    public AST Term() {
        AST node = this.Factor();

        while (Lexer.OPERATIONS.Contains(this.current_token.type)) {
            Token token = this.current_token;
            Type ast = null;
            if (token.type == Tokens.MULT) {
                this.Eat(Tokens.MULT);
                ast = typeof(Mult);
            }
            else if (token.type == Tokens.DIV) {
                this.Eat(Tokens.DIV);
                ast = typeof(Division);
            }
            else if (token.type == Tokens.MODULO) {
                this.Eat(Tokens.MODULO);
                ast = typeof(Modulo);
            }
            else if (token.type == Tokens.EXP) {
                this.Eat(Tokens.EXP);
                ast = typeof(Exp);
            }

            AST[] args = new AST[] {node, this.Factor()};

            node = (AST) Activator.CreateInstance(
              type: ast,
              args: args
            );
        }

        return node;
    }

    public AST Factor() {
        Token token = this.current_token;
        AST node = null;

        if (Lexer.LITERALS.Contains(token.type)) {
            node = this.LiteralNode();
        }
        else if (token.type == Tokens.LET) {
            node = this.Letin();
        }
        else if (token.type == Tokens.LPAREN) {
            this.Eat(Tokens.LPAREN);
            node = this.Expr();
            this.Eat(Tokens.RPAREN);
        }
        else if (token.type == Tokens.IF) {
            node = this.ConditionalStmt();
        }
        else if (token.type == Tokens.ID) {
            node = this.Namespace();
        }
        else {
            this.Error(new Exception(token.val));
        }

        return node;
    }

    public AST Expr() {
        AST node = this.Term();

        while (new HashSet<string>{Tokens.PLUS, Tokens.MINUS}.Union(Lexer.CONDITIONALS).Contains(this.current_token.type)) {
            Token token = this.current_token;
            Type ast = null;
            if (token.type == Tokens.PLUS) {
                this.Eat(Tokens.PLUS);
                ast = typeof(Sum);
            }
            else if (token.type == Tokens.MINUS) {
                this.Eat(Tokens.MINUS);
                ast = typeof(Substraction);
            }
            else if (token.type == Tokens.EQUALS) {
                this.Eat(Tokens.EQUALS);
                ast = typeof(Equals);
            }
            else if (token.type == Tokens.HIGHER) {
                this.Eat(Tokens.HIGHER);
                ast = typeof(Higher);
            }
            else if (token.type == Tokens.LOWER) {
                this.Eat(Tokens.LOWER);
                ast = typeof(Lower);
            }
            else {
                this.Error(new Exception("Ehhhh??? まじ？？"));
            }

            node = (AST) Activator.CreateInstance(ast, args: new Object[] {node, this.Factor()});
        }

        return node;
    }

    public AST _Parse() {
        AST node;

        if (this.current_token.type == Tokens.VAR) {
            node = this.Declaration();
        }
        else if (this.current_token.type == Tokens.FUNCTION) {
            node = this.Function();
        }
        else {
            node = this.Expr();
        }
        if (this.current_token.type != Tokens.END) {
            this.Error(new SyntaxError("Expected ';'"));
        }

        return node;
    }

    public AST Parse() {
        List<AST> nodes = new List<AST>();
        AST node;

        while (this.current_token.type != Tokens.EOF) {
            node = this._Parse();
            this.Eat(Tokens.END);
            nodes.Add(node);
        }

        return new BlockNode(nodes);
    }
}

public class Interpreter {
    Context GLOBAL_SCOPE = new Context();
    Parser parser;
    AST _tree;

    public Interpreter(Parser parser) {
        this.parser = parser;
        this._tree = null;
    }

    public AST tree {
        get {
            if (this._tree is null) {
                this._tree = this.parser.Parse();
            }
            return this._tree;
        }
        set {}
    }

    public dynamic Interpret() {
        if (this.tree is null) {
            return "";
        }
        return this.tree.Eval(this.GLOBAL_SCOPE);
    }
}
