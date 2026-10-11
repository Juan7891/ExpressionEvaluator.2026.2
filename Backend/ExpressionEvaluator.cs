using System.Globalization;

namespace Backend;

public static class ExpressionEvaluator
{
    public static double Evalute(string infix) => EvalutePostfix(ToPostfix(infix));

    // CAMBIO: ahora el postfijo se arma por TOKENS separados por espacio.
    // Ej: "12+3.5" => "12 3.5 +"   (antes: "12+3.5" => "1235.+", que se perdía)
    private static string ToPostfix(string infix)
    {
        var posfix = string.Empty;
        var stack = new Stack<char>();
        var number = string.Empty; // NUEVO: acumula los dígitos y el punto de un número

        foreach (var item in infix)
        {
            if (char.IsWhiteSpace(item))
            {
                continue; // NUEVO: ignora espacios
            }

            if (IsOperator(item))
            {
                // NUEVO: al llegar un operador, el número que se venía armando terminó
                if (number != string.Empty)
                {
                    posfix += number + " ";
                    number = string.Empty;
                }

                if (item == ')')
                {
                    var ope = stack.Pop();
                    while (ope != '(')
                    {
                        posfix += ope + " ";
                        ope = stack.Pop();
                    }
                }
                else
                {
                    // CAMBIO: antes se sacaba UN solo operador de la pila (if/else).
                    // Ahora se sacan TODOS los de prioridad mayor o igual (while).
                    // Así "10-2*3-4" da 0 y no 8.
                    while (stack.Count > 0 && PriorityInfix(item) <= PriorityStack(stack.Peek()))
                    {
                        posfix += stack.Pop() + " ";
                    }
                    stack.Push(item);
                }
            }
            else
            {
                number += item; // CAMBIO: antes era posfix += item (dígito por dígito)
            }
        }

        // NUEVO: el último número de la expresión (ej: el "1" en "...-1")
        if (number != string.Empty)
        {
            posfix += number + " ";
        }

        // CAMBIO: while en vez de do-while, para no hacer Pop() con la pila vacía (ej: "7")
        while (stack.Count != 0)
        {
            posfix += stack.Pop() + " ";
        }

        return posfix.Trim();
    }

    private static int PriorityStack(char op) => op switch
    {
        '^' => 3,
        '*' => 2,
        '/' => 2,
        '+' => 1,
        '-' => 1,
        '(' => 0,
        _ => throw new Exception("Invalid expression."),
    };

    private static int PriorityInfix(char op) => op switch
    {
        '^' => 4,
        '*' => 2,
        '/' => 2,
        '+' => 1,
        '-' => 1,
        '(' => 5,
        _ => throw new Exception("Invalid expression."),
    };

    private static bool IsOperator(char item) => item == '^' || item == '*' || item == '/' || item == '+' || item == '-' || item == '(' || item == ')';

    // CAMBIO: se recorre por tokens (Split) y no por caracteres
    private static double EvalutePostfix(string postfix)
    {
        var stack = new Stack<double>();
        var tokens = postfix.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (var token in tokens)
        {
            if (token.Length == 1 && IsOperator(token[0]))
            {
                if (stack.Count < 2)
                {
                    throw new Exception("Invalid expression.");
                }
                var ope2 = stack.Pop();
                var ope1 = stack.Pop();
                stack.Push(Calculate(ope1, ope2, token[0]));
            }
            else
            {
                // CAMBIO: antes char.GetNumericValue(item), que solo sirve para UN dígito.
                // InvariantCulture => el punto siempre es el separador decimal
                // (en Colombia la cultura usa coma y "3.1416" se leería mal).
                if (!double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out var number))
                {
                    throw new Exception($"Invalid number: {token}");
                }
                stack.Push(number);
            }
        }

        if (stack.Count != 1)
        {
            throw new Exception("Invalid expression.");
        }
        return stack.Pop();
    }

    private static double Calculate(double ope1, double ope2, char item) => item switch
    {
        '*' => ope1 * ope2,
        '/' => ope1 / ope2,
        '+' => ope1 + ope2,
        '-' => ope1 - ope2,
        '^' => Math.Pow(ope1, ope2),
        _ => throw new Exception("Invalid expression."),
    };
}