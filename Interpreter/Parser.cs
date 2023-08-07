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
    public AST expression;

    public Variable(string name, AST expression) {
        this.name = name;
    }

    public override dynamic Eval(Context ctx) {
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

class Function : AST {
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

class Lambda : AST {
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

public class Parser {

    public Parser(Lexer lexer) {//XXX}
}

public class Interpreter {
    public Interpreter(Parser parser) {}

    public dynamic Interpret() {
       return 1; 
    }
}
