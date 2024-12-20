using System.ComponentModel.DataAnnotations;

byte[] prog = [2, 4, 1, 5, 7, 5, 1, 6, 4, 1, 5, 5, 0, 3, 3, 0];

//long a = 35184372088832;
long a = 35409070377814;
var res = new byte[16];
bool equal = false;
while (!equal)
{
    Array.Fill(res, (byte)0);
    equal = true;
    var tmp = a;
    for (int i = 0; i < 16; i++)
    {
        var x1 = (byte)(tmp & 7) ^ 0b_011;
        var x2 = (byte)(tmp & 7) ^ 0b_101;
        var x3 = (byte)(tmp >> x2) & 7;
        res[i] = (byte)(x1 ^ x3);
        if (res[i] != prog[i])
        {
            equal = false;
            break;
        }
        tmp /= 8;
    }

    a++;
}

Console.WriteLine($"res: {a}");
Console.ReadLine();