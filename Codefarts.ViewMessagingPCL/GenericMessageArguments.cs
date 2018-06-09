namespace Codefarts.ViewMessaging
{
    using System.Collections.Generic;

    public class GenericMessageArguments
    {
        public static IDictionary<string, object> Build(params IDictionary<string, object>[] parts)
        {
            var args = new Dictionary<string, object>();
            foreach (var dictionary in parts)
            {
                foreach (var pair in dictionary)
                {
                    args[pair.Key] = pair.Value;
                }
            }

            return args;
        }


        public static IDictionary<string, object> Show
        {
            get
            {
                var arguments = new Dictionary<string, object>();
                arguments["show"] = string.Empty;
                return arguments;
            }
        }

        public static IDictionary<string, object> ShowDialog(string viewId)
        {
            var arguments = new Dictionary<string, object>();
            arguments["showdialog"] = viewId;
            return arguments;
        }

        public static IDictionary<string, object> SetModel(object model)
        {
            var arguments = new Dictionary<string, object>();
            arguments["setmodel"] = model;
            return arguments;
        }
    }
}