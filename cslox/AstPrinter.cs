// using System.Text;

// class AstPrinter : VisitorExpr<string>
// {
//     public string Print(Expr expr)
//     {
//         return expr.Accept(this);
//     }

//     public string VisitBinaryExpr(BinaryExpr expr) 
//     {
//         return Parenthesize(expr.operator_.lexeme,
//                             expr.left, expr.right);
//     }

//     public string VisitGroupingExpr(GroupingExpr expr) 
//     {
//         return Parenthesize("group", expr.expression);
//     }

//     public string VisitLiteralExpr(LiteralExpr expr) 
//     {
//         return expr.value?.ToString() ?? "nil";
//     }

//     public string VisitUnaryExpr(UnaryExpr expr) 
//     {
//         return Parenthesize(expr.operator_.lexeme, expr.right);
//     }


//     public string VisitVariableExpr(VariableExpr expr)
//     {
//         return expr.name.lexeme?.ToString()?? "";
//     }


//     private string Parenthesize(string name, params Expr[] exprs)
//     {
//         StringBuilder builder = new StringBuilder();

//         builder.Append($"({name}");
//         foreach (Expr expr in exprs)
//         {
//             builder.Append($" {expr.Accept(this)}");
//         }
//         builder.Append($")");

//         return builder.ToString();
//     }
// } 