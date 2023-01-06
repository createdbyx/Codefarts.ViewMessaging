namespace Codefarts.ViewMessaging
{
    using System;
    using System.Windows;

    public class WpfDataTemplate : IView
    {
        private readonly string viewPath;
        private DataTemplate templateReference;

        /// <inheritdoc />
        public string ViewName
        {
            get
            {
                return this.viewPath;
            }
        }

        /// <inheritdoc />
        public string Id
        {
            get;
        }

        public WpfViewService ViewService
        {
            get;
        }

        /// <inheritdoc />
        public object ViewReference
        {
            get;
        }

        public WpfDataTemplate(WpfViewService viewService, DataTemplate template, string path)
        {
            this.Id = Guid.NewGuid().ToString();
            this.ViewReference = template;
            this.templateReference = template;
            this.ViewService = viewService;
            this.viewPath = path;
        }
    }
}