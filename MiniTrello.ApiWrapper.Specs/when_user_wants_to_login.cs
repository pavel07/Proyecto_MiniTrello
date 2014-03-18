using Machine.Specifications;
using Microsoft.Win32;
using MiniTrello.Domain.DataObjects;

namespace MiniTrello.ApiWrapper.Specs
{
    public class when_user_wants_to_login
    {
        static MiniTrelloSdk _minitrelloApiWrapper;
        static AccountLoginModel _accountLoginModel;
        static AuthenticationModel _result;

        Establish context = () => { _accountLoginModel = new AccountLoginModel(); };

        Because of = () => { _result = MiniTrelloSdk.Login(_accountLoginModel); };

        It should_return_the_expected_token = () => { };
    }

    /*
    public class when_user_wants_to_register
    {
        static MiniTrelloSdk _minitrelloApiWrapper;
        static AccountLoginModel _accountLoginModel;
        static AuthenticationModel _result;

        Establish context = () => { _accountLoginModel = new AccountLoginModel(); };

        Because of = () => { _result = MiniTrelloSdk.Login(_accountLoginModel); };

        It should_return_the_expected_token = () => { };
    }*/
}