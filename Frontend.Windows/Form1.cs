using Backend;

namespace Frontend.Windows;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
        ConnectButtons(this); // conecta todos los botones a sus eventos, sin importar dónde estén
    }

    // Recorre los controles del formulario (y los que están dentro de paneles)
    // y le asigna a cada botón el evento que le corresponde según su texto.
    private void ConnectButtons(Control parent)
    {
        foreach (Control control in parent.Controls)
        {
            if (control is Button button)
            {
                switch (button.Text)
                {
                    case "=":
                        button.Click += BtnEqual_Click;
                        break;
                    case "Delete":
                        button.Click += BtnDelete_Click;
                        break;
                    case "Clear":
                        button.Click += BtnClear_Click;
                        break;
                    default:
                        button.Click += BtnKey_Click; // 0-9 . ( ) * / + - ^
                        break;
                }
            }
            else if (control.HasChildren)
            {
                ConnectButtons(control);
            }
        }
    }

    // Números, punto, paréntesis y operadores: se agregan al final de la expresión
    private void BtnKey_Click(object? sender, EventArgs e)
    {
        if (txtDisplay.Text.Contains('='))
        {
            txtDisplay.Text = string.Empty; // después de un resultado empieza una expresión nueva
        }
        ResetColor();
        txtDisplay.Text += ((Button)sender!).Text;
    }

    // Borra el último carácter
    private void BtnDelete_Click(object? sender, EventArgs e)
    {
        if (txtDisplay.Text.Contains('='))
        {
            txtDisplay.Text = txtDisplay.Text[..txtDisplay.Text.IndexOf('=')]; // quita "=resultado"
        }
        else if (txtDisplay.Text.Length > 0)
        {
            txtDisplay.Text = txtDisplay.Text[..^1];
        }
        ResetColor();
    }

    // Limpia todo
    private void BtnClear_Click(object? sender, EventArgs e)
    {
        txtDisplay.Text = string.Empty;
        ResetColor();
    }

    // Evalúa con el Backend y muestra "expresión=resultado"
    private void BtnEqual_Click(object? sender, EventArgs e)
    {
        var infix = txtDisplay.Text;
        if (string.IsNullOrWhiteSpace(infix) || infix.Contains('='))
        {
            return;
        }

        try
        {
            var result = ExpressionEvaluator.Evalute(infix);
            txtDisplay.Text = $"{infix}={result.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
            txtDisplay.BackColor = Color.Green;
        }
        catch (Exception)
        {
            txtDisplay.Text = $"{infix}=Error";
            txtDisplay.BackColor = Color.Red;
        }
    }

    private void ResetColor() => txtDisplay.BackColor = Color.Green;
}
