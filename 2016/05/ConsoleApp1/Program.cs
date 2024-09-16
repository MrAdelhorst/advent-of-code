var input = "ugkcyxxp";
var md5Calculator = System.Security.Cryptography.MD5.Create();
md5Calculator.Initialize();

//Prepare animating the result
Console.CursorVisible = false;
Console.Write($"Part 1...Decoding...[");
var password1Start = Console.GetCursorPosition();
Console.Write("________]");
Console.WriteLine();
Console.Write($"Part 2...Decoding...[");
var password2Start = Console.GetCursorPosition();
Console.Write("________]");

//Find passwords while animating the process
var current = 0;
var password = new char[8];
var password2 = new char[8];
while (!password2.IsComplete())
{
    var hash = md5Calculator.ComputeHash($"{input}{current}".Select(c => (byte)c).ToArray());
    var hexValue = Convert.ToHexString(hash);
    if (hexValue.StartsWith("00000"))
    {
        if (!password.IsComplete())
            password[Array.IndexOf(password, '\0')] = hexValue[5];

        var position = hexValue[5] - '0';
        if (position < 8 && password2[position] == '\0')
            password2[position] = hexValue[6];

        Extensions.PrintPassword(password.ToArray(), password1Start);
        Extensions.PrintPassword(password2, password2Start);
    }

    Extensions.Animate(password.IsComplete() ? default : password1Start, password2.IsComplete() ? default : password2Start);
    current++;
}

Console.ReadLine();

static class Extensions
{
    public static bool IsComplete(this char[] password) => password.All(c => c != '\0');

    public static void PrintPassword(char[] password, (int Left, int Top) position)
    {
        Console.SetCursorPosition(position.Left, position.Top);
        Console.Write($"{string.Join("", password.Select(c => c == '\0' ? '_' : c))}]");

        if (password.All(c => c != '\0'))
            Console.Write("...PASSWORD FOUND!");
        else
            Console.Write("   ");
    }

    static char progress = '|';
    static int count = 0;
    public static void Animate((int Left, int Top) position, (int Left, int Top) position2)
    {
        if (++count % 100000 == 0)
        {
            progress = progress switch
            {
                '|' => '/',
                '/' => '-',
                '-' => '\\',
                '\\' => '|'
            };

            if (position != default)
            {
                Console.SetCursorPosition(position.Left + 10, position.Top);
                Console.Write($"{progress}");
            }
            if (position2 != default)
            {
                Console.SetCursorPosition(position2.Left + 10, position2.Top);
                Console.Write($"{progress}");
            }
        }
    }
}