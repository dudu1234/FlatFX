/// <summary>
/// Extensions
/// </summary>
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public static class Extensions
{
    #region string extensions

    /// <summary>
    /// Substring a given string to a wanted length adding "..." in the end
    /// </summary>
    /// <param name="str"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string Elipssis(this string str, int length)
    {
        return (!string.IsNullOrEmpty(str) && str.Length > length) ? str.Substring(0, length) + "..." : str;
    }

    /// <summary>
    /// Converts a given string to int.
    /// Returns -1 if convertion faild.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int ToInt(this string str)
    {
        return ToInt(str, -1);
    }

    /// <summary>
    /// Converts a given string to int.
    /// Returns the defaultValue if convertion faild.
    /// </summary>
    /// <param name="str"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static int ToInt(this string str, int defaultValue)
    {
        if (!string.IsNullOrEmpty(str))
            int.TryParse(str, out defaultValue);
        return defaultValue;
    }

    /// <summary>
    /// Converts a given string to int.
    /// Returns null if convertion faild.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int? ToNullInt(this string str)
    {
        int? rv = null;
        if (!string.IsNullOrEmpty(str))
        {
            try
            {
                int.Parse(str);
            }
            catch { }
        }
        return rv;
    }

    /// <summary>
    /// Converts a given string to double.
    /// Returns -1 if convertion faild.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static double ToDouble(this string str)
    {
        return ToDouble(str, -1);
    }

    /// <summary>
    /// Converts a given string to double.
    /// Returns the defaultValue if convertion faild.
    /// </summary>
    /// <param name="str"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static double ToDouble(this string str, double defaultValue)
    {
        if (!string.IsNullOrEmpty(str))
            double.TryParse(str, out defaultValue);
        return defaultValue;
    }

    /// <summary>
    /// Converts a given string to decimal.
    /// Returns -1 if convertion faild.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static decimal ToDecimal(this string str)
    {
        return ToDecimal(str, -1);
    }

    /// <summary>
    /// Converts a given string to decimal.
    /// Returns the defaultValue if convertion faild.
    /// </summary>
    /// <param name="str"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static decimal ToDecimal(this string str, decimal defaultValue)
    {
        if (!string.IsNullOrEmpty(str))
            decimal.TryParse(str, out defaultValue);
        return defaultValue;
    }

    /// <summary>
    /// Converts a given string to byte.
    /// Returns zero if convertion faild.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static byte ToByte(this string str)
    {
        return ToByte(str, 0);
    }

    /// <summary>
    /// Converts a given string to byte.
    /// Returns the defaultValue if convertion faild.
    /// </summary>
    /// <param name="str"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static byte ToByte(this string str, byte defaultValue)
    {
        if (!string.IsNullOrEmpty(str))
            byte.TryParse(str, out defaultValue);
        return defaultValue;
    }

    /// <summary>
    /// ToDBString
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ToDBString(this string str)
    {
        if (!string.IsNullOrEmpty(str))
            return "'" + str.Replace("'", "''") + "'";
        else
            return "''";
    }

    #endregion

    #region object extensions

    /// <summary>
    /// Converts a given object to int.
    /// Returns -1 if convertion faild.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static short ToShort(this object obj)
    {
        return ToShort(obj, -1);
    }

    /// <summary>
    /// Converts a given object to int.
    /// Returns the defaultValue if convertion faild.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static short ToShort(this object obj, short defaultValue)
    {
        if (obj != null && !string.IsNullOrEmpty(obj.ToString()))
            short.TryParse(obj.ToString(), out defaultValue);
        return defaultValue;
    }

    /// <summary>
    /// Converts a given object to int.
    /// Returns -1 if convertion faild.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static int ToInt(this object obj)
    {
        return ToInt(obj, -1);
    }

    /// <summary>
    /// Converts a given object to int.
    /// Returns the defaultValue if convertion faild.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static int ToInt(this object obj, int defaultValue)
    {
        if (obj != null && !string.IsNullOrEmpty(obj.ToString()))
        {
            int.TryParse(obj.ToString(), out defaultValue);
            if (defaultValue == 0)
            {
                double rv2 = -1;
                double.TryParse(obj.ToString(), out rv2);
                if (rv2 > 0)
                    return (int)rv2;
            }
        }
        return defaultValue;
    }

    /// <summary>
    /// Converts a given object to float.
    /// Returns -1 if convertion faild.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static float ToFloat(this object obj)
    {
        return ToFloat(obj, -1);
    }

    /// <summary>
    /// Converts a given object to float.
    /// Returns the defaultValue if convertion faild.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static float ToFloat(this object obj, float defaultValue)
    {
        if (obj != null && !string.IsNullOrEmpty(obj.ToString()))
            float.TryParse(obj.ToString(), out defaultValue);
        return defaultValue;
    }

    /// <summary>
    /// Converts a given object to double.
    /// Returns -1 if convertion faild.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static double ToDouble(this object obj)
    {
        return ToDouble(obj, -1);
    }

    /// <summary>
    /// Converts a given object to double.
    /// Returns the defaultValue if convertion faild.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static double ToDouble(this object obj, double defaultValue)
    {
        if (obj != null && !string.IsNullOrEmpty(obj.ToString()))
            double.TryParse(obj.ToString(), out defaultValue);
        return defaultValue;
    }

    /// <summary>
    /// Converts a given object to decimal.
    /// Returns -1 if convertion faild.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static decimal ToDecimal(this object obj)
    {
        return ToDecimal(obj, -1);
    }

    /// <summary>
    /// Converts a given object to decimal.
    /// Returns the defaultValue if convertion faild.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static decimal ToDecimal(this object obj, decimal defaultValue)
    {
        if (obj != null && !string.IsNullOrEmpty(obj.ToString()))
            decimal.TryParse(obj.ToString(), out defaultValue);
        return defaultValue;
    }

    /// <summary>
    /// Converts a given object to long.
    /// Returns -1 if convertion faild.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static long ToLong(this object obj)
    {
        return ToLong(obj, -1);
    }

    /// <summary>
    /// Converts a given object to long.
    /// Returns the defaultValue if convertion faild.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static long ToLong(this object obj, long defaultValue)
    {
        if (obj != null && !string.IsNullOrEmpty(obj.ToString()))
            long.TryParse(obj.ToString(), out defaultValue);
        return defaultValue;
    }

    /// <summary>
    /// Converts a given string to byte.
    /// Returns zero if convertion faild.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static byte ToByte(this object obj)
    {
        return ToByte(obj, 0);
    }

    /// <summary>
    /// Converts a given string to byte.
    /// Returns the defaultValue if convertion faild.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static byte ToByte(this object obj, byte defaultValue)
    {
        if (obj != null && !string.IsNullOrEmpty(obj.ToString()))
            byte.TryParse(obj.ToString(), out defaultValue);
        return defaultValue;
    }

    /// <summary>
    /// Returns a dd/MM/yyyy date format for a given object
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static string ToDateString(this object obj, string format)
    {
        if (obj != null && !string.IsNullOrEmpty(obj.ToString()))
        {
            DateTime date = new DateTime();
            DateTime.TryParse(obj.ToString(), out date);
            return date.ToString(format);
        }
        return string.Empty;
    }

    /// <summary>
    /// ToNullString
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string ToNullString(this object obj)
    {
        return (obj != null) ? obj.ToString() : null;
    }
    /// <summary>
    /// Converts a given object to boolean
    /// Returns false if convertion faild.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool ToBoolean(this object obj)
    {
        return ToBoolean(obj, false);
    }

    /// <summary>
    /// Converts a given object to boolean
    /// Returns the defaultValue if convertion faild.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static bool ToBoolean(this object obj, bool defaultValue)
    {
        try
        {
            return (obj != null && !string.IsNullOrEmpty(obj.ToString())) ? bool.Parse(obj.ToString()) : defaultValue;
        }
        catch
        {
            return (obj.ToString() == "1");
        }
    }

    /// <summary>
    /// Converts a given object to boolean
    /// Returns the defaultValue if convertion faild.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool? ToNullBoolean(this object obj)
    {
        try
        {
            if (obj != null && !string.IsNullOrEmpty(obj.ToString()))
                return bool.Parse(obj.ToString());
            else
                return null;
        }
        catch
        {
            return (obj.ToString() == "1");
        }
    }

    /// <summary>
    /// Converts a given object to boolean
    /// Returns ' ' if convertion faild.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static char ToChar(this object obj)
    {
        return ToChar(obj, ' ');
    }

    /// <summary>
    /// Converts a given object to boolean
    /// Returns the defaultValue if convertion faild.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static char ToChar(this object obj, char defaultValue)
    {
        if (obj != null && !string.IsNullOrEmpty(obj.ToString()))
            char.TryParse(obj.ToString(), out defaultValue);
        return defaultValue;
    }

    #endregion

    #region DataTable extensions

    /*// <summary>
    /// Converts a given DataTable to a Dictionary
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TRow"></typeparam>
    /// <param name="table"></param>
    /// <param name="getKey"></param>
    /// <param name="getRow"></param>
    /// <returns></returns>
    /// public static void SampleUsage()
    /// {
    ///    DataTable t = new DataTable();
    ///    var dictionary = t.TableToDictionary(
    ///        row => row.Field<int>("ID"),
    ///        row => new {
    ///            Age = row.Field<int>("Age"),
    ///            Name = row.Field<string>("Name"),
    ///            Address = row.Field<string>("Address"),
    ///        });
    /// }*/
    /// <summary>
    /// Converts DataTable to Dictionary with generic key and value data types
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TRow"></typeparam>
    /// <param name="table"></param>
    /// <param name="getKey"></param>
    /// <param name="getRow"></param>
    /// <returns></returns>
    public static Dictionary<TKey, TRow> TableToDictionary<TKey, TRow>(this DataTable table, Func<DataRow, TKey> getKey, Func<DataRow, TRow> getRow)
    {
        return table.Rows.OfType<DataRow>().ToDictionary(getKey, getRow);
    }

    /// <summary>
    /// Converts DataTable to List with generic data types
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="table"></param>
    /// <param name="columnName"></param>
    /// <returns></returns>
    public static List<TValue> TableToList<TValue>(this DataTable table, string columnName)
    {
        return table.Rows.OfType<DataRow>().AsEnumerable()
                                           .Select(r => (TValue)r[columnName])
                                           .ToList();
    }

    /// <summary>
    /// Write all DataTable content to a .txt file
    /// </summary>
    /// <param name="dtbl"></param>
    /// <param name="filePath"></param>
    /// <param name="delimiter"></param>
    public static void TableToFile(this DataTable dtbl, string filePath, string delimiter)
    {
        using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
        {
            for (int i = 0; i < dtbl.Columns.Count; i++)
                writer.Write(dtbl.Columns[i].ColumnName + delimiter);
            writer.WriteLine();
            foreach (DataRow row in dtbl.Rows)
                writer.WriteLine(String.Join(delimiter, row.ItemArray.Select(rowData => rowData.ToString().Replace(delimiter, string.Empty)).ToArray<string>()));
        }
    }
    
    /// <summary>
    /// ListToString
    /// </summary>
    /// <param name="list"></param>
    /// <param name="delimiter"></param>
    /// <returns></returns>
    public static string ListToString<TValue>(this List<TValue> list, string delimiter)
    {
        if (list == null)
            return null;

        string returnedString = "";
        foreach (TValue val in list)
        {
            returnedString += (val == null) ? "" : val.ToString() + delimiter;
        }
        
        //remove last delimiter
        if (delimiter != null && returnedString.EndsWith(delimiter))
            returnedString = returnedString.Remove(returnedString.Length - delimiter.Length);

        return returnedString;
    }
    
    /// <summary>
    /// DictionaryToString
    /// </summary>
    /// <param name="dic"></param>
    /// <param name="delimiter"></param>
    /// <returns></returns>
    public static string DictionaryToString<TKey, TValue>(this Dictionary<TKey, TValue> dic, string delimiter)
    {
        if (dic == null)
            return null;

        StringBuilder returnedString = new StringBuilder();
        foreach (TKey obj in dic.Keys)
        {
            returnedString.Append(((obj == null) ? "" : obj.ToString()) + delimiter);
            returnedString.Append(((dic[obj] == null) ? "" : dic[obj].ToString()) + delimiter);
        }
        if (returnedString.Length > 0)
            returnedString.Remove(returnedString.Length - delimiter.Length, delimiter.Length);
        
        return returnedString.ToString();
    }
    
    #endregion
    
    /// <summary>
    /// Converts linq results to HashSet
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
    {
        return new HashSet<T>(source);
    }

    /// <summary>
    /// Compares two dictionaries with generic key and value data types
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns></returns>
    public static bool DictionaryEqual<TKey, TValue>(this IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second)
    {
        if (first == second)
            return true;
        if ((first == null) || (second == null))
            return false;
        if (first.Count != second.Count)
            return false;

        var comparer = EqualityComparer<TValue>.Default;

        foreach (KeyValuePair<TKey, TValue> kvp in first)
        {
            TValue secondValue;
            if (!second.TryGetValue(kvp.Key, out secondValue))
                return false;
            if (!comparer.Equals(kvp.Value, secondValue))
                return false;
        }
        return true;
    }
}