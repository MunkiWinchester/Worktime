using System.ComponentModel;

namespace Worktime.DataObjetcs
{
    public enum ErrorEnum
    {
        [Description("None!")] None,
        [Description("Password is incorrect!")] PasswordIncorrect,
        [Description("Personal number is invalid!")] InvalidPersonalNumber
    }
}