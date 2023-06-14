
abstract class Expr 
{
    public abstract T Accept<T>(VisitorExpr<T> visitor);
}

interface VisitorExpr<T>
{
	public T VisitAssignExpr(AssignExpr expr);
	public T VisitBinaryExpr(BinaryExpr expr);
	public T VisitCallExpr(CallExpr expr);
	public T VisitGetExpr(GetExpr expr);
	public T VisitGroupingExpr(GroupingExpr expr);
	public T VisitLiteralExpr(LiteralExpr expr);
	public T VisitLogicalExpr(LogicalExpr expr);
	public T VisitSetExpr(SetExpr expr);
	public T VisitSuperExpr(SuperExpr expr);
	public T VisitThisExpr(ThisExpr expr);
	public T VisitUnaryExpr(UnaryExpr expr);
	public T VisitVariableExpr(VariableExpr expr);

}

    
class AssignExpr : Expr
{
	public Token name;
	public Expr value;
    public AssignExpr (Token name, Expr value)
    {
		this.name = name;
		this.value = value;

    }

    public override T Accept<T>(VisitorExpr<T> visitor)
    {
        return visitor.VisitAssignExpr(this);
    }
}
    
class BinaryExpr : Expr
{
	public Expr left;
	public Token operator_;
	public Expr right;
    public BinaryExpr (Expr left, Token operator_, Expr right)
    {
		this.left = left;
		this.operator_ = operator_;
		this.right = right;

    }

    public override T Accept<T>(VisitorExpr<T> visitor)
    {
        return visitor.VisitBinaryExpr(this);
    }
}
    
class CallExpr : Expr
{
	public Expr callee;
	public Token paren;
	public List<Expr?> arguments;
    public CallExpr (Expr callee, Token paren, List<Expr?> arguments)
    {
		this.callee = callee;
		this.paren = paren;
		this.arguments = arguments;

    }

    public override T Accept<T>(VisitorExpr<T> visitor)
    {
        return visitor.VisitCallExpr(this);
    }
}
    
class GetExpr : Expr
{
	public Expr object_;
	public Token name;
    public GetExpr (Expr object_, Token name)
    {
		this.object_ = object_;
		this.name = name;

    }

    public override T Accept<T>(VisitorExpr<T> visitor)
    {
        return visitor.VisitGetExpr(this);
    }
}
    
class GroupingExpr : Expr
{
	public Expr expression;
    public GroupingExpr (Expr expression)
    {
		this.expression = expression;

    }

    public override T Accept<T>(VisitorExpr<T> visitor)
    {
        return visitor.VisitGroupingExpr(this);
    }
}
    
class LiteralExpr : Expr
{
	public object? value;
    public LiteralExpr (object? value)
    {
		this.value = value;

    }

    public override T Accept<T>(VisitorExpr<T> visitor)
    {
        return visitor.VisitLiteralExpr(this);
    }
}
    
class LogicalExpr : Expr
{
	public Expr left;
	public Token operator_;
	public Expr right;
    public LogicalExpr (Expr left, Token operator_, Expr right)
    {
		this.left = left;
		this.operator_ = operator_;
		this.right = right;

    }

    public override T Accept<T>(VisitorExpr<T> visitor)
    {
        return visitor.VisitLogicalExpr(this);
    }
}
    
class SetExpr : Expr
{
	public Expr object_;
	public Token name;
	public Expr value;
    public SetExpr (Expr object_, Token name, Expr value)
    {
		this.object_ = object_;
		this.name = name;
		this.value = value;

    }

    public override T Accept<T>(VisitorExpr<T> visitor)
    {
        return visitor.VisitSetExpr(this);
    }
}
    
class SuperExpr : Expr
{
	public Token keyword;
	public Token method;
    public SuperExpr (Token keyword, Token method)
    {
		this.keyword = keyword;
		this.method = method;

    }

    public override T Accept<T>(VisitorExpr<T> visitor)
    {
        return visitor.VisitSuperExpr(this);
    }
}
    
class ThisExpr : Expr
{
	public Token keyword;
    public ThisExpr (Token keyword)
    {
		this.keyword = keyword;

    }

    public override T Accept<T>(VisitorExpr<T> visitor)
    {
        return visitor.VisitThisExpr(this);
    }
}
    
class UnaryExpr : Expr
{
	public Token operator_;
	public Expr right;
    public UnaryExpr (Token operator_, Expr right)
    {
		this.operator_ = operator_;
		this.right = right;

    }

    public override T Accept<T>(VisitorExpr<T> visitor)
    {
        return visitor.VisitUnaryExpr(this);
    }
}
    
class VariableExpr : Expr
{
	public Token name;
    public VariableExpr (Token name)
    {
		this.name = name;

    }

    public override T Accept<T>(VisitorExpr<T> visitor)
    {
        return visitor.VisitVariableExpr(this);
    }
}
    