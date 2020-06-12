// <copyright>
//   Copyright (c) 2012 Codefarts
//   All rights reserved.
//   contact@codefarts.com
//   http://www.codefarts.com
// </copyright>

namespace Codefarts.ViewMessaging
{
    public interface IViewMessage
    {
        string MessageName
        {
            get;
        }

        void SendMessage(IView view, ViewArguments args);
    }
}