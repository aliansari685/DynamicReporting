namespace SignalRClientTest.WinForm;

public static class ExtensionMethod
{
    public static void AppendTextNewLine(this RichTextBox box, string? text)
    {
        box.AppendText(text + "\n");
    }
}