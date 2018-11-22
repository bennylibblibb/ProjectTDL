namespace JC_SoccerWeb.UserControl
{
    using System;

    public class TabItem
    {
        private string _name;
        private string _path;

        public TabItem(string newName, string newPath)
        {
            this._name = newName;
            this._path = newPath;
        }

        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
            }
        }

        public string Path
        {
            get
            {
                return this._path;
            }
            set
            {
                this._path = value;
            }
        }
    }
}

