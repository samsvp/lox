from typing import *


def define_ast(filename: str, basename: str, types: List[str]):
    code = f"""
abstract class {basename} 
{{
    public abstract T Accept<T>(Visitor{basename}<T> visitor);
}}
{define_visitor(basename, types)}
    """
    for t in types:
        class_name = t.split(":")[0].strip() 
        fieldlist = t.split(":")[1].strip()
        code += define_type(basename, class_name, fieldlist)
    with open(filename, "w") as f:
        f.write(code)


def define_type(basename: str, classname: str, fieldlist: str):
    fields_declaration = "\n".join([f"\tpublic {field};" for field in fieldlist.split(", ")])
    fields_definition = ""
    for field in fieldlist.split(", "):
        name = field.split(" ")[1]
        fields_definition += f"\t\tthis.{name} = {name};\n"
    
    code = f"""
class {classname} : {basename}
{{
{fields_declaration}
    public {classname} ({fieldlist})
    {{
{fields_definition}
    }}

    public override T Accept<T>(Visitor{basename}<T> visitor)
    {{
        return visitor.Visit{classname}(this);
    }}
}}
    """
    return code


def define_visitor(basename: str, types: List[str]):
    function_names = ""
    for t in types:
        typename = t.split(":")[0].strip()
        function_names += f"\tpublic T Visit{typename}({typename} {basename.lower()});\n"
    code = f"""
interface Visitor{basename}<T>
{{
{function_names}
}}
"""
    return code


if __name__ == "__main__":
    import sys
    define_ast(sys.argv[1], "Expr", [
        "AssignExpr   : Token name, Expr value",
        "BinaryExpr   : Expr left, Token operator_, Expr right",
        "CallExpr     : Expr callee, Token paren, List<Expr?> arguments",
        "GetExpr      : Expr object_, Token name",
        "GroupingExpr : Expr expression",
        "LiteralExpr  : object? value",
        "LogicalExpr  : Expr left, Token operator_, Expr right",
        "SetExpr      : Expr object_, Token name, Expr value",
        "SuperExpr    : Token keyword, Token method",
        "ThisExpr     : Token keyword",
        "UnaryExpr    : Token operator_, Expr right",
        "VariableExpr : Token name"
    ])

    define_ast(sys.argv[2], "Stmt", [
        "BlockStmt      : List<Stmt?> statements",
        "ClassStmt      : Token name, VariableExpr? superclass, List<FunctionStmt> methods",
        "ExpressionStmt : Expr expression",
        "FunctionStmt   : Token name, List<Token> params_, List<Stmt?> body",
        "IfStmt         : Expr condition, Stmt thenBranch, Stmt? elseBranch",
        "PrintStmt      : Expr expression",
        "ReturnStmt     : Token keyword, Expr? value",
        "VarStmt        : Token name, Expr? initializer",
        "WhileStmt      : Expr condition, Stmt body"
    ])