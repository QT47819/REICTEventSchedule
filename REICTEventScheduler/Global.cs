using REICTEventScheduler.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

public static class Global
{
    private static Person _loggedInPerson { get; set; }
    private static REICTModel _REICTModel;
    public static int NumberOfEvents = 9;
    public static REICTModel GlobalREICTModel
    {
        get
        {
            if (_REICTModel == null)
            {
                _REICTModel = new REICTModel();
            }

            return _REICTModel;
        }

        set
        {
            if (value != _REICTModel)
            {
                _REICTModel = value;
            }
        }
    }

    public static Person LoggedInPerson
    {
        get
        {
            if (_loggedInPerson == null)
                _loggedInPerson = new Person();

            return _loggedInPerson;
        }

        set
        {
            if (value != _loggedInPerson)
            {
                _loggedInPerson = value;
            }
        }
    }
}
