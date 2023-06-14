
abstract class Stmt 
{
    public abstract T Accept<T>(VisitorStmt<T> visitor);
}

interface VisitorStmt<T>
{
	public T VisitBlockStmt(BlockStmt stmt);
	public T VisitClassStmt(ClassStmt stmt);
	public T VisitExpressionStmt(ExpressionStmt stmt);
	public T VisitFunctionStmt(FunctionStmt stmt);
	public T VisitIfStmt(IfStmt stmt);
	public T VisitPrintStmt(PrintStmt stmt);
	public T VisitReturnStmt(ReturnStmt stmt);
	public T VisitVarStmt(VarStmt stmt);
	public T VisitWhileStmt(WhileStmt stmt);

}

    
class BlockStmt : Stmt
{
	public List<Stmt?> statements;
    public BlockStmt (List<Stmt?> statements)
    {
		this.statements = statements;

    }

    public override T Accept<T>(VisitorStmt<T> visitor)
    {
        return visitor.VisitBlockStmt(this);
    }
}
    
class ClassStmt : Stmt
{
	public Token name;
	public VariableExpr? superclass;
	public List<FunctionStmt> methods;
    public ClassStmt (Token name, VariableExpr? superclass, List<FunctionStmt> methods)
    {
		this.name = name;
		this.superclass = superclass;
		this.methods = methods;

    }

    public override T Accept<T>(VisitorStmt<T> visitor)
    {
        return visitor.VisitClassStmt(this);
    }
}
    
class ExpressionStmt : Stmt
{
	public Expr expression;
    public ExpressionStmt (Expr expression)
    {
		this.expression = expression;

    }

    public override T Accept<T>(VisitorStmt<T> visitor)
    {
        return visitor.VisitExpressionStmt(this);
    }
}
    
class FunctionStmt : Stmt
{
	public Token name;
	public List<Token> params_;
	public List<Stmt?> body;
    public FunctionStmt (Token name, List<Token> params_, List<Stmt?> body)
    {
		this.name = name;
		this.params_ = params_;
		this.body = body;

    }

    public override T Accept<T>(VisitorStmt<T> visitor)
    {
        return visitor.VisitFunctionStmt(this);
    }
}
    
class IfStmt : Stmt
{
	public Expr condition;
	public Stmt thenBranch;
	public Stmt? elseBranch;
    public IfStmt (Expr condition, Stmt thenBranch, Stmt? elseBranch)
    {
		this.condition = condition;
		this.thenBranch = thenBranch;
		this.elseBranch = elseBranch;

    }

    public override T Accept<T>(VisitorStmt<T> visitor)
    {
        return visitor.VisitIfStmt(this);
    }
}
    
class PrintStmt : Stmt
{
	public Expr expression;
    public PrintStmt (Expr expression)
    {
		this.expression = expression;

    }

    public override T Accept<T>(VisitorStmt<T> visitor)
    {
        return visitor.VisitPrintStmt(this);
    }
}
    
class ReturnStmt : Stmt
{
	public Token keyword;
	public Expr? value;
    public ReturnStmt (Token keyword, Expr? value)
    {
		this.keyword = keyword;
		this.value = value;

    }

    public override T Accept<T>(VisitorStmt<T> visitor)
    {
        return visitor.VisitReturnStmt(this);
    }
}
    
class VarStmt : Stmt
{
	public Token name;
	public Expr? initializer;
    public VarStmt (Token name, Expr? initializer)
    {
		this.name = name;
		this.initializer = initializer;

    }

    public override T Accept<T>(VisitorStmt<T> visitor)
    {
        return visitor.VisitVarStmt(this);
    }
}
    
class WhileStmt : Stmt
{
	public Expr condition;
	public Stmt body;
    public WhileStmt (Expr condition, Stmt body)
    {
		this.condition = condition;
		this.body = body;

    }

    public override T Accept<T>(VisitorStmt<T> visitor)
    {
        return visitor.VisitWhileStmt(this);
    }
}
    