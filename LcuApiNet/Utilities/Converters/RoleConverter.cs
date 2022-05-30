using System.Globalization;
using LcuApiNet.Model.Enums;

namespace LcuApiNet.Utilities.Converters;

public class RoleConverter
{
    public static Role FromStringToEnum(string role)
    {
        return role == "utility" ? Role.Support : Role.FromName(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(role.ToLower()));
    }
    
}