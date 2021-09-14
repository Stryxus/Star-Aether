using System;

namespace UEESA.Client.Data.States;

public class UserState
{
    internal event Action OnUserLoggedInStateChanged;
    private bool isLoggedIn;
    internal bool IsLoggedIn
    {
        get
        {
            return isLoggedIn;
        }

        private set
        {
            isLoggedIn = value;
        }
    }

    public UserState() 
    {

    }
}
