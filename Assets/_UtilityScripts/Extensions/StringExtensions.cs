
public static class StringExtensions
{

    public static string SubstringBefore(this string s, char c)
    {
        int index = s.IndexOf(c);

        if (index > 0)
        {
            return s.Substring(0, index);
        }
        else
        {
            return s;
        }
    }

    public static string SubstringAfter(this string s, char c)
    {

        int index = s.IndexOf(c);

        if (index > 0)
        {
            return s.Substring(index + 1);
        }
        else
        {
            return s;
        }
    }
}

